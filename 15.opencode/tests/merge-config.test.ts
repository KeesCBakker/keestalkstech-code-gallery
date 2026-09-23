import { describe, expect, test } from "bun:test"
import { mkdir, mkdtemp, readFile, rm, writeFile } from "node:fs/promises"
import { tmpdir } from "node:os"
import { join } from "node:path"
import {
  hasSkill,
  fragmentFileNames,
  formatConfig,
  mergeConfiguration,
  parseJsonc,
  parseSkills,
  readFragments,
  readRef,
  resolveSecretPath,
  runProcess,
  type Fragments
} from "../merge-config"

function fragments(overrides: Partial<Fragments> = {}): Fragments {
  return {
    "opencode.jsonc": {},
    "opencode-mcps.jsonc": {},
    "opencode-ask.jsonc": {},
    "opencode-config-files.jsonc": {},
    "opencode-watcher.jsonc": {},
    ...overrides
  }
}

const accept = async () => true

describe("mergeConfiguration", () => {
  test("selects all opencode JSONC files and ignores unrelated files", () => {
    expect(fragmentFileNames(["opencode.jsonc", "opencode-custom.jsonc", "other.jsonc", "opencode-skills.yaml"])).toEqual(["opencode.jsonc", "opencode-custom.jsonc"])
  })

  test("reads matching fragment files only", async () => {
    const directory = await mkdtemp(join(tmpdir(), "opencode-fragments-"))
    try {
      await writeFile(join(directory, "opencode-custom.jsonc"), '{\n  "share": "disabled"\n}\n')
      await writeFile(join(directory, "notes.jsonc"), "not a fragment")
      await mkdir(join(directory, "opencode-folder.jsonc"))

      const result = await readFragments(directory)
      expect(Object.keys(result)).toEqual(["opencode-custom.jsonc"])
      expect(result["opencode-custom.jsonc"].share).toBe("disabled")
    } finally {
      await rm(directory, { recursive: true, force: true })
    }
  })

  test("preserves comments while adding nested values", async () => {
    const central = `{
  // Keep this explanation.
  "permission": {
    "bash": {
      "git status*": "allow"
    }
  }
}
`
    const result = await mergeConfiguration(
      central,
      fragments({
        "opencode-ask.jsonc": {
          $schema: "https://opencode.ai/config.json",
          permission: { bash: { "git log*": "allow" } }
        }
      }),
      accept
    )

    expect(result.changed).toBe(true)
    expect(result.text).toContain("// Keep this explanation.")
    expect(result.data).toMatchObject({
      permission: { bash: { "git status*": "allow", "git log*": "allow" } }
    })
  })

  test("merges behavior from a newly named fragment", async () => {
    const result = await mergeConfiguration(
      '{ "share": "manual" }',
      {
        "opencode-custom.jsonc": { share: "disabled", permission: { edit: "ask" } }
      },
      accept
    )
    expect(result.data).toMatchObject({ share: "disabled", permission: { edit: "ask" } })
  })

  test("returns the original bytes for a no-op", async () => {
    const central = '{\r\n  // existing\r\n  "share": "disabled"\r\n}\r\n'
    const result = await mergeConfiguration(
      central,
      fragments({
        "opencode.jsonc": { $schema: "https://opencode.ai/config.json", share: "disabled" }
      }),
      accept
    )

    expect(result.changed).toBe(false)
    expect(result.text).toBe(central)
  })

  test("accepts and preserves a UTF-8 BOM", async () => {
    const central = '\uFEFF{\n  "share": "manual"\n}\n'
    const result = await mergeConfiguration(
      central,
      fragments({
        "opencode.jsonc": { share: "disabled" }
      }),
      accept
    )

    expect(result.text.startsWith("\uFEFF")).toBe(true)
    expect(result.data.share).toBe("disabled")
  })

  test("keeps the current value when a conflict is declined", async () => {
    const result = await mergeConfiguration(
      '{ "share": "manual" }',
      fragments({
        "opencode.jsonc": { share: "disabled" }
      }),
      async () => false
    )

    expect(result.changed).toBe(false)
    expect(result.data.share).toBe("manual")
  })

  test("adds and replaces complete MCP definitions after approval", async () => {
    const decisions: string[] = []
    const result = await mergeConfiguration(
      '{ "mcp": { "old": { "type": "remote", "url": "https://old.example" } } }',
      fragments({
        "opencode-mcps.jsonc": {
          mcp: {
            old: { type: "remote", url: "https://new.example", enabled: false },
            new: { type: "local", command: ["bun", "x", "example"] }
          }
        }
      }),
      async path => {
        decisions.push(path)
        return true
      }
    )

    expect(decisions).toEqual(['.mcp["old"]', '.mcp["new"]'])
    expect(result.data.mcp).toEqual({
      old: { type: "remote", url: "https://new.example", enabled: false },
      new: { type: "local", command: ["bun", "x", "example"] }
    })
  })

  test("merges watcher patterns without duplicates", async () => {
    const result = await mergeConfiguration(
      '{ "watcher": { "ignore": ["node_modules/**", "dist/**"] } }',
      fragments({
        "opencode-watcher.jsonc": { watcher: { ignore: ["dist/**", ".git/**"] } }
      }),
      accept
    )

    expect((result.data.watcher as { ignore: string[] }).ignore).toEqual(["node_modules/**", "dist/**", ".git/**"])
  })

  test("creates missing MCP and watcher parents", async () => {
    const result = await mergeConfiguration(
      "{}\n",
      fragments({
        "opencode-mcps.jsonc": {
          mcp: { context7: { type: "remote", url: "https://mcp.example" } }
        },
        "opencode-watcher.jsonc": { watcher: { ignore: ["node_modules/**"] } }
      }),
      accept
    )

    expect(result.data).toMatchObject({
      mcp: { context7: { type: "remote", url: "https://mcp.example" } },
      watcher: { ignore: ["node_modules/**"] }
    })
  })

  test("rejects invalid JSONC", () => {
    expect(() => parseJsonc('{ "share": }', "broken.jsonc")).toThrow("Invalid JSONC")
  })
})

