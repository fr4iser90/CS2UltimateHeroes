# 🔧 Benötigte CS2 APIs für Ultimate Heroes

## ⚠️ WICHTIGER HINWEIS

**GetProperty/SetProperty sind NICHT explizit in der CounterStrikeSharp Dokumentation dokumentiert!**

**Ich habe die Dokumentation gelesen:**
- ✅ https://docs.cssharp.dev/ - Gelesen
- ✅ https://docs.cssharp.dev/api/CounterStrikeSharp.API.Core.html - Gelesen  
- ✅ Alle BasePlugin, CCSPlayerController, CBasePlayerWeapon Seiten - Gelesen

**Ergebnis:** `GetProperty<T>()` und `SetProperty<T>()` stehen **NICHT** in der Dokumentation!

CounterStrikeSharp verwendet ein **Schema-System** für Entity Properties, aber die genaue API ist nicht klar dokumentiert. Diese müssen **getestet** werden!

**Test-Plugin erstellt:** `test/ApiTestPlugin.cs` - Kompilieren und auf Server laden!

## 📋 Übersicht

Dieses Dokument listet alle APIs auf, die für die vollständige Implementierung der Ultimate Heroes Features benötigt werden.

## ✅ **BEREITS VERFÜGBAR & IMPLEMENTIERT**

### **Player APIs:**
- ✅ `CCSPlayerController.PlayerPawn.Value` - Player Pawn Zugriff
- ✅ `CCSPlayerPawn.Health.Value` - HP Get/Set
- ✅ `CCSPlayerPawn.ArmorValue` - Armor Get/Set
- ✅ `CCSPlayerPawn.MovementServices.MoveSpeedFactor` - Movement Speed
- ✅ `CCSPlayerPawn.AbsOrigin` - Position
- ✅ `CCSPlayerPawn.EyeAngles` - Blickrichtung
- ✅ `CCSPlayerPawn.RenderMode` - Render Mode (für Invisibility)
- ✅ `CCSPlayerPawn.Render` - Render Color

### **Weapon APIs:**
- ✅ `CBasePlayerWeapon.Clip1.Value` - Magazin-Munition Get/Set
- ✅ `CBasePlayerWeapon.ReserveAmmo.Value` - Reserve-Munition Get/Set
- ✅ `CBasePlayerWeapon.GetMaxClip1()` - Maximale Magazin-Größe
- ✅ `CBasePlayerWeapon.GetMaxReserveAmmo()` - Maximale Reserve-Munition

### **Event APIs:**
- ✅ `EventPlayerHurt` - Damage Events
- ✅ `EventPlayerDeath` - Kill Events
- ✅ `EventRoundStart` - Round Start
- ✅ `EventRoundEnd` - Round End

### **Utility APIs:**
- ✅ `Utilities.GetPlayers()` - Alle Spieler abrufen
- ✅ `ConVar.Find()` - ConVars finden
- ✅ `Server.CurrentTime` - Server Zeit

---

## ❓ **FEHLEND & BENÖTIGT (Zu testen/prüfen)**

### **1. Weapon Spread Modifier** 🔴 KRITISCH
**Benötigt für:**
- Taunt Effect (Weapon Spread Increase)
- Accuracy Boost Buffs

**Problem:** GetProperty/SetProperty sind NICHT in der Dokumentation dokumentiert!

**Mögliche APIs (MÜSSEN GETESTET WERDEN):**
```csharp
// Option 1: Schema System (CounterStrikeSharp)
// NICHT DOKUMENTIERT - MUSS GETESTET WERDEN!
weapon.GetProperty<float>("m_flSpread")
weapon.SetProperty<float>("m_flSpread", newValue)

// Option 2: ConVar (global, nicht ideal)
ConVar.Find("weapon_accuracy_nospread")?.SetValue(0)

// Option 3: Native Function (falls verfügbar)
NativeAPI.SetWeaponSpread(weapon, spreadValue)
```

**Status:** ❌ NICHT DOKUMENTIERT - Test erforderlich  
**Dokumentation:** https://docs.cssharp.dev/ (keine explizite Dokumentation für GetProperty/SetProperty)  
**Alternative:** Falls nicht verfügbar → Workaround via Recoil/Accuracy Modifiers oder Feature deaktivieren

---

### **2. Fire Rate Modifier** 🔴 KRITISCH
**Benötigt für:**
- Bullet Storm Ultimate (Fire Rate Multiplier)

**Problem:** GetProperty/SetProperty sind NICHT in der Dokumentation dokumentiert!

