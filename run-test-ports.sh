#!/bin/bash
echo "========================================"
echo "Building Test Port Opener..."
echo "========================================"
dotnet build TestPortOpener/TestPortOpener.csproj
echo ""
echo "========================================"
echo "Starting Test Port Opener..."
echo "========================================"
echo ""
dotnet run --project TestPortOpener/TestPortOpener.csproj
