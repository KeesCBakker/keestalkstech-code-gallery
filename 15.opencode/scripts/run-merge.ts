import { mkdir, mkdtemp, rm } from "node:fs/promises"
import { tmpdir } from "node:os"
import { dirname, join } from "node:path"
import { parseArgs } from "node:util"

const repository = "KeesCBakker/keestalkstech-code-gallery"
const files = ["package.json", "bun.lock", ".prettierrc", "src/merge-config.ts"]

function readRef(): string {
  const { values } = parseArgs({
    args: process.argv.slice(2),
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

async function download(ref: string, directory: string, file: string): Promise<void> {
  const url = new URL(`https://raw.githubusercontent.com/${repository}/${ref}/15.opencode/${file}`)
  url.searchParams.set("cachebust", crypto.randomUUID())
  const response = await fetch(url, { headers: { "Cache-Control": "no-cache" } })
  if (!response.ok) throw new Error(`Could not download ${file}: HTTP ${response.status}`)
  const destination = join(directory, file)
  await mkdir(dirname(destination), { recursive: true })
  await Bun.write(destination, response)
}

async function main(): Promise<void> {
  const ref = readRef()
  const directory = await mkdtemp(join(tmpdir(), "opencode-bootstrap-"))
  try {
    for (const file of files) await download(ref, directory, file)

    const opencode = Bun.which("opencode")
    if (!opencode) throw new Error("Could not find the OpenCode executable on PATH.")
    const env = { ...Bun.env, BUN_BE_BUN: "1" }
    const install = Bun.spawn([opencode, "install", "--frozen-lockfile", "--production"], {
      cwd: directory,
      env,
      stdin: "inherit",
      stdout: "inherit",
      stderr: "inherit"
    })
    if ((await install.exited) !== 0) throw new Error("Could not install the pinned merge dependencies.")

    const merge = Bun.spawn([opencode, "run", "merge", "--ref", ref], {
      cwd: directory,
      env,
      stdin: "inherit",
      stdout: "inherit",
      stderr: "inherit"
    })
    if ((await merge.exited) !== 0) throw new Error("The merge program failed.")
  } finally {
    await rm(directory, { recursive: true, force: true })
  }
}

await main().catch(error => {
  console.error(error instanceof Error ? error.message : error)
  process.exitCode = 1
})
