#!/usr/bin/env bash
set -euo pipefail

# Build, pack, and install the tool globally from local source.
# Usage: ./scripts/install-local.sh <VERSION>
# After running this, "sm" will be available on your PATH.

if [ $# -lt 1 ]; then
    echo "Usage: $0 <VERSION>"
    exit 1
fi

VERSION="$1"
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"

cd "$ROOT_DIR"

bash scripts/build.sh Release
bash scripts/pack.sh "$VERSION" Release

echo "==> Uninstalling previous version (if any)..."
dotnet tool uninstall -g Amba.SecretManager 2>/dev/null || true

echo "==> Installing from local package..."
dotnet tool install -g Amba.SecretManager --version "$VERSION" --add-source artifacts

echo "==> Installed. Verify with:"
echo "    sm --help"
