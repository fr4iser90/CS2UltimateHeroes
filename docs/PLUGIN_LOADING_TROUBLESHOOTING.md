# 🔧 Plugin Loading Troubleshooting

## Problem: Plugin wird nicht geladen

Wenn `meta list` nur "CounterStrikeSharp" zeigt, aber nicht "Ultimate Heroes", dann ist das Plugin nicht geladen.

## ✅ Checkliste:

### 1. **Plugin-Verzeichnis prüfen**

Das Plugin muss im richtigen Verzeichnis sein:

```
addons/counterstrikesharp/plugins/UltimateHeroes/
├── UltimateHeroes.dll
├── UltimateHeroes.deps.json
├── UltimateHeroes.runtimeconfig.json
└── (alle anderen DLLs aus bin/Release/net8.0/)
```

**WICHTIG:** Alle DLLs aus `bin/Release/net8.0/` müssen kopiert werden!

### 2. **Config-Datei prüfen**

Die Config-Datei wird automatisch erstellt, sollte aber existieren:

```
addons/counterstrikesharp/configs/plugins/UltimateHeroes/
└── UltimateHeroes.json
```

**Falls die Datei fehlt**, erstelle sie manuell:

```json
{
  "ConfigVersion": 1,
  "DefaultHero": "vanguard",
  "MaxSkillSlots": 3,
  "MaxPowerBudget": 100,
  "BotSettings": {
    "Enabled": true,
    "DefaultLevelMode": "MatchPlayerAverage",
    "DefaultBuildMode": "Random",
    "TrackStats": true,
    "AutoAssignBuild": true,
    "BuildChangeInterval": 300.0,
    "PredefinedBuilds": ["dps", "mobility", "stealth", "support", "balanced"],
    "BuildPool": [],
    "XpPerKill": 25.0
  }
}
```

### 3. **Server-Logs prüfen**

Schau in die Server-Konsole nach:

**Erfolg:**
```
[UltimateHeroes] Plugin loaded successfully!
```

**Fehler:**
```
[ERROR] Failed to load UltimateHeroes: ...
```

### 4. **Dependencies prüfen**

Stelle sicher, dass alle Dependencies vorhanden sind:
- `CounterStrikeSharp.API.dll`
- `Dapper.dll`
- `Microsoft.Data.Sqlite.dll`
- `Newtonsoft.Json.dll`
- Alle anderen DLLs aus `bin/Release/net8.0/`

### 5. **Plugin neu laden**

Im RCON:
```
> meta reload UltimateHeroes
```

Oder Server neu starten.

### 6. **Commands testen**

Nach erfolgreichem Laden sollten diese Commands funktionieren:
- `!hero` oder `css_hero`
- `!build` oder `css_build`
- `!skills` oder `css_skills`
- `!stats` oder `css_stats`

### 7. **Admin-Commands testen**

Wenn du als Admin eingetragen bist:
- `!admin_listplayers` oder `css_admin_listplayers`
- `!admin_givexp <player> <amount>` oder `css_admin_givexp <player> <amount>`

## 🐛 Häufige Fehler:

### Fehler: "Plugin not found"
- **Ursache:** DLL nicht im richtigen Verzeichnis
- **Lösung:** Prüfe Verzeichnisstruktur (siehe Punkt 1)

### Fehler: "Config file not found"
- **Ursache:** Config-Datei fehlt oder ist fehlerhaft
- **Lösung:** Erstelle Config-Datei manuell (siehe Punkt 2)

### Fehler: "Dependencies missing"
- **Ursache:** Nicht alle DLLs kopiert
- **Lösung:** Kopiere ALLE Dateien aus `bin/Release/net8.0/`

### Fehler: "AdminManager not found"
- **Ursache:** `AdminManager` ist möglicherweise nicht verfügbar
- **Lösung:** Plugin sollte trotzdem laden, Admin-Commands funktionieren dann nicht

## 📋 Debug-Befehle:

### Im RCON:
```
> meta list
> meta info UltimateHeroes
> meta reload UltimateHeroes
```

### In der Server-Konsole:
```
log on
```

Dann schau nach `[UltimateHeroes]` oder `[ERROR]` Meldungen.
