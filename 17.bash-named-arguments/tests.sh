#!/usr/bin/env bash

set -u

script_dir=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" &>/dev/null && pwd)
passed=0
failed=0

check() {
  local name=$1
  local expected_status=$2
  local expected_text=$3
  shift 3

  local output
  local actual_status
  output=$("$@" 2>&1)
  actual_status=$?

  if [[ $actual_status -eq $expected_status && $output == *"$expected_text"* ]]; then
    printf "PASS: %s\n" "$name"
    ((passed += 1))
  else
    printf "FAIL: %s\n" "$name"
    printf "  expected status: %s\n" "$expected_status"
    printf "  actual status: %s\n" "$actual_status"
    printf "  expected text: %s\n" "$expected_text"
    printf "  output: %s\n" "$output"
    ((failed += 1))
  fi
}

library_cli() (
  set -euo pipefail
  source "$script_dir/lib/arguments.sh"

  declare repo="" branch=""
  declare -ar allowed_arguments=(repo branch)
  program_version="0.0.1"

  usage() {
    printf "usage: library_cli --repo value --branch value\n"
  }

  version() {
    echo "$program_version"
  }

  parse_input_args "$@"
  ensure_required_input_arg "--repo" "$repo"
  ensure_required_input_arg "--branch" "$branch"

  printf "Repository: %s\n" "$repo"
  printf "Branch: %s\n" "$branch"
)

default_cli() (
  set -euo pipefail
  source "$script_dir/lib/arguments.sh"

  declare env="" tag="nothing"
  declare -ar allowed_arguments=(env tag)

  parse_input_args "$@"

  printf "Env: %s\n" "$env"
  printf "Tag: %s\n" "$tag"
)

whitelisted_but_undeclared() (
  set -euo pipefail
  source "$script_dir/lib/arguments.sh"

  declare -ar allowed_arguments=(missing)
  parse_input_args "$@"
)

declared_but_not_whitelisted() (
  set -euo pipefail
  source "$script_dir/lib/arguments.sh"

  declare repo=""
  declare -ar allowed_arguments=(repo)
  parse_input_args "$@"
)

readonly_target() (
  set -euo pipefail
  source "$script_dir/lib/arguments.sh"

  declare -r repo="original"
  declare -ar allowed_arguments=(repo)
  parse_input_args "$@"
)

source_only() (
  set -euo pipefail
  source "$script_dir/lib/arguments.sh"
  printf "sourced\n"
)

check "library is sourceable" 0 "sourced" source_only
check "valid named arguments" 0 "Repository: KeesCBakker/example" library_cli --repo KeesCBakker/example --branch main
check "value with spaces" 0 "Branch: feature/named arguments" library_cli --repo KeesCBakker/example --branch "feature/named arguments"
check "single-dash value" 0 "Branch: -candidate" library_cli --repo KeesCBakker/example --branch -candidate
check "repeated argument uses last value" 0 "Branch: second" library_cli --repo KeesCBakker/example --branch first --branch second
check "shell-sensitive value remains literal" 0 'Branch: $HOME * %s $(echo unsafe)' library_cli --repo KeesCBakker/example --branch '$HOME * %s $(echo unsafe)'
check "defaults" 0 "Tag: nothing" default_cli
check "long help" 0 "usage:" library_cli --help
check "short help" 0 "usage:" library_cli -h
check "help takes precedence" 0 "usage:" library_cli --help --unknown value
check "long version" 0 "0.0.1" library_cli --version
check "short version" 0 "0.0.1" library_cli -v
check "missing final value" 1 "Missing value for --repo" library_cli --repo
check "next option is not a value" 1 "Missing value for --repo" library_cli --repo --branch main
check "double-dash value is rejected" 1 "Missing value for --branch" library_cli --repo KeesCBakker/example --branch --candidate
check "explicit empty required value" 1 "Missing parameter --branch" library_cli --repo KeesCBakker/example --branch ""
check "unknown option" 1 "Unknown parameter: --unknown" library_cli --unknown value
check "positional argument" 1 "Unexpected positional argument: value" library_cli value
check "missing required argument" 1 "Missing parameter --branch" library_cli --repo KeesCBakker/example
check "whitelisted variable must be declared" 1 "Unknown parameter: --missing" whitelisted_but_undeclared --missing value
check "declared variable must be whitelisted" 1 "Unknown parameter: --PATH" declared_but_not_whitelisted --PATH changed
check "version variable is not an argument" 1 "Unknown parameter: --program_version" library_cli --program_version changed --version
check "readonly target cannot be changed" 1 "readonly variable" readonly_target --repo changed

printf "\nPassed: %d\nFailed: %d\n" "$passed" "$failed"
((failed == 0))
