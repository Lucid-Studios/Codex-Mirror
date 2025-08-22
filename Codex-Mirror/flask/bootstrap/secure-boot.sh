#!/usr/bin/env bash
set -euo pipefail
echo "[secure-boot] starting checks..."

command -v python3 >/dev/null || { echo "python3 missing"; exit 1; }
command -v git >/dev/null || { echo "git missing"; exit 1; }

echo "[secure-boot] OK"