**Mögliche APIs (MÜSSEN GETESTET WERDEN):**
```csharp
// Option 1: Schema System (CounterStrikeSharp)
// NICHT DOKUMENTIERT - MUSS GETESTET WERDEN!
weapon.GetProperty<float>("m_flNextPrimaryAttack")
weapon.SetProperty<float>("m_flNextPrimaryAttack", newValue)

// Option 2: Native Function (falls verfügbar)
NativeAPI.SetWeaponFireRate(weapon, fireRateMultiplier)

// Option 3: Attack Time Manipulation (falls Property verfügbar)
weapon.SetProperty<float>("m_flNextPrimaryAttack", 
    Server.CurrentTime - (baseAttackTime * (1f - multiplier)))
```

**Status:** ❌ NICHT DOKUMENTIERT - Test erforderlich  
**Dokumentation:** https://docs.cssharp.dev/ (keine explizite Dokumentation für GetProperty/SetProperty)  
**Alternative:** Falls nicht verfügbar → Feature deaktivieren oder nur Infinite Ammo (ohne Fire Rate)

---

### **3. Collision Disable** 🟡 WICHTIG
**Benötigt für:**
- Shadow Realm Ultimate (No Collision)

**Problem:** SetCollisionGroup/SetProperty sind NICHT in der Dokumentation dokumentiert!

**Mögliche APIs (MÜSSEN GETESTET WERDEN):**
```csharp
// Option 1: Collision Group (falls verfügbar)
// NICHT DOKUMENTIERT - MUSS GETESTET WERDEN!
pawn.SetCollisionGroup(CollisionGroup.COLLISION_GROUP_NONE)

// Option 2: Solid Flags (falls verfügbar)
pawn.SetSolidFlags(SolidFlags_t.SOLID_NOT)

// Option 3: Schema System (falls verfügbar)
pawn.SetProperty<int>("m_CollisionGroup", COLLISION_GROUP_NONE)
```

**Status:** ❌ NICHT DOKUMENTIERT - Test erforderlich  
**Dokumentation:** https://docs.cssharp.dev/ (keine explizite Dokumentation)  
**Alternative:** ✅ Workaround implementiert: Ghost Mode (100% Damage Reduction + Invisibility)

---

### **4. Silent Footsteps** 🟡 WICHTIG
**Benötigt für:**
- Silent Footsteps Passive

**Problem:** GetProperty/SetProperty sind NICHT in der Dokumentation dokumentiert!

**Mögliche APIs (MÜSSEN GETESTET WERDEN):**
```csharp
// Option 1: Schema System (falls verfügbar)
// NICHT DOKUMENTIERT - MUSS GETESTET WERDEN!
pawn.GetProperty<bool>("m_bPlayFootstepSounds")
pawn.SetProperty<bool>("m_bPlayFootstepSounds", false)

// Option 2: ConVar (global, nicht ideal)
ConVar.Find("sv_footsteps")?.SetValue(0)

// Option 3: Native Function (falls verfügbar)
NativeAPI.SetPlayerFootstepSounds(player, false)
```

**Status:** ❌ NICHT DOKUMENTIERT - Test erforderlich  
**Dokumentation:** https://docs.cssharp.dev/ (keine explizite Dokumentation)  
**Alternative:** Falls nicht verfügbar → Feature deaktivieren

---

### **5. Jump Height Modifier** 🟡 WICHTIG
**Benötigt für:**
- PlayerService Jump Height Modifier
- Talent Modifiers

**Problem:** GetProperty/SetProperty sind NICHT in der Dokumentation dokumentiert!

**Mögliche APIs (MÜSSEN GETESTET WERDEN):**
```csharp
// Option 1: Schema System (falls verfügbar)
// NICHT DOKUMENTIERT - MUSS GETESTET WERDEN!
pawn.GetProperty<float>("m_flJumpPower")
pawn.SetProperty<float>("m_flJumpPower", baseJump * multiplier)

// Option 2: ConVar (global, nicht ideal)
ConVar.Find("sv_jump_impulse")?.SetValue(baseJump * multiplier)

// Option 3: Native Function (falls verfügbar)
NativeAPI.SetPlayerJumpHeight(player, baseJump * multiplier)
```

**Status:** ❌ NICHT DOKUMENTIERT - Test erforderlich  
**Dokumentation:** https://docs.cssharp.dev/ (keine explizite Dokumentation)  
**Alternative:** Falls nicht verfügbar → Feature deaktivieren

---

### **6. Air Control Modifier** 🟡 WICHTIG
**Benötigt für:**
- PlayerService Air Control Modifier
- Talent Modifiers

**Problem:** GetProperty/SetProperty sind NICHT in der Dokumentation dokumentiert!

**Mögliche APIs (MÜSSEN GETESTET WERDEN):**
```csharp
// Option 1: Schema System (falls verfügbar)
// NICHT DOKUMENTIERT - MUSS GETESTET WERDEN!
pawn.GetProperty<float>("m_flAirControl")
pawn.SetProperty<float>("m_flAirControl", baseAirControl * multiplier)

// Option 2: ConVar (global, nicht ideal)
ConVar.Find("sv_air_max_wishspeed")?.SetValue(baseAirControl * multiplier)

// Option 3: Native Function (falls verfügbar)
NativeAPI.SetPlayerAirControl(player, baseAirControl * multiplier)
```

