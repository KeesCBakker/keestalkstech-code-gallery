import { inspect } from "util"

export function pathConsoleLog(limit = 100, delay = 10): void {
  let count = 0
  let buffer = ""
  let timer: NodeJS.Timeout | null = null

  console.log = (...args: unknown[]) => {
    const line =
      args
        .map(a => (typeof a === "string" ? a : inspect(a, { colors: true })))
        .join(" ") + "\n"

    if (++count <= limit) {
      process.stdout.write(line)
      return
    }

    buffer += line

    if (!timer) {
      timer = setTimeout(() => {
        process.stdout.write(buffer)
        buffer = ""
        timer = null
      }, delay)

      timer.unref?.()
    }
  }
}
