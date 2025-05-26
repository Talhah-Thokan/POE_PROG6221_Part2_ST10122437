#!/bin/bash

# Make sure we're in the right directory
cd "$(dirname "$0")"

# Build the project
echo "Building CyberBot..."
dotnet build CyberBot.sln

# Run the application
echo "Starting CyberBot..."
dotnet run --project FINAL.csproj

echo "CyberBot closed." 