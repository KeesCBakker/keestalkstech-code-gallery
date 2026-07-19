function_exists() {
  declare -F "$1" >/dev/null
}

die() {
  printf "Script failed: %s\n" "$1" >&2
  exit 1
}

is_allowed_argument() {
  local candidate=$1
  local allowed

  for allowed in "${allowed_arguments[@]}"; do
    if [[ $candidate == "$allowed" ]]; then
      declare -p -- "$candidate" &>/dev/null
      return
    fi
  done

  return 1
}

parse_input_args() {
  while [ $# -gt 0 ]; do
    if [[ $1 == "--help" || $1 == "-h" ]]; then
      function_exists usage || die "Function usage is not defined"
      usage
      exit 0
    elif [[ $1 == "--version" || $1 == "-v" ]]; then
      function_exists version || die "Function version is not defined"
      version
      exit 0
    elif [[ $1 == "--"* ]]; then
      if ! is_allowed_argument "${1#--}"; then
        die "Unknown parameter: $1"
      fi

      if (( $# < 2 )); then
        die "Missing value for $1"
      fi

      if [[ $2 == "--"* ]]; then
        die "Missing value for $1"
      fi

      printf -v "${1#--}" "%s" "$2"
      shift 2
    else
      die "Unexpected positional argument: $1"
    fi
  done
}

ensure_required_input_arg() {
  local name=$1
  local value=$2

  if [[ -z $value ]]; then
    function_exists usage && usage
    die "Missing parameter $name"
  fi
}
