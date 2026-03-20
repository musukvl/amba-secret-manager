#!/usr/bin/env bash
set -euo pipefail

SOLUTION="Amba.SecretManager/Amba.SecretManager.sln"
CONFIGURATION="${1:-Release}"

echo "==> Restoring..."
dotnet restore "$SOLUTION"

echo "==> Building ($CONFIGURATION)..."
dotnet build "$SOLUTION" -c "$CONFIGURATION" --no-restore

echo "==> Running tests..."
dotnet test "$SOLUTION" -c "$CONFIGURATION" --no-build

echo "==> Done."
