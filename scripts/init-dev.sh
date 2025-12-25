#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

require_cmd() {
  if ! command -v "$1" >/dev/null 2>&1; then
    echo "Missing required command: $1" >&2
    exit 1
  fi
}

require_cmd yarn
require_cmd dotnet

echo "Installing Node and .NET tooling..."
(
  cd "$ROOT_DIR"
  yarn install --frozen-lockfile
  dotnet tool restore
)

echo "Tooling install complete. No database actions performed."

