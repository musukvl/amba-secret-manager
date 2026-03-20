#!/usr/bin/env bash
set -euo pipefail

# Usage: ./scripts/pack.sh <VERSION> [CONFIGURATION]
# Example: ./scripts/pack.sh 1.0.0
#          ./scripts/pack.sh 1.2.3 Debug

if [ $# -lt 1 ]; then
    echo "Usage: $0 <VERSION> [CONFIGURATION]"
    exit 1
fi

VERSION="$1"
CONFIGURATION="${2:-Release}"
PROJECT="Amba.SecretManager/Amba.SecretManager/Amba.SecretManager.csproj"
OUTPUT_DIR="artifacts"

echo "==> Packing NuGet tool package v${VERSION} ($CONFIGURATION)..."
dotnet pack "$PROJECT" -c "$CONFIGURATION" -o "$OUTPUT_DIR" /p:Version="$VERSION"

echo "==> Package created in $OUTPUT_DIR/"
ls -la "$OUTPUT_DIR"/Amba.SecretManager."${VERSION}".nupkg
