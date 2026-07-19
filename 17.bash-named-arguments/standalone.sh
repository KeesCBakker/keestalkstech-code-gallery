#!/usr/bin/env bash

set -euo pipefail

declare service_name="" tag="" namespace="" env=""
declare -ar allowed_arguments=(service_name tag namespace env)

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

usage() {
  echo ""
  echo "Deploys an image using the supplied service, tag, namespace, and environment."
  echo ""
  echo "usage: $0 --service_name string --tag string --namespace string --env string"
  echo ""
  echo "  --service_name string   name of the service"
  echo "  --tag string            tag of the image to deploy"
  echo "  --namespace string      namespace of the cluster"
  echo "  --env string            environment to deploy to"
  echo "  --help                  show this help"
  echo ""
}

die() {
  printf "Script failed: %s\n" "$1" >&2
  exit 1
}

while [ $# -gt 0 ]; do
  if [[ $1 == "--help" ]]; then
    usage
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

if [[ -z $service_name ]]; then
  usage
  die "Missing parameter --service_name"
elif [[ -z $tag ]]; then
  usage
  die "Missing parameter --tag"
elif [[ -z $namespace ]]; then
  usage
  die "Missing parameter --namespace"
elif [[ -z $env ]]; then
  usage
  die "Missing parameter --env"
fi

printf "Deploying %s:%s to %s/%s\n" \
  "$service_name" "$tag" "$env" "$namespace"
