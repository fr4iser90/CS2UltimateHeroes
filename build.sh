#!/bin/bash

echo "=== Ultimate Heroes Plugin Build Script ==="
echo ""

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null
then
    echo "❌ .NET SDK nicht gefunden!"
    echo ""
    echo "Bitte starte die Nix-Shell:"
    echo "  nix-shell"
    echo "  # oder"
    echo "  nix develop"
    echo ""
    echo "Oder installiere .NET 8.0 SDK manuell:"
    echo "  Linux: https://learn.microsoft.com/dotnet/core/install/linux"
    echo ""
    exit 1
fi

echo "✅ .NET Version: $(dotnet --version)"
echo ""

# Navigate to project directory
cd src/UltimateHeroes || exit 1

echo "📦 Restore NuGet Packages..."
dotnet restore

echo ""
echo "🔨 Kompiliere Projekt (Release)..."
dotnet build -c Release

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ Build erfolgreich!"
    echo ""
    echo "📁 Kompilierte Dateien:"
    echo "   src/UltimateHeroes/bin/Release/net8.0/"
    echo ""
    echo "📋 Nächste Schritte:"
    echo "   1. Kopiere alle Dateien aus bin/Release/net8.0/ in deinen CounterStrikeSharp plugins/UltimateHeroes/ Ordner"
    echo "   2. Starte deinen CS2 Server"
    echo "   3. Teste mit !class, !skills, etc."
else
    echo ""
    echo "❌ Build fehlgeschlagen! Prüfe die Fehlermeldungen oben."
    exit 1
fi
