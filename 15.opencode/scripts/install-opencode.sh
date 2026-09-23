#!/usr/bin/env bash
set -euo pipefail

curl -fsSL https://opencode.ai/install | bash

export PATH="$HOME/.opencode/bin:$HOME/bin:$PATH"
printf 'opencode: %s\n' "$(opencode --version)"
printf 'bun: %s\n' "$(bun --version)"
