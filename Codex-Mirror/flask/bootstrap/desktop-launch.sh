#!/usr/bin/env bash
set -euo pipefail
DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
"$DIR/bootstrap/secure-boot.sh"
echo "[launch] starting operator flask in $DIR"

if [ ! -d "$DIR/.venv" ]; then
  python3 -m venv "$DIR/.venv"
fi
source "$DIR/.venv/bin/activate"
pip install -U pip -q
pip install -r "$DIR/requirements.txt" -q

echo "[launch] environment ready."
echo "[launch] run scripts in ./scripts"
