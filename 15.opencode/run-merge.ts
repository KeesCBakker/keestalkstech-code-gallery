import { mkdtemp, rm } from "node:fs/promises"
import { closeSync, openSync } from "node:fs"
import { tmpdir } from "node:os"
import { join } from "node:path"
import { parseArgs } from "node:util"

const repository = "KeesCBakker/keestalkstech-code-gallery"
const sourceDirectory = "15.opencode"
const files = ["package.json", "bun.lock", ".prettierrc", "merge-config.ts"]

function openTerminalInput(): number | "inherit" {
  if (process.stdin.isTTY) return "inherit"

  try {
    return openSync(process.platform === "win32" ? "CONIN$" : "/dev/tty", "r")
  } catch {
    throw new Error("An interactive terminal is required to answer the configuration prompts.")
  }
}

function readRef(arguments_: string[]): string {
  const { values } = parseArgs({
    args: arguments_,
    options: { ref: { type: "string", default: "main" } },
    strict: true,
    allowPositionals: false
  })
  const ref = values.ref
  if (!ref || ref.startsWith("-") || ref.includes("..") || !/^[A-Za-z0-9._/-]+$/.test(ref)) {
    throw new Error(`Invalid Git ref: ${ref ?? ""}`)
  }
  return ref
}

export async function main(): Promise<void> {
  const ref = readRef(process.argv.slice(2))
  const temporaryDirectory = await mkdtemp(join(tmpdir(), "opencode-merge-"))
  try {
    for (const file of files) {
      const url = `https://raw.githubusercontent.com/${repository}/${ref}/${sourceDirectory}/${file}`
      const response = await fetch(url)
      if (!response.ok) throw new Error(`Could not download ${file}: HTTP ${response.status}`)
      await Bun.write(join(temporaryDirectory, file), response)
    }

    const install = Bun.spawn(["bun", "install", "--frozen-lockfile", "--production"], {
      cwd: temporaryDirectory,
      stdin: "inherit",
      stdout: "inherit",
      stderr: "inherit"
    })
    if ((await install.exited) !== 0) throw new Error("Could not install the pinned merge dependencies.")

    const terminalInput = openTerminalInput()
    try {
      const merge = Bun.spawn(["bun", "run", "merge-config.ts", "--ref", ref], {
        cwd: temporaryDirectory,
        stdin: terminalInput,
        stdout: "inherit",
        stderr: "inherit"
      })
      if ((await merge.exited) !== 0) throw new Error("The downloaded merge program failed.")
    } finally {
      if (typeof terminalInput === "number") closeSync(terminalInput)
    }
  } finally {
    await rm(temporaryDirectory, { recursive: true, force: true })
  }
}

if (import.meta.main) {
  await main().catch(error => {
    console.error(error instanceof Error ? error.message : error)
    process.exitCode = 1
  })
}
