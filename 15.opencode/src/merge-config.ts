import { chmod, copyFile, mkdir, open, readFile, readdir, rename, rm, writeFile } from "node:fs/promises"
import { existsSync } from "node:fs"
import { homedir } from "node:os"
import { dirname, isAbsolute, join, relative, resolve, sep } from "node:path"
import { parseArgs, styleText } from "node:util"
import { cancel, confirm, intro, isCancel, log, multiselect, note, outro, password } from "@clack/prompts"
import { diffLines } from "diff"
import { applyEdits, modify, parse, printParseErrorCode, type FormattingOptions, type JSONPath, type ParseError } from "jsonc-parser"
import { parse as parseYaml } from "yaml"

const projectDirectory = dirname(import.meta.dir)
const banner = String.raw`  ____                   ______          __     ______            _____
  / __ \____  ___  ____  / ____/___  ____/ /__  / ____/___  ____  / __(_)___ _
 / / / / __ \/ _ \/ __ \/ /   / __ \/ __  / _ \/ /   / __ \/ __ \/ /_/ / __ ${"`"}/
/ /_/ / /_/ /  __/ / / / /___/ /_/ / /_/ /  __/ /___/ /_/ / / / / __/ / /_/ /
\____/ .___/\___/_/ /_/\____/\____/\__,_/\___/\____/\____/_/ /_/_/ /_/\__, /
    /_/                                                              /____/`

export type JsonObject = Record<string, unknown>
type Decision = (path: string, current: unknown, incoming: unknown) => Promise<boolean>
type SelectAdditions = (section: string, additions: ConfigAddition[]) => Promise<string[]>
export type Skill = { name: string; source: string }

interface ConfigAddition {
  id: string
  path: JSONPath
  label: string
  value: unknown
}

export type Fragments = Record<string, JsonObject>
export type MergeStage = "configuration" | "bash" | "sensitive-files" | "watcher" | "mcps"

const compareNames = (left: string, right: string) => left.localeCompare(right, "en", { sensitivity: "base" })

export function sortSkills(skills: Skill[]): Skill[] {
  return [...skills].sort((left, right) => compareNames(left.name, right.name))
}

export function sortSecretReferences(references: Iterable<string>): string[] {
  return [...references].sort((left, right) => compareNames(left.split(/[\\/]/).at(-1) ?? left, right.split(/[\\/]/).at(-1) ?? right))
}

type ProcessOptions = {
  cwd?: string
  env?: Record<string, string | undefined>
  output?: "capture" | "inherit" | "ignore"
  stdin?: "inherit" | "ignore"
}

async function runOpenCodeBun(args: string[], options: ProcessOptions = {}) {
  return runProcess([process.execPath, ...args], {
    ...options,
    env: { ...Bun.env, BUN_BE_BUN: "1", ...options.env }
  })
}

export async function runProcess(
  command: string[],
  { cwd, env, output = "inherit", stdin = "inherit" }: ProcessOptions = {}
): Promise<{ exitCode: number; stdout: string; stderr: string }> {
  const child = Bun.spawn(command, {
    cwd,
    env,
    stdin,
    stdout: output === "capture" ? "pipe" : output,
    stderr: output === "capture" ? "pipe" : output
  })
  const [stdout, stderr, exitCode] = await Promise.all([
    output === "capture" ? new Response(child.stdout).text() : "",
    output === "capture" ? new Response(child.stderr).text() : "",
    child.exited
  ])
  return { exitCode, stdout, stderr }
}

class CancelledError extends Error {
  constructor() {
    super("Operation cancelled.")
  }
}

function isObject(value: unknown): value is JsonObject {
  return value !== null && typeof value === "object" && !Array.isArray(value)
}

export function selectStageFragments(fragments: Fragments, stage: MergeStage): Fragments {
  const selected: Fragments = {}

  for (const [file, fragment] of Object.entries(fragments)) {
    const section: JsonObject = {}

    if (stage === "configuration") {
      const { $schema: _schema, permission, mcp: _mcp, watcher: _watcher, ...settings } = fragment
      Object.assign(section, settings)
      if (isObject(permission)) {
        const otherPermissions = Object.fromEntries(Object.entries(permission).filter(([name]) => name !== "bash" && name !== "read"))
        if (Object.keys(otherPermissions).length > 0) section.permission = otherPermissions
      }
    } else if (stage === "bash" || stage === "sensitive-files") {
      const permission = isObject(fragment.permission) ? fragment.permission : undefined
      const ruleName = stage === "bash" ? "bash" : "read"
      const rules = permission && isObject(permission[ruleName]) ? permission[ruleName] : undefined
      if (rules) section.permission = { [ruleName]: rules }
    } else if (stage === "watcher" && isObject(fragment.watcher)) {
      section.watcher = fragment.watcher
    } else if (stage === "mcps" && isObject(fragment.mcp)) {
      section.mcp = fragment.mcp
    }

    if (Object.keys(section).length > 0) selected[file] = section
  }

  return selected
}

export function fragmentFileNames(names: string[]): string[] {
  return names
    .filter(name => /^opencode.*\.jsonc$/i.test(name))
    .sort((left, right) => {
      if (left === "opencode.jsonc") return -1
      if (right === "opencode.jsonc") return 1
      return left.localeCompare(right)
    })
}

export function parseJsonc(text: string, source: string): JsonObject {
  const errors: ParseError[] = []
  const parsed = parse(text.startsWith("\uFEFF") ? text.slice(1) : text, errors, { allowTrailingComma: true })
  if (errors.length) {
    const details = errors.map(error => `${printParseErrorCode(error.error)} at offset ${error.offset}`).join(", ")
    throw new Error(`Invalid JSONC in ${source}: ${details}`)
  }
  if (!isObject(parsed)) throw new Error(`Expected a JSON object in ${source}.`)
  return parsed
}

function formattingFor(text: string): FormattingOptions {
  const whitespace = text.match(/\n([\t ]+)\S/)?.[1] ?? "  "
  return {
    insertSpaces: !whitespace.includes("\t"),
    tabSize: whitespace.includes("\t") ? 1 : whitespace.length,
    eol: text.includes("\r\n") ? "\r\n" : "\n"
  }
}

function equal(left: unknown, right: unknown): boolean {
  return JSON.stringify(left) === JSON.stringify(right)
}

function pathLabel(path: JSONPath): string {
  return path.reduce<string>((label, segment, index) => {
    if (typeof segment === "number") return `${label}[${segment}]`
    return index === 0 ? `.${segment}` : `${label}[${JSON.stringify(segment)}]`
  }, "")
}

class JsoncDocument {
  text: string
  data: JsonObject
  changed = false
  readonly bom: string
  readonly formatting: FormattingOptions

  constructor(text: string) {
    this.bom = text.startsWith("\uFEFF") ? "\uFEFF" : ""
    this.text = this.bom ? text.slice(1) : text
    this.data = parseJsonc(text, "central OpenCode configuration")
    this.formatting = formattingFor(this.text)
  }

  get(path: JSONPath): unknown {
    let value: unknown = this.data
    for (const part of path) {
      if (typeof part === "number") {
        if (!Array.isArray(value)) return undefined
        value = value[part]
      } else {
        if (!isObject(value) || !(part in value)) return undefined
        value = value[part]
      }
    }
    return value
  }

  set(path: JSONPath, value: unknown): void {
    const edits = modify(this.text, path, value, { formattingOptions: this.formatting })
    if (!edits.length) return
    this.text = applyEdits(this.text, edits)
    this.data = parseJsonc(this.text, "merged configuration")
    this.changed = true
  }
}

async function mergeObject(document: JsoncDocument, values: JsonObject, path: JSONPath, decide: Decision, selectAdditions: SelectAdditions): Promise<void> {
  const additions: ConfigAddition[] = []
  for (const [name, incoming] of Object.entries(values).sort(([left], [right]) => compareNames(left, right))) {
    const propertyPath = [...path, name]
    const current = document.get(propertyPath)
    if (current === undefined) {
      if (isObject(incoming)) await mergeObject(document, incoming, propertyPath, decide, selectAdditions)
      else {
        const itemPath = pathLabel(propertyPath)
        additions.push({
          id: itemPath,
          path: propertyPath,
          label: `${styleText(["bold", "yellow"], name)}: ${styleText("blueBright", formatInlineValue(itemPath, incoming))}`,
          value: incoming
        })
      }
    } else if (isObject(current) && isObject(incoming)) await mergeObject(document, incoming, propertyPath, decide, selectAdditions)
    else if (!equal(current, incoming) && (await decide(pathLabel(propertyPath), current, incoming))) {
      document.set(propertyPath, incoming)
    }
  }

  if (additions.length === 0) return
  const section = path.length === 0 ? "OpenCode settings" : pathLabel(path)
  const selected = new Set(await selectAdditions(section, additions))
  for (const addition of additions) {
    if (selected.has(addition.id)) document.set(addition.path, addition.value)
  }
}

async function selectAdditionsIndividually(additions: ConfigAddition[], decide: Decision): Promise<string[]> {
  const selected: string[] = []
  for (const addition of additions) {
    if (await decide(addition.id, undefined, addition.value)) selected.push(addition.id)
  }
  return selected
}

export async function mergeConfiguration(
  centralText: string,
  fragments: Fragments,
  decide: Decision,
  selectAdditions?: SelectAdditions
): Promise<{ text: string; data: JsonObject; changed: boolean }> {
  const document = new JsoncDocument(centralText)
  const select = selectAdditions ?? ((_, additions) => selectAdditionsIndividually(additions, decide))
  for (const fragment of Object.values(fragments)) {
    const { $schema: _schema, mcp, watcher, ...values } = fragment
    await mergeObject(document, values, [], decide, select)

    if (isObject(mcp)) {
      for (const [mcpName, incoming] of Object.entries(mcp).sort(([left], [right]) => compareNames(left, right))) {
        const current = document.get(["mcp", mcpName])
        if (!equal(current, incoming) && (await decide(`.mcp[${JSON.stringify(mcpName)}]`, current, incoming))) {
          document.set(["mcp", mcpName], incoming)
        }
      }
    }

    if (isObject(watcher) && Array.isArray(watcher.ignore)) {
      const current = document.get(["watcher", "ignore"])
      const merged = Array.isArray(current) ? [...current] : []
      const additions = [...new Set(watcher.ignore)]
        .filter(pattern => !merged.some(item => equal(item, pattern)))
        .sort(compareNames)
        .map(pattern => {
          const patternPath = ["watcher", "ignore", pattern]
          return {
            id: pathLabel(patternPath),
            path: patternPath,
            label: styleText("yellow", pattern),
            value: pattern
          }
        })
      if (additions.length > 0) {
        const selected = new Set(await select(pathLabel(["watcher", "ignore"]), additions))
        for (const addition of additions) {
          if (selected.has(addition.id)) merged.push(addition.value)
        }
        if (selected.size > 0) document.set(["watcher", "ignore"], merged)
      }
      const { ignore: _ignore, ...otherWatcherValues } = watcher
      await mergeObject(document, otherWatcherValues, ["watcher"], decide, select)
    } else if (isObject(watcher)) {
      await mergeObject(document, watcher, ["watcher"], decide, select)
    }
  }
  return { text: `${document.bom}${document.text}`, data: document.data, changed: document.changed }
}

async function askYesNo(message: string): Promise<boolean> {
  const answer = await confirm({ message, initialValue: false })
  if (isCancel(answer)) {
    cancel("Operation cancelled.")
    throw new CancelledError()
  }
  return answer
}

async function selectConfigAdditions(section: string, additions: ConfigAddition[]): Promise<string[]> {
  const message =
    section === '.permission["bash"]'
      ? "Choose which Bash rules to include"
      : section === '.permission["read"]'
        ? "Choose which Read rules to include"
        : section === '.watcher["ignore"]'
          ? "Choose which watcher patterns to include"
          : `Choose which settings to include under ${section}`
  const answer = await multiselect({
    message,
    options: additions.map(addition => ({ value: addition.id, label: addition.label })),
    initialValues: [],
    maxItems: 10,
    required: false
  })
  if (isCancel(answer)) {
    cancel("Operation cancelled.")
    throw new CancelledError()
  }
  return answer
}

async function askSecret(message: string): Promise<string> {
  const answer = await password({ message, mask: "*" })
  if (isCancel(answer)) {
    cancel("Operation cancelled.")
    throw new CancelledError()
  }
  return answer
}

const sensitiveKey = /(secret|token|password|credential|authorization|api.?key|private.?key)/i

function redactValue(value: unknown, key = ""): unknown {
  if (sensitiveKey.test(key)) return "<redacted>"
  if (Array.isArray(value)) return value.map(item => redactValue(item))
  if (isObject(value)) {
    return Object.fromEntries(
      Object.entries(value)
        .sort(([left], [right]) => left.localeCompare(right))
        .map(([name, item]) => [name, redactValue(item, name)])
    )
  }
  return value
}

function highlightJsonValue(value: string): string {
  return value.replace(/("(?:\\.|[^"\\])*"|-?\b\d+(?:\.\d+)?(?:[eE][+-]?\d+)?\b|\b(?:true|false|null)\b|[{}\[\],:])/g, token => {
    if (token.startsWith('"')) return styleText("green", token)
    if (token === "true") return styleText("cyan", token)
    if (token === "false" || token === "null") return styleText("magenta", token)
    if (/^-?\d/.test(token)) return styleText("magenta", token)
    return styleText("dim", token)
  })
}

function highlightJsonLine(line: string): string {
  const keyMatch = /^(\s*)((?:"(?:\\.|[^"\\])*"))(:)(.*)$/.exec(line)
  if (!keyMatch) return highlightJsonValue(line)
  const [, indent, key, colon, value] = keyMatch
  return `${indent}${styleText("yellowBright", key)}${styleText("dim", colon)}${highlightJsonValue(value)}`
}

function formatDiffValue(value: unknown): string {
  const redacted = redactValue(value)
  const serialized = JSON.stringify(redacted, null, 2) ?? ""
  if (!isObject(redacted)) return serialized
  const entries = Object.keys(redacted)
  if (entries.length === 0) return ""
  if (entries.length === 1)
    return serialized
      .split("\n")
      .slice(1, -1)
      .map(line => line.replace(/^  /, ""))
      .join("\n")
  return serialized
}

export function createConfigDiff(before: unknown, after: unknown, path?: string): string {
  const beforeValue = path ? (before === undefined ? {} : { [path]: before }) : before
  const afterValue = path ? { [path]: after } : after
  const beforeText = beforeValue === undefined ? "" : formatDiffValue(beforeValue)
  const afterText = formatDiffValue(afterValue)
  const rows: string[] = []

  for (const change of diffLines(beforeText, afterText)) {
    if (!change.added && !change.removed) continue
    const prefix = change.removed ? "- " : "+ "
    const color = change.removed ? "red" : "green"
    for (const line of change.value.split("\n")) {
      if (line) rows.push(`${styleText(color, prefix)}${highlightJsonLine(line)}`)
    }
  }

  return rows.join("\n")
}

function isInlineValue(value: unknown): boolean {
  return value === undefined || value === null || ["string", "number", "boolean"].includes(typeof value)
}

function formatInlineValue(path: string, value: unknown): string {
  if (value === undefined) return "<unset>"
  return JSON.stringify(redactValue(value, path)) ?? "null"
}

function formatInlinePath(path: string): { section: string; name: string } {
  const firstSegment = /^\.([^.[\]]+)/.exec(path)?.[1]
  const bracketSegments = [...path.matchAll(/\[("(?:\\.|[^"\\])*")\]/g)].map(match => JSON.parse(match[1]) as string)
  const segments = [firstSegment, ...bracketSegments].filter((segment): segment is string => segment !== undefined)
  if (segments[0] === "watcher" && segments.length > 2) return { section: "watcher pattern", name: segments.at(-1) ?? path }
  if (segments[0] === "permission" && segments.length > 1) return { section: "permission", name: segments.at(-1) ?? path }
  if (segments.length === 1) return { section: "setting", name: segments[0] }
  return { section: segments[0]?.replace(/[-_]/g, " ") ?? "setting", name: segments.at(-1) ?? path }
}

async function decideConflict(path: string, current: unknown, incoming: unknown): Promise<boolean> {
  const mcpName = path.startsWith(".mcp[") ? (JSON.parse(path.slice(5, -1)) as string) : undefined
  if (isInlineValue(current) && isInlineValue(incoming)) {
    const { section, name } = formatInlinePath(path)
    const highlightedName = styleText(["bold", "yellow"], name)
    const highlightedIncoming = styleText("blueBright", formatInlineValue(path, incoming))
    if (section === "watcher pattern" && current === undefined) {
      return await askYesNo(`Add watcher pattern ${highlightedName}?`)
    }
    if (current === undefined) {
      return await askYesNo(`Add ${section} ${highlightedName} as ${highlightedIncoming}?`)
    }
    return await askYesNo(`Change ${section} ${highlightedName} to ${highlightedIncoming}?`)
  }

  if (current === undefined) {
    note(
      mcpName ? createConfigDiff(undefined, incoming) : createSettingsPreview(incoming),
      styleText(["bold", "yellowBright"], mcpName ? `New MCP: ${mcpName}` : `New settings: ${path}`)
    )
    if (mcpName) return await askYesNo(`Add MCP '${styleText(["bold", "yellow"], mcpName)}'?`)
    const count = isObject(incoming) ? Object.keys(incoming).length : 1
    const noun = count === 1 ? "setting" : "settings"
    return await askYesNo(`Add ${count} ${noun} under ${styleText(["bold", "yellow"], path)}?`)
  }

  note(createConfigDiff(current, incoming, mcpName ? undefined : path), styleText(["bold", "yellowBright"], mcpName ? `MCP ${mcpName}` : `Config conflict: ${path}`))
  return await askYesNo(
    mcpName ? `Overwrite MCP '${styleText(["bold", "yellow"], mcpName)}' with the project version?` : `Use incoming value for ${styleText(["bold", "yellow"], path)}?`
  )
}

function showPreflight(central: JsonObject, fragments: Fragments, path: string): void {
  const objectCount = (value: unknown) => (isObject(value) ? Object.keys(value).length : 0)
  const arrayCount = (value: unknown) => (Array.isArray(value) ? value.length : 0)
  const permission = isObject(central.permission) ? central.permission : {}
  const watcher = isObject(central.watcher) ? central.watcher : {}
  const mcps = isObject(central.mcp) ? central.mcp : {}
  const project = Object.values(fragments)
  const rules = project.reduce(
    (counts, fragment) => {
      if (isObject(fragment.permission)) {
        counts.bash += objectCount(fragment.permission.bash)
        counts.read += objectCount(fragment.permission.read)
      }
      if (isObject(fragment.watcher)) counts.watcher += arrayCount(fragment.watcher.ignore)
      if (isObject(fragment.mcp)) counts.mcp += objectCount(fragment.mcp)
      return counts
    },
    { bash: 0, read: 0, watcher: 0, mcp: 0 }
  )
  note(
    `Central config: ${path}\n${Object.keys(fragments)
      .map(name => `${name.padEnd(30)} found`)
      .join("\n")}`,
    "Preflight"
  )
  note(
    [
      `Bash rules:       central ${String(objectCount(permission.bash)).padStart(3)} | project ${String(rules.bash).padStart(3)}`,
      `Read rules:       central ${String(objectCount(permission.read)).padStart(3)} | project ${String(rules.read).padStart(3)}`,
      `Watcher patterns: central ${String(arrayCount(watcher.ignore)).padStart(3)} | project ${String(rules.watcher).padStart(3)}`,
      `MCPs:             central ${String(objectCount(mcps)).padStart(3)} | project ${String(rules.mcp).padStart(3)}`
    ].join("\n"),
    "Sections"
  )
}

export function parseSkills(text: string): Skill[] {
  const data = parseYaml(text) as { skills?: unknown }
  if (!Array.isArray(data?.skills) || data.skills.length === 0) throw new Error("Skills configuration is empty or invalid.")
  return data.skills.map((skill: unknown) => {
    if (!isObject(skill) || typeof skill.name !== "string" || typeof skill.source !== "string" || !skill.name.trim() || !skill.source.trim()) {
      throw new Error("Skills configuration is empty or invalid.")
    }
    return { name: skill.name.trim(), source: skill.source.trim() }
  })
}

export function hasSkill(output: string, name: string): boolean {
  const plain = output.replace(/\x1b\[[0-9;]*m/g, "")
  const escaped = name.replace(/[.*+?^${}()|[\]\\]/g, "\\$&")
  return new RegExp(`^\\s*${escaped}\\s+`, "im").test(plain)
}

async function runSkills(args: string[]): Promise<string> {
  const { stdout, stderr, exitCode } = await runOpenCodeBun(["x", "--no-install", "skills", ...args], { output: "capture" })
  if (exitCode !== 0) throw new Error((stderr || stdout).trim() || `skills exited with code ${exitCode}.`)
  return stdout
}

function collectFileReferences(value: unknown, result = new Set<string>()): Set<string> {
  if (typeof value === "string") {
    for (const match of value.matchAll(/\{file:([^}]+)\}/g)) result.add(match[1])
  } else if (Array.isArray(value)) value.forEach(item => collectFileReferences(item, result))
  else if (isObject(value)) Object.values(value).forEach(item => collectFileReferences(item, result))
  return result
}

export function resolveSecretPath(directory: string, reference: string): string {
  const path = resolve(directory, reference.startsWith("./") ? reference.slice(2) : reference)
  const relativePath = relative(directory, path)
  if (!relativePath || relativePath === ".." || relativePath.startsWith(`..${sep}`) || isAbsolute(relativePath)) {
    throw new Error(`Secret reference escapes the OpenCode config directory: ${reference}`)
  }
  return path
}

export async function readFragments(directory: string): Promise<Fragments> {
  const directoryEntries = await readdir(directory, { withFileTypes: true })
  const names = fragmentFileNames(directoryEntries.filter(entry => entry.isFile()).map(entry => entry.name))
  if (names.length === 0) throw new Error(`No opencode*.jsonc fragments found in ${directory}.`)
  const entries = await Promise.all(
    names.map(async name => {
      const path = join(directory, name)
      return [name, parseJsonc(await readFile(path, "utf8"), path)] as const
    })
  )
  return Object.fromEntries(entries)
}

export function readRef(arguments_: string[]): string | undefined {
  const { values } = parseArgs({
    args: arguments_,
    options: { ref: { type: "string" } },
    strict: true,
    allowPositionals: false
  })
  const ref = values.ref
  if (ref && (ref.startsWith("-") || ref.includes("..") || !/^[A-Za-z0-9._/-]+$/.test(ref))) {
    throw new Error(`Invalid Git ref: ${ref}`)
  }
  return ref
}

interface PendingSecret {
  path: string
  name: string
  value: string
}

async function collectSecrets(config: JsonObject, directory: string): Promise<PendingSecret[]> {
  if (!isObject(config.mcp)) return []
  const pending: PendingSecret[] = []
  for (const reference of sortSecretReferences(collectFileReferences(config.mcp))) {
    const path = resolveSecretPath(directory, reference)
    if (existsSync(path)) continue
    const name = path.split(/[\\/]/).at(-1) ?? reference
    const value = await askSecret(`Enter value for MCP secret '${name}' (leave empty to skip):`)
    if (!value.trim()) continue
    pending.push({ path, name, value })
  }
  return pending
}

async function saveSecrets(secrets: PendingSecret[]): Promise<void> {
  for (const secret of secrets) {
    await mkdir(dirname(secret.path), { recursive: true })
    try {
      const file = await open(secret.path, "wx", 0o600)
      await file.writeFile(secret.value, "utf8").finally(() => file.close())
      if (process.platform !== "win32") await chmod(secret.path, 0o600)
      log.success(`Created secret file: ${secret.name}`)
    } catch (error) {
      if ((error as NodeJS.ErrnoException).code !== "EEXIST") throw error
      log.info(`Secret file already exists: ${secret.name}`)
    }
  }
}

async function downloadProjectFiles(ref: string): Promise<void> {
  const listingUrl = new URL(`https://api.github.com/repos/KeesCBakker/keestalkstech-code-gallery/contents/15.opencode/config`)
  listingUrl.searchParams.set("ref", ref)
  listingUrl.searchParams.set("cachebust", crypto.randomUUID())
  const listing = await fetch(listingUrl, {
    headers: { Accept: "application/vnd.github+json", "Cache-Control": "no-cache" }
  })
  if (!listing.ok) throw new Error(`Could not list remote config files: HTTP ${listing.status}`)

  const files = (await listing.json()) as { name: string; download_url: string | null; type: string }[]
  for (const file of files) {
    if (file.type !== "file") continue
    if (!file.download_url) throw new Error(`No download URL for ${file.name}.`)
    log.step(`Downloading ${file.name}`)
    const downloadUrl = new URL(file.download_url)
    downloadUrl.searchParams.set("cachebust", crypto.randomUUID())
    const response = await fetch(downloadUrl, { headers: { "Cache-Control": "no-cache" } })
    if (!response.ok) throw new Error(`Could not download ${file.name}: HTTP ${response.status}`)

    const destination = join(projectDirectory, "config", file.name)
    await mkdir(dirname(destination), { recursive: true })
    await Bun.write(destination, response)
  }
}

