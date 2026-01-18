# 🧪 Wie man die APIs testet

## ⚠️ WICHTIG: Warum "Unbekannt"?

**Ich habe die CounterStrikeSharp Dokumentation gelesen:**
- ✅ https://docs.cssharp.dev/ - Gelesen
- ✅ https://docs.cssharp.dev/api/CounterStrikeSharp.API.Core.html - Gelesen
- ✅ https://busheezy.github.io/CounterStrikeSharp/api/CounterStrikeSharp.API.html - Gelesen

**Ergebnis:** `GetProperty<T>()` und `SetProperty<T>()` sind **NICHT** in der Dokumentation!

**ABER:** CounterStrikeSharp verwendet ein Schema-System, das möglicherweise Property-Zugriff ermöglicht - das muss getestet werden!

## 🚀 SO TESTEST DU ES:

### **Schritt 1: Test-Plugin erstellen**

Ich habe bereits ein Test-Plugin erstellt: `test/ApiTestPlugin.cs`

### **Schritt 2: Plugin kompilieren**

```bash
cd /home/fr4iser/Documents/Git/CS2UltimateHeroes
dotnet build test/ApiTestPlugin.cs
```

**ODER:** Kopiere den Code in dein Hauptprojekt und kompiliere es mit.

### **Schritt 3: Plugin auf Server laden**

1. Kopiere die kompilierte DLL in den CS2 Plugin-Ordner
2. Starte den Server
3. Schau in die Server-Konsole

### **Schritt 4: Ergebnisse prüfen**

Das Plugin testet:
- ✅ **Reflection:** Prüft ob Properties via Reflection existieren
- ✅ **GetProperty Method:** Prüft ob `GetProperty<T>()` Methode existiert
- ✅ **ConVars:** Prüft ob ConVars verfügbar sind
- ✅ **Methods:** Prüft ob Methoden wie `SetCollisionGroup()` existieren

## 📊 **Was das Plugin testet:**

### **1. Weapon Properties:**
- `m_flSpread` - Weapon Spread
- `m_flNextPrimaryAttack` - Fire Rate

### **2. Player Pawn Properties:**
- `m_flJumpPower` - Jump Height
- `m_flAirControl` - Air Control
- `m_bPlayFootstepSounds` - Footstep Sounds
- `m_CollisionGroup` - Collision Group

### **3. Methods:**
- `SetCollisionGroup()` - Collision Control

### **4. ConVars:**
- `weapon_accuracy_nospread` - Weapon Spread (global)
- `sv_jump_impulse` - Jump Height (global)
- `sv_footsteps` - Footstep Sounds (global)

## 📝 **Erwartete Ausgabe:**

```
[API Test] === Test Run #1 ===
[API Test] Testing weapon properties for player 7656119...
[API Test] ✅ Weapon Spread (m_flSpread) EXISTS via Reflection
  - Type: Single
  - Value: 0.5
[API Test] ✅ Weapon Spread (m_flSpread) ACCESSIBLE via GetProperty<T>
  - Value: 0.5
[API Test] ❌ Fire Rate (m_flNextPrimaryAttack) NOT FOUND via Reflection
[API Test] ❌ GetProperty method NOT FOUND for Fire Rate
...
```

## 🎯 **Nach dem Test:**

1. **Wenn APIs verfügbar sind:**
   - Implementiere die Features korrekt
   - Dokumentiere die APIs in `CS2_API_REFERENCE.md`

2. **Wenn APIs NICHT verfügbar sind:**
   - Implementiere Workarounds (nur wenn nötig)
   - Oder deaktiviere Features
   - Dokumentiere in `TODO_IMPLEMENTATION_STATUS.md`

## 🔍 **Alternative: GitHub Code prüfen**

Falls du den CounterStrikeSharp Source Code hast:
1. Prüfe ob `GetProperty`/`SetProperty` im Code existiert
2. Prüfe die Examples im GitHub Repository
3. Prüfe Issues/Discussions für Property Access

**GitHub:** https://github.com/roflmuffin/CounterStrikeSharp

---

**FAZIT:** Die APIs sind nicht dokumentiert, aber das Test-Plugin wird zeigen, ob sie verfügbar sind!
