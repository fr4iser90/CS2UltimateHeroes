{ pkgs ? import <nixpkgs> {} }:

pkgs.mkShell {
  buildInputs = with pkgs; [
    dotnet-sdk_8
    dotnet-runtime_8
  ];

  shellHook = ''
    echo "=== UltimateHeroes Plugin Development Environment ==="
    echo "✅ .NET SDK Version:"
    dotnet --version
    echo ""
    echo "📁 Aktuelles Verzeichnis: $(pwd)"
    echo ""
    echo "🔨 Verfügbare Befehle:"
    echo "   dotnet restore    - NuGet Packages wiederherstellen"
    echo "   dotnet build      - Projekt kompilieren"
    echo "   dotnet build -c Release  - Release Build"
    echo "   build-heroes      - Alias: cd src/UltimateHeroes && dotnet restore && dotnet build -c Release"
    echo ""
    
    # Alias für schnelles Build
    alias build-heroes='cd src/UltimateHeroes && dotnet restore && dotnet build -c Release'
  '';
}