function findCentralConfig(directory: string): string {
  const path = ["opencode.jsonc", "opencode.json"].map(name => join(directory, name)).find(existsSync)
  if (!path) throw new Error(`No central OpenCode config found in ${directory}.`)
  return path
}

async function validateConfig(path: string): Promise<void> {
  const env = { ...Bun.env, OPENCODE_CONFIG: path }
  delete env.BUN_BE_BUN
  const { exitCode, stderr, stdout } = await runProcess([process.execPath, "debug", "config"], {
    env,
    stdin: "ignore",
    output: "capture"
  })
  if (exitCode !== 0) {
    const details = (stderr || stdout).trim()
    throw new Error(details || "OpenCode rejected the merged configuration.")
  }
}

export async function formatConfig(path: string): Promise<void> {
  const { exitCode, stderr, stdout } = await runOpenCodeBun(
    ["x", "--no-install", "prettier", "--write", "--parser", "jsonc", "--config", join(projectDirectory, ".prettierrc"), path],
    { cwd: projectDirectory, output: "capture" }
  )
  if (exitCode !== 0) throw new Error((stderr || stdout).trim() || "Prettier could not format the merged config.")
}

async function installSkills(): Promise<void> {
  const skills = sortSkills(parseSkills(await readFile(join(projectDirectory, "config", "opencode-skills.yaml"), "utf8")))
  note(skills.map(skill => `${skill.name.padEnd(20)} ${skill.source}`).join("\n"), "Configured skills")
  for (const skill of skills) {
    if (hasSkill(await runSkills(["list", "--global", "--agent", "opencode"]), skill.name)) {
      log.success(`Already installed: ${skill.name}`)
      continue
    }
    if (!(await askYesNo(`Install '${skill.name}' globally for OpenCode?`))) {
      log.info(`Skipped: ${skill.name}`)
      continue
    }
    await runSkills(["add", skill.source, "--skill", skill.name, "--global", "--agent", "opencode", "--yes"])
    if (!hasSkill(await runSkills(["list", "--global", "--agent", "opencode"]), skill.name)) {
      throw new Error(`Skill '${skill.name}' was not listed after installation.`)
    }
    log.success(`Installed ${skill.name}`)
  }
}

