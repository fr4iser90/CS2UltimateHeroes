# ⚠️ API Status Klarstellung

## 🔍 **WARUM "UNBEKANNT"?**

Ich habe die CounterStrikeSharp Dokumentation gelesen, aber:

### **Was DOKUMENTIERT ist:**
- ✅ `BasePlugin` - Vollständig dokumentiert
- ✅ `CCSPlayerController` - Dokumentiert
- ✅ `CCSPlayerPawn` - Dokumentiert (Health, Armor, MovementServices, etc.)
- ✅ `CBasePlayerWeapon` - Dokumentiert (Clip1, ReserveAmmo, GetMaxClip1, etc.)
- ✅ `EventPlayerDeath`, `EventPlayerHurt` - Dokumentiert
- ✅ `Utilities.GetPlayers()` - Dokumentiert
- ✅ `ConVar.Find()` - Dokumentiert

### **Was NICHT DOKUMENTIERT ist:**
- ❌ `GetProperty<T>()` - **NICHT in der Dokumentation**
- ❌ `SetProperty<T>()` - **NICHT in der Dokumentation**
- ❌ `SetCollisionGroup()` - **NICHT in der Dokumentation**
- ❌ Schema System Property Access - **NICHT explizit dokumentiert**

## 📚 **Dokumentation Links (was ich gelesen habe):**

1. **Offizielle Docs:** https://docs.cssharp.dev/
2. **API Referenz:** https://docs.cssharp.dev/api/CounterStrikeSharp.API.Core.html
3. **BasePlugin:** https://docs.cssharp.dev/api/CounterStrikeSharp.API.Core.BasePlugin.html
4. **Utilities:** https://docs.cssharp.dev/api/CounterStrikeSharp.API.Utilities.html

**Problem:** In diesen Dokumentationen steht **NICHTS** über `GetProperty/SetProperty`!

## 🔍 **Was CounterStrikeSharp WIRKLICH hat:**

CounterStrikeSharp verwendet ein **Schema-System** für Entity Properties, aber:
- Die genaue API ist **nicht explizit dokumentiert**
- Es könnte `GetProperty<T>()` geben, aber es ist **nicht dokumentiert**
- Es könnte über Reflection funktionieren, aber das ist **nicht dokumentiert**

## ✅ **WAS WIR WIRKLICH BRAUCHEN:**

### **Bereits verfügbar (dokumentiert):**
1. ✅ Infinite Ammo - `weapon.Clip1.Value`, `weapon.ReserveAmmo.Value`
2. ✅ Movement Speed - `pawn.MovementServices.MoveSpeedFactor`
3. ✅ Health/Armor - `pawn.Health.Value`, `pawn.ArmorValue`
4. ✅ Invisibility - `pawn.RenderMode`, `pawn.Render`
5. ✅ Events - `EventPlayerDeath`, `EventPlayerHurt`
6. ✅ Assist Tracking - Via Events (bereits implementiert)
7. ✅ Backstab Detection - Via Position/Angles (bereits implementiert)

### **NICHT dokumentiert (muss getestet werden):**
1. ❌ Weapon Spread - `GetProperty<float>("m_flSpread")` - **NICHT DOKUMENTIERT**
2. ❌ Fire Rate - `GetProperty<float>("m_flNextPrimaryAttack")` - **NICHT DOKUMENTIERT**
3. ❌ Collision - `SetCollisionGroup()` - **NICHT DOKUMENTIERT**
4. ❌ Silent Footsteps - `GetProperty<bool>("m_bPlayFootstepSounds")` - **NICHT DOKUMENTIERT**
5. ❌ Jump Height - `GetProperty<float>("m_flJumpPower")` - **NICHT DOKUMENTIERT**
6. ❌ Air Control - `GetProperty<float>("m_flAirControl")` - **NICHT DOKUMENTIERT**

## 🎯 **FAZIT:**

**"Unbekannt" bedeutet:** Die API existiert möglicherweise (via Schema System), ist aber **NICHT in der offiziellen Dokumentation** dokumentiert und muss **getestet** werden!

**Nächster Schritt:** Test-Plugin erstellen und alle Properties testen (siehe `docs/API_TESTING_PLAN.md`)
