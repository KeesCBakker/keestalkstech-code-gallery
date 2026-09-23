import { chmod, copyFile, mkdir, open, readFile, readdir, rename, rm, writeFile } from "node:fs/promises"
import { existsSync } from "node:fs"
import { homedir } from "node:os"
import { dirname, isAbsolute, join, relative, resolve, sep } from "node:path"
import { parseArgs } from "node:util"
import { cancel, confirm, intro, isCancel, log, note, outro, password } from "@clack/prompts"
import { applyEdits, modify, parse, printParseErrorCode, type FormattingOptions, type JSONPath, type ParseError } from "jsonc-parser"
import { parse as parseYaml } from "yaml"

export type JsonObject = Record<string, unknown>
type Decision = (path: string, current: unknown, incoming: unknown) => Promise<boolean>
export type Skill = { name: string; source: string }

export type Fragments = Record<string, JsonObject>

type ProcessOptions = {
  cwd?: string
  env?: Record<string, string | undefined>
  output?: "capture" | "inherit" | "ignore"
  stdin?: "inherit" | "ignore"
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

async function mergeObject(document: JsoncDocument, values: JsonObject, path: JSONPath, decide: Decision): Promise<void> {
  for (const [name, incoming] of Object.entries(values)) {
    const propertyPath = [...path, name]
    const current = document.get(propertyPath)
    if (current === undefined) document.set(propertyPath, incoming)
    else if (isObject(current) && isObject(incoming)) await mergeObject(document, incoming, propertyPath, decide)
    else if (!equal(current, incoming) && (await decide(pathLabel(propertyPath), current, incoming))) {
      document.set(propertyPath, incoming)
    }
  }
}

export async function mergeConfiguration(centralText: string, fragments: Fragments, decide: Decision): Promise<{ text: string; data: JsonObject; changed: boolean }> {
  const document = new JsoncDocument(centralText)
  for (const fragment of Object.values(fragments)) {
    const { $schema: _schema, mcp, watcher, ...values } = fragment
    await mergeObject(document, values, [], decide)

    if (isObject(mcp)) {
      for (const [mcpName, incoming] of Object.entries(mcp)) {
        const current = document.get(["mcp", mcpName])
        if (!equal(current, incoming) && (await decide(`.mcp[${JSON.stringify(mcpName)}]`, current, incoming))) {
          document.set(["mcp", mcpName], incoming)
        }
      }
    }

    if (isObject(watcher) && Array.isArray(watcher.ignore)) {
      const current = document.get(["watcher", "ignore"])
      const merged = Array.isArray(current) ? [...current] : []
      for (const pattern of watcher.ignore) {
        if (!merged.some(item => equal(item, pattern))) merged.push(pattern)
      }
      if (!equal(current, merged)) document.set(["watcher", "ignore"], merged)
      const { ignore: _ignore, ...otherWatcherValues } = watcher
      await mergeObject(document, otherWatcherValues, ["watcher"], decide)
    } else if (isObject(watcher)) {
      await mergeObject(document, watcher, ["watcher"], decide)
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

async function askSecret(message: string): Promise<string> {
  const answer = await password({ message, mask: "*" })
  if (isCancel(answer)) {
    cancel("Operation cancelled.")
    throw new CancelledError()
  }
  return answer
}

function safeValue(name: string, value: unknown): string {
  if (/(secret|token|password|api.?key|private.?key|credential)/i.test(name)) return "<sensitive>"
  if (Array.isArray(value)) return `[${value.map(item => safeValue(name, item)).join(", ")}]`
  if (isObject(value)) {
    return `{ ${Object.entries(value)
      .sort(([left], [right]) => left.localeCompare(right))
      .map(([key, item]) => `${key}=${safeValue(key, item)}`)
      .join(", ")} }`
  }
  return value === null ? "null" : String(value)
}

async function decideConflict(path: string, current: unknown, incoming: unknown): Promise<boolean> {
  const mcpName = path.startsWith(".mcp[") ? (JSON.parse(path.slice(5, -1)) as string) : undefined
  if (mcpName && current === undefined) return await askYesNo(`MCP '${mcpName}' is not configured. Add it?`)
  note(`Current:  ${safeValue(path, current)}\nIncoming: ${safeValue(path, incoming)}`, `Conflict: ${path}`)
  return await askYesNo(mcpName ? `Overwrite MCP '${mcpName}' with the project version?` : "Use incoming value?")
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
  const { stdout, stderr, exitCode } = await runProcess(["bun", "x", "--no-install", "skills", ...args], {
    output: "capture"
  })
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

async function saveSecrets(config: JsonObject, directory: string): Promise<void> {
  if (!isObject(config.mcp)) return
  for (const reference of collectFileReferences(config.mcp)) {
    const path = resolveSecretPath(directory, reference)
    if (existsSync(path)) continue
    const name = path.split(/[\\/]/).at(-1) ?? reference
    const value = await askSecret(`Enter value for MCP secret '${name}' (leave empty to skip):`)
    if (!value.trim()) continue
    await mkdir(dirname(path), { recursive: true })
    try {
      const file = await open(path, "wx", 0o600)
      await file.writeFile(value, "utf8").finally(() => file.close())
      if (process.platform !== "win32") await chmod(path, 0o600)
      log.success(`Created secret file: ${name}`)
    } catch (error) {
      if ((error as NodeJS.ErrnoException).code !== "EEXIST") throw error
      log.info(`Secret file already exists: ${name}`)
    }
  }
}

async function downloadProjectFiles(ref: string): Promise<void> {
  const listing = await fetch(`https://api.github.com/repos/KeesCBakker/keestalkstech-code-gallery/contents/15.opencode/config?ref=${encodeURIComponent(ref)}`, {
    headers: { Accept: "application/vnd.github+json" }
  })
  if (!listing.ok) throw new Error(`Could not list remote config files: HTTP ${listing.status}`)

  const files = (await listing.json()) as { name: string; download_url: string | null; type: string }[]
  for (const file of files) {
    if (file.type !== "file") continue
    if (!file.download_url) throw new Error(`No download URL for ${file.name}.`)
    log.step(`Downloading ${file.name}`)
    const response = await fetch(file.download_url)
    if (!response.ok) throw new Error(`Could not download ${file.name}: HTTP ${response.status}`)

    const destination = join(import.meta.dir, "config", file.name)
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
  const { exitCode } = await runProcess(["opencode", "debug", "config"], {
    env: { ...Bun.env, OPENCODE_CONFIG: path },
    stdin: "ignore",
    output: "ignore"
  })
  if (exitCode !== 0) throw new Error("OpenCode rejected the merged configuration.")
}

export async function formatConfig(path: string): Promise<void> {
  const { exitCode, stderr, stdout } = await runProcess(
    ["bun", "x", "--no-install", "prettier", "--write", "--parser", "jsonc", "--config", join(import.meta.dir, ".prettierrc"), path],
    { cwd: import.meta.dir, output: "capture" }
  )
  if (exitCode !== 0) throw new Error((stderr || stdout).trim() || "Prettier could not format the merged config.")
}

async function installSkills(): Promise<void> {
  const skills = parseSkills(await readFile(join(import.meta.dir, "config", "opencode-skills.yaml"), "utf8"))
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

async function main(): Promise<void> {
  intro("OpenCode configuration merge")

  const ref = readRef(process.argv.slice(2))
  if (ref) await downloadProjectFiles(ref)

  const configDirectory = join(homedir(), ".config", "opencode")
  const centralPath = findCentralConfig(configDirectory)
  const centralText = await readFile(centralPath, "utf8")
  const fragments = await readFragments(join(import.meta.dir, "config"))

  showPreflight(parseJsonc(centralText, centralPath), fragments, centralPath)
  const result = await mergeConfiguration(centralText, fragments, decideConflict)
  await saveSecrets(result.data, configDirectory)

  if (!result.changed) {
    outro("No configuration changes detected; the central config is unchanged.")
    return
  }

  const timestamp = new Date()
    .toISOString()
    .replace(/[-:TZ]/g, "")
    .replace(".", "_")
  const backupPath = `${centralPath}.backup.${timestamp}`
  const temporaryPath = `${centralPath}.tmp.${crypto.randomUUID()}.jsonc`

  await copyFile(centralPath, backupPath)
  log.success(`Backup created at ${backupPath}`)

  try {
    await writeFile(temporaryPath, result.text, "utf8")
    await formatConfig(temporaryPath)
    await validateConfig(temporaryPath)
    await rename(temporaryPath, centralPath)
    log.success(`Merged configuration written to ${centralPath}`)
  } catch (error) {
    log.warn(`The central config was not changed. Backup remains at ${backupPath}`)
    throw error
  } finally {
    await rm(temporaryPath, { force: true })
  }

  try {
    await installSkills()
  } catch (error) {
    log.warn(`Configuration was updated, but the skill check failed: ${error instanceof Error ? error.message : error}`)
  }
  outro("OpenCode configuration updated.")
}

if (import.meta.main) {
  await main().catch(error => {
    if (error instanceof CancelledError) return
    log.error(error instanceof Error ? error.message : String(error))
    process.exitCode = 1
  })
}
