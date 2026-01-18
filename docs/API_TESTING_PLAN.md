# 🧪 CS2 API Testing Plan

## 📋 Übersicht

Dieses Dokument beschreibt, wie die möglichen CS2 APIs getestet werden können, um zu prüfen, ob sie verfügbar sind.

## 🔗 OFFIZIELLE API-DOKUMENTATION

### **📚 CounterStrikeSharp Haupt-Dokumentation:**
- **Offizielle Docs:** https://docs.cssharp.dev/
- **Getting Started:** https://docs.cssharp.dev/docs/guides/hello-world-plugin.html
- **API Referenz (Core):** https://docs.cssharp.dev/api/CounterStrikeSharp.API.Core.html
- **API Referenz (API):** https://busheezy.github.io/CounterStrikeSharp/api/CounterStrikeSharp.API.html

### **🔧 Wichtige API-Klassen:**
- **BasePlugin:** https://docs.cssharp.dev/api/CounterStrikeSharp.API.Core.BasePlugin.html
- **Utilities:** https://docs.cssharp.dev/api/CounterStrikeSharp.API.Utilities.html
- **NativeAPI:** https://busheezy.github.io/CounterStrikeSharp/api/CounterStrikeSharp.API.Core.NativeAPI.html
- **Api Klasse:** https://docs.cssharp.dev/api/CounterStrikeSharp.API.Api.html

### **💻 GitHub Repository:**
- **GitHub:** https://github.com/roflmuffin/CounterStrikeSharp
- **Wiki:** https://github.com/roflmuffin/CounterStrikeSharp/wiki
- **Issues:** https://github.com/roflmuffin/CounterStrikeSharp/issues
- **Discussions:** https://github.com/roflmuffin/CounterStrikeSharp/discussions
- **Examples:** Prüfe `/examples` Ordner im Repository

### **🎮 Source 2 SDK:**
- **Valve Developer Wiki:** https://developer.valvesoftware.com/wiki/Source_2
- **Valve Developer Community:** https://developer.valvesoftware.com/

## 🧪 Test-Plugin für API-Prüfung

### **Zu testende Properties:**

1. **Weapon Properties:**
   - `m_flSpread` - Weapon Spread
   - `m_flNextPrimaryAttack` - Fire Rate Control

2. **Player Pawn Properties:**
   - `m_flJumpPower` - Jump Height
   - `m_flAirControl` - Air Control
   - `m_bPlayFootstepSounds` - Footstep Sounds

3. **Collision Methods:**
   - `SetCollisionGroup()` - Collision Control

## 📝 Test-Code Beispiel

