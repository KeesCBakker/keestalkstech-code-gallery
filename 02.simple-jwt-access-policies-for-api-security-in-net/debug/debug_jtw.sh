#!/bin/bash
set -e

# Decoding a JWT token, based on the discussion here:
# https://gist.github.com/thomasdarimont/46358bc8167fce059d83a1ebdb92b0e7
# More here:
# https://keestalkstech.com/2024/12/debug-a-jwt-token-expiration-locally-with-bash-or-powershell/

function jwt_decode(){
    jq -R 'split(".") | .[1] | @base64d | fromjson' <<< "$1"
}

function check_expiration {
  local payload
  payload=$(jwt_decode "$1")

  if [ -z "$payload" ]; then
    echo "Invalid JWT or unable to decode."
    return 1
  fi

  local exp
  exp=$(echo "$payload" | jq -r '.exp')

  if [ "$exp" == "null" ]; then
    echo "No expiration ('exp') field found in the token."
    return 1
  fi

  local now
  now=$(date +%s)

  if [ "$now" -lt "$exp" ]; then
    local remaining
    remaining=$((exp - now))
    local minutes
    minutes=$((remaining / 60))
    echo "Token expires at: $(date -d @$exp)"
    echo "Time remaining: $minutes minutes."
  else
    echo "Token has expired. Expired at: $(date -d @$exp)"
  fi
}

echo ""

# Decode and display JWT info
jwt_decode "$1"
echo ""

# Check expiration
check_expiration "$1"
echo ""
