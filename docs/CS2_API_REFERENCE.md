# 📚 CS2 API Reference - CounterStrikeSharp

## 🔗 Offizielle Dokumentation

### **Haupt-Dokumentation:**
- **📚 Offizielle Docs:** https://docs.cssharp.dev/
- **🚀 Getting Started:** https://docs.cssharp.dev/docs/guides/hello-world-plugin.html
- **📖 API Referenz (Core):** https://docs.cssharp.dev/api/CounterStrikeSharp.API.Core.html
- **📖 API Referenz (API Namespace):** https://busheezy.github.io/CounterStrikeSharp/api/CounterStrikeSharp.API.html

### **GitHub Repository:**
- **GitHub:** https://github.com/roflmuffin/CounterStrikeSharp
- **Wiki:** https://github.com/roflmuffin/CounterStrikeSharp/wiki
- **Issues:** https://github.com/roflmuffin/CounterStrikeSharp/issues
- **Discussions:** https://github.com/roflmuffin/CounterStrikeSharp/discussions

## 📋 Wichtige API-Klassen

### **BasePlugin**
- **Link:** https://docs.cssharp.dev/api/CounterStrikeSharp.API.Core.BasePlugin.html
- **Beschreibung:** Basisklasse für alle Plugins
- **Wichtige Methoden:**
  - `Load(bool hotReload)` - Plugin wird geladen
  - `Unload(bool hotReload)` - Plugin wird entladen
  - `RegisterCommand()` - Commands registrieren
  - `RegisterEventHandler<T>()` - Event Handler registrieren
  - `RegisterListener<T>()` - Listener registrieren
  - `AddTimer()` - Timer hinzufügen

### **CCSPlayerController**
- **Beschreibung:** Repräsentiert einen Spieler
- **Wichtige Properties:**
  - `PlayerPawn.Value` - Zugriff auf Player Pawn
  - `AuthorizedSteamID` - Steam ID des Spielers
  - `IsValid` - Prüft ob Spieler gültig ist
- **Wichtige Methoden:**
  - `PrintToChat()` - Nachricht an Spieler senden

### **CCSPlayerPawn**
- **Beschreibung:** Player Pawn Entity
- **Wichtige Properties:**
  - `Health.Value` - Aktuelle HP
  - `ArmorValue` - Aktuelle Armor
  - `MovementServices` - Movement Services
  - `WeaponServices` - Weapon Services
  - `AbsOrigin` - Position
  - `EyeAngles` - Blickrichtung

### **CBasePlayerWeapon**
- **Beschreibung:** Waffe Entity
- **Wichtige Properties:**
  - `Clip1.Value` - Aktuelle Magazin-Munition
  - `ReserveAmmo.Value` - Reserve-Munition
  - `GetMaxClip1()` - Maximale Magazin-Größe
  - `GetMaxReserveAmmo()` - Maximale Reserve-Munition

### **Utilities**
- **Link:** https://docs.cssharp.dev/api/CounterStrikeSharp.API.Utilities.html
- **Wichtige Methoden:**
  - `GetPlayers()` - Alle Spieler abrufen
  - `CreateEntityByName<T>()` - Entity erstellen

### **NativeAPI**
- **Link:** https://busheezy.github.io/CounterStrikeSharp/api/CounterStrikeSharp.API.Core.NativeAPI.html
- **Beschreibung:** Zugriff auf Native Functions

### **ConVar**
- **Beschreibung:** Console Variable
- **Wichtige Methoden:**
  - `ConVar.Find(string name)` - ConVar finden
  - `GetPrimitiveValue<T>()` - Wert abrufen
  - `SetValue()` - Wert setzen

## 🔍 Property Access (GetProperty/SetProperty)

### **Status: Unbekannt**
Die Verfügbarkeit von `GetProperty<T>()` und `SetProperty<T>()` Methoden muss getestet werden.

### **Zu testende Properties:**

#### **Weapon Properties:**
- `m_flSpread` - Weapon Spread
- `m_flNextPrimaryAttack` - Fire Rate Control

#### **Player Pawn Properties:**
- `m_flJumpPower` - Jump Height
- `m_flAirControl` - Air Control
- `m_bPlayFootstepSounds` - Footstep Sounds

#### **Collision Methods:**
- `SetCollisionGroup()` - Collision Control

### **Test-Code:**
Siehe `docs/API_TESTING_PLAN.md` für vollständigen Test-Code.

## 📝 Beispiel-Code

### **Plugin Basis:**
```csharp
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

public class MyPlugin : BasePlugin
{
    public override string ModuleName => "My Plugin";
    public override string ModuleVersion => "1.0.0";

    public override void Load(bool hotReload)
    {
        // Plugin Initialisierung
    }

    public override void Unload(bool hotReload)
    {
        // Cleanup
    }
}
```

### **Event Handling:**
```csharp
public override void Load(bool hotReload)
{
    RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
}

public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
{
    var player = @event.Userid;
    if (player != null && player.IsValid)
    {
        // Event Handling
    }
    return HookResult.Continue;
}
```

### **Command Registrierung:**
```csharp
public override void Load(bool hotReload)
{
    RegisterCommand("css_myskill", "My Skill Command", OnSkillCommand);
}

private void OnSkillCommand(CCSPlayerController? player, CommandInfo commandInfo)
{
    if (player == null || !player.IsValid) return;
    player.PrintToChat("Skill activated!");
}
```

### **Timer:**
```csharp
public override void Load(bool hotReload)
{
    AddTimer(1.0f, OnTimer, TimerFlags.REPEAT);
}

private void OnTimer()
{
    // Timer Logic
}
```

## 🎯 Für unsere TODOs relevante APIs

### **Assist Tracking:**
- `EventPlayerHurt` - Damage Events
- `EventPlayerDeath` - Kill Events

### **Backstab Detection:**
- `CCSPlayerPawn.AbsOrigin` - Position
- `CCSPlayerPawn.EyeAngles` - Blickrichtung

### **Infinite Ammo:**
- `CBasePlayerWeapon.Clip1.Value` - Magazin
- `CBasePlayerWeapon.ReserveAmmo.Value` - Reserve
- `CBasePlayerWeapon.GetMaxClip1()` - Max Magazin
- `CBasePlayerWeapon.GetMaxReserveAmmo()` - Max Reserve

### **Movement Modifiers:**
- `CCSPlayerPawn.MovementServices.MoveSpeedFactor` - Movement Speed

### **Weapon Modifiers:**
- ❓ `GetProperty<float>("m_flSpread")` - Spread (zu testen)
- ❓ `GetProperty<float>("m_flNextPrimaryAttack")` - Fire Rate (zu testen)

## 📚 Weitere Ressourcen

- **CounterStrikeSharp Examples:** Prüfe `/examples` im GitHub Repository
- **Source 2 Entity List:** Prüfe Source 2 SDK Dokumentation
- **CS2 Plugin Development:** Community-Diskussionen und Tutorials

---

**Hinweis:** Diese Dokumentation wird regelmäßig aktualisiert. Für die neuesten Informationen, siehe die offizielle Dokumentation unter https://docs.cssharp.dev/