**Status:** ❌ NICHT DOKUMENTIERT - Test erforderlich  
**Dokumentation:** https://docs.cssharp.dev/ (keine explizite Dokumentation)  
**Alternative:** Falls nicht verfügbar → Feature deaktivieren

---

## 📊 **PRIORITÄTEN**

### **🔴 KRITISCH (Muss funktionieren):**
1. **Weapon Spread Modifier** - Für Taunt Effect
2. **Fire Rate Modifier** - Für Bullet Storm Ultimate

### **🟡 WICHTIG (Sollte funktionieren):**
3. **Collision Disable** - Für Shadow Realm Ultimate
4. **Silent Footsteps** - Für Silent Footsteps Passive
5. **Jump Height Modifier** - Für Talent Modifiers
6. **Air Control Modifier** - Für Talent Modifiers

---

## 🧪 **TEST-PLAN**

### **Phase 1: Property Access Test**
Teste ob `GetProperty<T>()` und `SetProperty<T>()` verfügbar sind:
```csharp
// Test für alle Properties
var spread = weapon.GetProperty<float>("m_flSpread");
var jumpPower = pawn.GetProperty<float>("m_flJumpPower");
var airControl = pawn.GetProperty<float>("m_flAirControl");
var footstepSounds = pawn.GetProperty<bool>("m_bPlayFootstepSounds");
```

### **Phase 2: Method Access Test**
Teste ob Methoden verfügbar sind:
```csharp
// Test für Collision
var method = pawn.GetType().GetMethod("SetCollisionGroup");
if (method != null) {
    // Methode existiert
}
```

### **Phase 3: ConVar Test**
Teste ob ConVars verfügbar sind:
```csharp
var spreadConVar = ConVar.Find("weapon_accuracy_nospread");
var jumpConVar = ConVar.Find("sv_jump_impulse");
var footstepConVar = ConVar.Find("sv_footsteps");
```

### **Phase 4: Native Function Test**
Teste ob Native Functions verfügbar sind:
```csharp
// Prüfe NativeAPI Klasse
// Siehe: https://busheezy.github.io/CounterStrikeSharp/api/CounterStrikeSharp.API.Core.NativeAPI.html
```

---

## 📚 **DOKUMENTATION LINKS**

### **CounterStrikeSharp:**
- **Offizielle Docs:** https://docs.cssharp.dev/
- **API Referenz:** https://docs.cssharp.dev/api/CounterStrikeSharp.API.Core.html
- **NativeAPI:** https://busheezy.github.io/CounterStrikeSharp/api/CounterStrikeSharp.API.Core.NativeAPI.html

### **Source 2 SDK:**
- **Valve Developer Wiki:** https://developer.valvesoftware.com/wiki/Source_2

---

## 🎯 **NÄCHSTE SCHRITTE**

1. **Test-Plugin erstellen** (siehe `docs/API_TESTING_PLAN.md`)
2. **Alle Properties testen** auf CS2 Server
3. **Ergebnisse dokumentieren** in `TODO_IMPLEMENTATION_STATUS.md`
4. **Alternative Lösungen implementieren** für nicht-verfügbare APIs
5. **Workarounds dokumentieren** für API-Limits

---

## 📝 **ZUSAMMENFASSUNG**

| API | Status | Priorität | Alternative |
|-----|--------|-----------|-------------|
| Weapon Spread | ❌ **NICHT DOKUMENTIERT** | 🔴 Kritisch | Recoil Modifier / Feature deaktivieren |
| Fire Rate | ❌ **NICHT DOKUMENTIERT** | 🔴 Kritisch | Feature deaktivieren (nur Infinite Ammo) |
| Collision Disable | ❌ **NICHT DOKUMENTIERT** | 🟡 Wichtig | ✅ Ghost Mode Workaround (bereits implementiert) |
| Silent Footsteps | ❌ **NICHT DOKUMENTIERT** | 🟡 Wichtig | Feature deaktivieren |
| Jump Height | ❌ **NICHT DOKUMENTIERT** | 🟡 Wichtig | Feature deaktivieren |
| Air Control | ❌ **NICHT DOKUMENTIERT** | 🟡 Wichtig | Feature deaktivieren |

**Gesamt:** 6 APIs sind NICHT dokumentiert und müssen getestet werden:
- 2 kritisch (Weapon Spread, Fire Rate)
- 4 wichtig (Collision, Silent Footsteps, Jump Height, Air Control)

**WICHTIG:** GetProperty/SetProperty sind **NICHT** in der offiziellen CounterStrikeSharp Dokumentation dokumentiert! Sie müssen via Test-Plugin geprüft werden!
