# Decoding a JWT token, based on the discussion here:
# https://gist.github.com/thomasdarimont/46358bc8167fce059d83a1ebdb92b0e7
# More here:
# https://keestalkstech.com/2024/12/debug-a-jwt-token-expiration-locally-with-bash-or-powershell/

function Decode-JWT {
    param (
        [string]$Token
    )

    $parts = $Token -split '\.'
    if ($parts.Count -ne 3) {
        Write-Error "Invalid JWT format."
        return $null
    }

    try {
        $payloadBase64 = $parts[1].PadRight($parts[1].Length + (4 - $parts[1].Length % 4) % 4, '=')
        $payload = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($payloadBase64))
        return $payload | ConvertFrom-Json
    } catch {
        Write-Error "Failed to decode or parse JSON payload. Ensure the token is a valid Base64-encoded string."
        return $null
    }
}

function Check-Expiration {
    param (
        [string]$Token
    )

    $payload = Decode-JWT $Token
    if (-not $payload) {
        Write-Error "Invalid JWT or unable to decode."
        return
    }

    if (-not $payload.exp) {
        Write-Error "No expiration ('exp') field found in the token."
        return
    }

    $exp = [long]$payload.exp
    $now = [long][DateTimeOffset]::Now.ToUnixTimeSeconds()

    if ($now -lt $exp) {
        $remaining = $exp - $now
        $minutes = [math]::Floor($remaining / 60)
        Write-Host "Token expires at: $(Get-Date -Date ([datetime]::FromFileTimeUtc($exp * 10000000 + 116444736000000000)))"
        Write-Host "Time remaining: $minutes minutes."
    } else {
        Write-Host "Token has expired. Expired at: $(Get-Date -Date ([datetime]::FromFileTimeUtc($exp * 10000000 + 116444736000000000)))"
    }
}

if ($args.Count -eq 0) {
    Write-Error "No JWT provided."
    return
}

$Token = $args[0]

Decode-JWT $Token
Check-Expiration $Token
Write-Host ""
