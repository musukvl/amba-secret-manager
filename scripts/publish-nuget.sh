#!/usr/bin/env bash
set -euo pipefail

# Publish the tool to NuGet.org.
# Usage: NUGET_API_KEY=<key> ./scripts/publish-nuget.sh <VERSION>

if [ $# -lt 1 ]; then
    echo "Usage: NUGET_API_KEY=<key> $0 <VERSION>"
    exit 1
fi

if [ -z "${NUGET_API_KEY:-}" ]; then
    echo "Error: NUGET_API_KEY environment variable is not set."
    exit 1
fi

VERSION="$1"
OUTPUT_DIR="artifacts"

bash scripts/build.sh Release
bash scripts/pack.sh "$VERSION" Release

PACKAGE="$OUTPUT_DIR/Amba.SecretManager.${VERSION}.nupkg"

echo "==> Pushing $PACKAGE to NuGet.org..."
dotnet nuget push "$PACKAGE" --api-key "$NUGET_API_KEY" --source https://api.nuget.org/v3/index.json

echo "==> Published. Install with:"
echo "    dotnet tool install -g Amba.SecretManager"