```csharp
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace UltimateHeroes.Tests
{
    public class ApiTestPlugin : BasePlugin
    {
        public override string ModuleName => "API Test Plugin";
        public override string ModuleVersion => "1.0.0";

        public override void Load(bool hotReload)
        {
            RegisterListener<Listeners.OnTick>(OnTick);
        }

        private void OnTick()
        {
            var players = Utilities.GetPlayers();
            foreach (var player in players)
            {
                if (player == null || !player.IsValid) continue;
                
                TestWeaponProperties(player);
                TestPlayerPawnProperties(player);
                TestCollisionMethods(player);
            }
        }

        private void TestWeaponProperties(CCSPlayerController player)
        {
            var pawn = player.PlayerPawn.Value;
            if (pawn?.WeaponServices?.ActiveWeapon.Value == null) return;
            
            var weapon = pawn.WeaponServices.ActiveWeapon.Value;
            
            // Test 1: m_flSpread
            try
            {
                var spread = weapon.GetProperty<float>("m_flSpread");
                Console.WriteLine($"[API Test] m_flSpread: {spread} ✅");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Test] m_flSpread: ❌ {ex.Message}");
            }
            
            // Test 2: m_flNextPrimaryAttack
            try
            {
                var nextAttack = weapon.GetProperty<float>("m_flNextPrimaryAttack");
                Console.WriteLine($"[API Test] m_flNextPrimaryAttack: {nextAttack} ✅");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Test] m_flNextPrimaryAttack: ❌ {ex.Message}");
            }
        }

        private void TestPlayerPawnProperties(CCSPlayerController player)
        {
            var pawn = player.PlayerPawn.Value;
            if (pawn == null) return;
            
            // Test 1: m_flJumpPower
            try
            {
                var jumpPower = pawn.GetProperty<float>("m_flJumpPower");
                Console.WriteLine($"[API Test] m_flJumpPower: {jumpPower} ✅");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Test] m_flJumpPower: ❌ {ex.Message}");
            }
            
            // Test 2: m_flAirControl
            try
            {
                var airControl = pawn.GetProperty<float>("m_flAirControl");
                Console.WriteLine($"[API Test] m_flAirControl: {airControl} ✅");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Test] m_flAirControl: ❌ {ex.Message}");
            }
            
            // Test 3: m_bPlayFootstepSounds
            try
            {
                var footstepSounds = pawn.GetProperty<bool>("m_bPlayFootstepSounds");
                Console.WriteLine($"[API Test] m_bPlayFootstepSounds: {footstepSounds} ✅");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Test] m_bPlayFootstepSounds: ❌ {ex.Message}");
            }
        }

        private void TestCollisionMethods(CCSPlayerController player)
        {
            var pawn = player.PlayerPawn.Value;
            if (pawn == null) return;
            
            // Test: SetCollisionGroup
            try
            {
                // Prüfe ob Methode existiert
                var method = pawn.GetType().GetMethod("SetCollisionGroup");
                if (method != null)
                {
                    Console.WriteLine($"[API Test] SetCollisionGroup: ✅ Methode gefunden");
                    // Teste Aufruf (vorsichtig!)
                    // method.Invoke(pawn, new object[] { CollisionGroup.COLLISION_GROUP_NONE });
                }
                else
                {
                    Console.WriteLine($"[API Test] SetCollisionGroup: ❌ Methode nicht gefunden");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Test] SetCollisionGroup: ❌ {ex.Message}");
            }
        }
    }
}
```

## 🔍 Alternative Test-Methoden

### **1. Reflection-basierte Prüfung:**
```csharp
// Prüfe alle verfügbaren Properties
var properties = weapon.GetType().GetProperties();
foreach (var prop in properties)
{
    Console.WriteLine($"Property: {prop.Name} ({prop.PropertyType.Name})");
}
```

### **2. ConVar-Prüfung:**
```csharp
// Prüfe verfügbare ConVars
var spreadConVar = ConVar.Find("weapon_accuracy_nospread");
if (spreadConVar != null)
{
    Console.WriteLine($"ConVar gefunden: {spreadConVar.Name}");
}
```

### **3. Native Function-Prüfung:**
```csharp
// Prüfe Native Functions (falls verfügbar)
// Dies hängt von CounterStrikeSharp's Native API ab
```

## 📊 Test-Ergebnisse Dokumentation

Nach dem Test sollten die Ergebnisse dokumentiert werden:

| Property/Methode | Verfügbar | GetProperty | SetProperty | Alternative |
|------------------|-----------|-------------|-------------|-------------|
| `m_flSpread` | ❓ | ❓ | ❓ | ConVar? |
| `m_flNextPrimaryAttack` | ❓ | ❓ | ❓ | Event Hook? |
| `m_flJumpPower` | ❓ | ❓ | ❓ | ConVar? |
| `m_flAirControl` | ❓ | ❓ | ❓ | ConVar? |
| `m_bPlayFootstepSounds` | ❓ | ❓ | ❓ | Event Hook? |
| `SetCollisionGroup()` | ❓ | N/A | ❓ | Solid Flags? |

## 🎯 Nächste Schritte

1. **Test-Plugin erstellen** basierend auf obigem Code
2. **Plugin auf CS2 Server laden** und testen
3. **Ergebnisse dokumentieren** in `TODO_IMPLEMENTATION_STATUS.md`
4. **Alternative Lösungen implementieren** für nicht-verfügbare APIs
5. **Workarounds dokumentieren** für API-Limits

## 📚 Weitere Ressourcen

- **CounterStrikeSharp Examples:** https://github.com/roflmuffin/CounterStrikeSharp/tree/main/examples
- **Source 2 Entity List:** Prüfe Source 2 SDK Dokumentation
- **CS2 Plugin Development:** Community-Diskussionen und Tutorials

---

**Hinweis:** Diese Tests sollten auf einem Test-Server durchgeführt werden, nicht auf einem Produktions-Server!
