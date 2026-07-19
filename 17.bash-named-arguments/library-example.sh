#!/usr/bin/env bash

set -euo pipefail

script_dir=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" &>/dev/null && pwd)
source "$script_dir/lib/arguments.sh"

declare repo="" branch=""
declare -ar allowed_arguments=(repo branch)
program_version="0.0.1"

usage() {
  echo ""
  echo "Shows the repository and branch supplied as named arguments."
  echo ""
  echo "usage: $0 --repo string --branch string [--help] [--version]"
  echo ""
  echo "  --repo string    name of the repository"
  echo "  --branch string  branch to use"
  echo "  --version        show the version of the script"
  echo "  --help           show this help"
  echo ""
}

version() {
  echo "$program_version"
}

parse_input_args "$@"

ensure_required_input_arg "--repo" "$repo"
ensure_required_input_arg "--branch" "$branch"

printf "Repository: %s\n" "$repo"
printf "Branch: %s\n" "$branch"
