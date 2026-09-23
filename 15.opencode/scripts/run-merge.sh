#!/usr/bin/env bash
set -euo pipefail

ref=${1:-main}
if (( $# > 1 )) || [[ "$ref" == -* || "$ref" == *..* || ! "$ref" =~ ^[a-zA-Z0-9._/-]+$ ]]; then
  printf 'Invalid Git ref: %s\n' "$ref" >&2
  exit 1
fi

if [[ ! -r /dev/tty ]]; then
  printf 'An interactive terminal is required to answer the configuration prompts.\n' >&2
  exit 1
fi

directory=$(mktemp -d)
trap 'rm -rf -- "$directory"' EXIT

files=(package.json bun.lock .prettierrc src/merge-config.ts)
for file in "${files[@]}"; do
  mkdir -p "$directory/$(dirname "$file")"
  curl -fsSL "https://raw.githubusercontent.com/KeesCBakker/keestalkstech-code-gallery/$ref/15.opencode/$file" \
    -o "$directory/$file"
done

cd "$directory"
bun install --frozen-lockfile --production
bun run src/merge-config.ts --ref "$ref" </dev/tty
