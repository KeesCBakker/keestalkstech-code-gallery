#!/usr/bin/env bash
set -euo pipefail

if [[ "$(uname -s)" != "Linux" ]]; then
  printf 'This installer supports Linux. On Windows, run install-opencode.ps1 in PowerShell.\n' >&2
  exit 1
fi

if ! command -v curl >/dev/null 2>&1; then
  printf "curl is required. Install it with your distribution's package manager and try again.\n" >&2
  exit 1
fi

if ! command -v tar >/dev/null 2>&1; then
  printf "tar is required. Install it with your distribution's package manager and try again.\n" >&2
  exit 1
fi

printf 'Installing OpenCode using the official Linux installer...\n'
curl -fsSL https://opencode.ai/install | bash

# Load the standard user install locations in this process so we can verify
# the installation even when the installer updated a shell profile only.
export PATH="$HOME/.opencode/bin:$HOME/bin:$PATH"

if ! command -v opencode >/dev/null 2>&1; then
  printf 'OpenCode was installed but is not on PATH. Add ~/.opencode/bin to PATH, then run opencode --version.\n' >&2
  exit 1
fi

printf 'opencode: %s\n' "$(opencode --version)"
if command -v bun >/dev/null 2>&1; then
  printf 'bun: %s\n' "$(bun --version)"
else
  printf 'bun was not found on PATH. Install Bun before running the TypeScript configurator.\n' >&2
  exit 1
fi