export async function runSkillsStep(install: () => Promise<void>): Promise<void> {
  try {
    await install()
  } catch (error) {
    log.warn(`The skill check failed: ${error instanceof Error ? error.message : error}`)
  }
}

async function main(): Promise<void> {
  process.stdout.write(`${banner}\n\n`)
  intro("OPENCODE CONFIGURATOR")

  const ref = readRef(process.argv.slice(2))
  if (ref) await downloadProjectFiles(ref)

  const configDirectory = join(homedir(), ".config", "opencode")
  const opencodeExecutable = process.execPath
  const version = await runProcess([opencodeExecutable, "--version"], { output: "capture" })
  if (version.exitCode !== 0) throw new Error((version.stderr || version.stdout).trim() || "Could not run the OpenCode executable.")
  log.info(`OpenCode executable: ${opencodeExecutable} (${version.stdout.trim()})`)
  const centralPath = findCentralConfig(configDirectory)
  const centralText = await readFile(centralPath, "utf8")
  const fragments = await readFragments(join(projectDirectory, "config"))

  showPreflight(parseJsonc(centralText, centralPath), fragments, centralPath)
  let mergedText = centralText
  let mergedData = parseJsonc(centralText, centralPath)
  let configurationChanged = false
  const applyStage = async (index: number, title: string, stage: MergeStage): Promise<void> => {
    log.step(`Step ${index}/7: ${title}`)
    const result = await mergeConfiguration(mergedText, selectStageFragments(fragments, stage), decideConflict, selectConfigAdditions)
    mergedText = result.text
    mergedData = result.data
    configurationChanged ||= result.changed
  }

  await applyStage(1, "Config", "configuration")
  await applyStage(2, "Bash rules", "bash")
  await applyStage(3, "Sensitive files", "sensitive-files")
  await applyStage(4, "Watcher excludes", "watcher")
  await applyStage(5, "MCPs", "mcps")
  const result = { text: mergedText, data: mergedData, changed: configurationChanged }
  const pendingSecrets = await collectSecrets(result.data, configDirectory)

  log.step("Step 6/7: Update config")
  const changed = result.changed || pendingSecrets.length > 0
  await saveSecrets(pendingSecrets)
  if (!result.changed) {
    log.info("No configuration changes detected; the central config is unchanged.")
  } else {
    const timestamp = new Date()
      .toISOString()
      .replace(/[-:TZ]/g, "")
      .replace(".", "_")
    const backupPath = `${centralPath}.backup.${timestamp}`
    const temporaryPath = `${centralPath}.tmp.${crypto.randomUUID()}.jsonc`

    let backupCreated = false
    try {
      await writeFile(temporaryPath, result.text, "utf8")
      await formatConfig(temporaryPath)
      await validateConfig(temporaryPath)
      await copyFile(centralPath, backupPath)
      backupCreated = true
      log.success(`Backup created at ${backupPath}`)
      await rename(temporaryPath, centralPath)
      log.success(`Merged configuration written to ${centralPath}`)
    } catch (error) {
      log.warn(backupCreated ? `The central config was not changed. Backup remains at ${backupPath}` : "The central config was not changed; no backup was created.")
      throw error
    } finally {
      await rm(temporaryPath, { force: true })
    }
  }

  log.step("Step 7/7: Install skills")
  await runSkillsStep(installSkills)
  outro(changed ? "OpenCode configuration updated." : "OpenCode configuration is up to date.")
}

if (import.meta.main) {
  await main().catch(error => {
    if (error instanceof CancelledError) return
    log.error(error instanceof Error ? error.message : String(error))
    process.exitCode = 1
  })
}