describe("secret paths", () => {
  test("keeps secret files inside the config directory", () => {
    const configDirectory = process.platform === "win32" ? "C:\\Users\\Kees\\.config\\opencode" : "/home/kees/.config/opencode"
    expect(resolveSecretPath(configDirectory, "./secrets/api-key")).toBe(
      process.platform === "win32" ? "C:\\Users\\Kees\\.config\\opencode\\secrets\\api-key" : "/home/kees/.config/opencode/secrets/api-key"
    )
    expect(() => resolveSecretPath(configDirectory, "../api-key")).toThrow("escapes")
    if (process.platform === "win32") {
      expect(() => resolveSecretPath(configDirectory, "D:\\secrets\\api-key")).toThrow("escapes")
    }
  })
})

describe("skills configuration", () => {
  test("parses the configured YAML skill list", () => {
    expect(
      parseSkills(`skills:
  - name: find-skills
    source: https://github.com/vercel-labs/skills
`)
    ).toEqual([{ name: "find-skills", source: "https://github.com/vercel-labs/skills" }])
  })

  test("rejects incomplete entries", () => {
    expect(() => parseSkills("skills:\n  - name: missing-source\n")).toThrow("empty or invalid")
  })

  test("detects names in ANSI-colored output", () => {
    expect(hasSkill("  \x1b[32mfind-skills\x1b[0m  ~/.config/opencode\n", "find-skills")).toBe(true)
    expect(hasSkill("  skill-creator  ~/.config/opencode\n", "find-skills")).toBe(false)
  })
})

describe("launcher arguments", () => {
  test("uses local files unless a remote ref is provided", () => {
    expect(readRef([])).toBeUndefined()
    expect(readRef(["--ref", "feature/bun-launcher"])).toBe("feature/bun-launcher")
  })

  test("rejects unknown arguments and unsafe refs", () => {
    expect(() => readRef(["--unknown"])).toThrow()
    expect(() => readRef(["--ref", "../main"])).toThrow("Invalid Git ref")
    expect(() => readRef(["--ref", "--upload-pack=bad"])).toThrow()
  })
})

describe("runProcess", () => {
  test("captures output and exit code consistently", async () => {
    const result = await runProcess([process.execPath, "-e", 'console.log("out"); console.error("err"); process.exitCode = 7'], { output: "capture" })
    expect(result).toEqual({ exitCode: 7, stdout: "out\n", stderr: "err\n" })
  })
})

describe("Prettier JSONC formatting", () => {
  test("formats a temporary config and preserves comments and values", async () => {
    const directory = await mkdtemp(join(tmpdir(), "opencode-prettier-"))
    const path = join(directory, "opencode.jsonc")
    try {
      await writeFile(path, '\uFEFF{"share":"disabled",// keep me\n"watcher":{"ignore":["dist/**"]}}')
      await formatConfig(path)
      const formatted = await readFile(path, "utf8")
      expect(formatted.startsWith("\uFEFF")).toBe(true)
      expect(formatted).toContain("// keep me")
      expect(parseJsonc(formatted, path)).toEqual({ share: "disabled", watcher: { ignore: ["dist/**"] } })
    } finally {
      await rm(directory, { recursive: true, force: true })
    }
  })
})

describe("OpenCode config integration", () => {
  test("merged and formatted fragments pass opencode config validation", async () => {
    if (!Bun.which("opencode")) return

    const directory = await mkdtemp(join(tmpdir(), "opencode-config-validation-"))
    const path = join(directory, "opencode.jsonc")
    try {
      const current = '{\n  "$schema": "https://opencode.ai/config.json",\n  "share": "disabled"\n}\n'
      const fragments = await readFragments(join(import.meta.dir, "..", "config"))
      const merged = await mergeConfiguration(current, fragments, async () => true)
      await writeFile(path, merged.text, "utf8")
      await mkdir(join(directory, "secrets"), { recursive: true })
      for (const name of ["context7-api-key", "slack-client-id", "slack-client-secret"]) {
        await writeFile(join(directory, "secrets", name), "test-value", "utf8")
      }
      await formatConfig(path)

      const validation = await runProcess(["opencode", "debug", "config"], {
        env: { ...Bun.env, OPENCODE_CONFIG: path },
        stdin: "ignore",
        output: "capture"
      })
      expect(validation.exitCode, validation.stderr || validation.stdout).toBe(0)
    } finally {
      await rm(directory, { recursive: true, force: true })
    }
  })
})

describe("launcher terminal input", () => {
  test("rejects piped stdin before downloading or prompting", async () => {
    const child = Bun.spawn(["bun", "run", join(import.meta.dir, "..", "run-merge.ts")], {
      stdin: "ignore",
      stdout: "pipe",
      stderr: "pipe"
    })
    const [exitCode, stderr] = await Promise.all([child.exited, new Response(child.stderr).text()])
    expect(exitCode).toBe(1)
    expect(stderr).toContain("Interactive prompts need a terminal")
  })
})
