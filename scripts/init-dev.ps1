[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
function Require-Command($name) {
  if (-not (Get-Command $name -ErrorAction SilentlyContinue)) {
    throw "$name is required but was not found in PATH."
  }
}

Require-Command "yarn"
Require-Command "dotnet"

Write-Host "Installing Node and .NET tooling..."
Push-Location $repoRoot
try {
  yarn install --frozen-lockfile
  dotnet tool restore
}
finally {
  Pop-Location
}

Write-Host "Tooling install complete. No database actions performed."
