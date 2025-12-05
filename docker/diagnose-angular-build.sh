#!/usr/bin/env bash
set -euo pipefail

log() {
  printf '\n[%s] %s\n' "$(date +%H:%M:%S)" "$*"
}

measure() {
  local label=$1
  shift
  log "START ${label}"
  local start=$SECONDS
  "$@"
  local duration=$((SECONDS - start))
  log "END ${label} (${duration}s)"
}

cd /workspace

log "Node: $(node -v)"
log "Yarn: $(yarn -v)"
log "CPU cores: $(nproc --all)"
log "Memory: $(awk '/MemTotal/ {print int($2/1024) \" MB\"}' /proc/meminfo)"

if [[ "${RESET_CACHE:-0}" == "1" ]]; then
  log "RESET_CACHE=1 -> cleaning caches"
  yarn cache clean --all || true
  rm -rf node_modules .nx dist/apps/todo || true
fi

measure "yarn install" yarn install --frozen-lockfile --ignore-scripts --ignore-optional --cache-folder /tmp/yarn-cache

measure "nx build todo (production)" yarn nx build todo --configuration=production --skip-nx-cache=false

measure "disk write check (256MiB fdatasync)" dd if=/dev/zero of=/workspace/.tmp-dd bs=1M count=256 conv=fdatasync status=none
rm -f /workspace/.tmp-dd

log "node_modules size: $(du -sh node_modules 2>/dev/null | awk '{print $1}')"
log ".nx/cache size: $(du -sh .nx/cache 2>/dev/null | awk '{print $1}')"
