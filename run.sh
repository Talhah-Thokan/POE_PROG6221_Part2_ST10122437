#!/bin/bash

echo "=== CyberBot WPF Application ==="
echo "This is a Windows WPF application that requires Windows to run."
echo "On macOS, we can only build it, not run it."
echo ""

if [[ "$OSTYPE" == "darwin"* ]]; then
    echo "macOS detected - Building for Windows deployment..."
    dotnet build FINAL.csproj --configuration Release
    echo ""
    echo "Build complete! Copy the bin/Release/net8.0-windows/ folder to a Windows machine to run."
    echo "On Windows, run: CyberBot.exe"
else
    echo "Running on Windows..."
    dotnet run --project FINAL.csproj
fi 