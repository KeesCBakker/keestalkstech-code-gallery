#!/usr/bin/env bash
set -euo pipefail

imageName="ktt-docker-todo"
integrationImage="$imageName-integration-test"

echo -e "\n🔨 Building integration test image: $integrationImage\n"
if ! docker build --target integration-test -t "$integrationImage" .; then
    echo "❌ Failed to build integration test image."
    exit 1
fi

echo -e "\n🚀 Running integration tests in: $integrationImage\n"
if ! docker run --rm --privileged "$integrationImage"; then
    echo "❌ Integration tests failed."
    exit 1
fi

echo -e "\n📦 Building final image: $imageName\n"
if ! docker build -t "$imageName" .; then
    echo "❌ Failed to build final image."
    exit 1
fi

echo -e "\n✅ Done\n"
