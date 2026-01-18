# 🧪 API Test Plugin

## 📋 Zweck

Dieses Plugin testet, welche CS2 APIs verfügbar sind, die nicht in der Dokumentation stehen.

## 📁 Location

Das Test-Plugin ist jetzt im Hauptprojekt integriert:
- **Datei:** `src/UltimateHeroes/Infrastructure/Testing/ApiTestPlugin.cs`

## 🚀 Aktivierung

Um das Test-Plugin zu aktivieren, füge in `UltimateHeroes.cs` hinzu:

```csharp
#if DEBUG
// API Testing aktivieren
var apiTest = new Infrastructure.Testing.ApiTestPlugin();
apiTest.Load(false);
#endif
```

**ODER:** Kompiliere das Hauptprojekt normal - das Test-Plugin ist bereits enthalten.

## 📊 Ergebnisse

Die Test-Ergebnisse werden in die Server-Konsole ausgegeben. Dokumentiere sie in:
`docs/API_TEST_RESULTS.md`

## 🔍 Was wird getestet?

- Weapon Properties (m_flSpread, m_flNextPrimaryAttack)
- Player Pawn Properties (m_flJumpPower, m_flAirControl, m_bPlayFootstepSounds, m_CollisionGroup)
- Collision Methods (SetCollisionGroup)
- ConVars (weapon_accuracy_nospread, sv_jump_impulse, sv_footsteps)
