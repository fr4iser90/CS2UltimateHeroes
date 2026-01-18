# ✅ MVP Checklist: Ultimate Heroes

## 🎯 **Was ist bereits fertig?**

### ✅ **Domain Layer (10 Dateien)**
- Build.cs + BuildSlot.cs
- HeroCore.cs + Vanguard.cs
- SkillBase.cs + PassiveSkillBase.cs + ActiveSkillBase.cs
- Fireball.cs + ArmorPerKillPassive.cs
- UltimatePlayer.cs + RoleInfluence.cs

### ✅ **Infrastructure Layer (12 Dateien)**
- Database.cs + Schema.sql
- PlayerRepository.cs + BuildRepository.cs
- CooldownManager.cs
- EventSystem.cs + Event Handlers

### ✅ **Application Layer (9 Dateien)**
- PlayerService.cs
- HeroService.cs
- SkillService.cs
- BuildService.cs
- XpService.cs
- BuildValidator.cs

### ✅ **Presentation Layer (4 Dateien)**
- HeroMenu.cs
- BuildMenu.cs
- SkillMenu.cs
- Plugin Integration

---

## ❌ **Was fehlt noch für MVP?**

### **1. Commands zum Interagieren** 🔧 (KRITISCH)

**Aktuell:** Menus zeigen nur an, aber keine Commands zum Erstellen/Aktivieren

**Fehlt:**
- ❌ `!selecthero <hero_id>` - Hero auswählen
- ❌ `!createbuild <slot> <hero> <skill1> [skill2] [skill3] <name>` - Build erstellen
- ❌ `!activatebuild <slot>` - Build aktivieren
- ❌ `!use <skill_id>` - Skill aktivieren (für Active Skills)
- ❌ `!stats` - Player Stats anzeigen (Level, XP, etc.)

### **2. Event Hooks korrigieren** 🔌 (KRITISCH)

**Problem:**
- `OnClientConnect`/`OnClientDisconnect` verwenden möglicherweise falsche Signatur
- SteamID muss korrekt extrahiert werden

**Zu prüfen:**
- ❌ CounterStrikeSharp Event Signatures
- ❌ SteamID Extraction (SteamId64 vs SteamId2)

### **3. Database Path korrigieren** 💾 (KRITISCH)

**Problem:**
- Database Path ist hardcoded
- Muss relativ zum Plugin-Pfad sein

**Zu fixen:**
- ❌ Database Path sollte `ModuleDirectory` nutzen
- ❌ Schema.sql muss als Embedded Resource oder korrekter Pfad

### **4. Build Creation Flow** 🏗️ (WICHTIG)

**Aktuell:** BuildService kann Builds erstellen, aber keine Commands

**Fehlt:**
- ❌ Command Handler für Build Creation
- ❌ Validation Feedback für Spieler
- ❌ Error Messages für Spieler

### **5. Hero Selection Flow** 🎭 (WICHTIG)

**Aktuell:** HeroService kann Heroes setzen, aber keine Commands

**Fehlt:**
- ❌ Command zum Hero auswählen
- ❌ Hero wird nicht automatisch beim Spawn aktiviert
- ❌ Default Hero wird nicht gesetzt

### **6. Skill Activation** ⚡ (WICHTIG)

**Aktuell:** SkillService kann Skills aktivieren, aber keine Commands

**Fehlt:**
- ❌ Command zum Skill aktivieren
- ❌ Keybindings (optional, später)
- ❌ Cooldown Feedback

### **7. Testing & Debugging** 🐛 (NICHT KRITISCH, aber hilfreich)

**Fehlt:**
- ❌ `!uh_debug` - Debug Info
- ❌ `!uh_reload` - Reload Player Data
- ❌ Logging für wichtige Events

### **8. Mehr Heroes/Skills** 🎮 (OPTIONAL für MVP)

**Aktuell:** Nur 1 Hero (Vanguard), 1 Active Skill (Fireball), 1 Passive Skill

**Für Testing gut:**
- ⚠️ 2-3 Heroes (Phantom, Engineer)
- ⚠️ 3-5 Skills (Blink, Stealth, HealingAura, Teleport)

---

## 📋 **MVP Priorität**

### **🔥 KRITISCH (Muss rein für MVP)**
1. ✅ Commands zum Interagieren (!selecthero, !createbuild, !activatebuild, !use)
2. ✅ Event Hooks korrigieren (OnClientConnect/Disconnect)
3. ✅ Database Path korrigieren
4. ✅ Hero Selection Flow (Hero wird gesetzt und aktiviert)
5. ✅ Build Creation/Activation Commands

### **⚡ WICHTIG (Sollte rein)**
6. ✅ Skill Activation Command
7. ✅ Stats Command (!stats)
8. ✅ Error Handling & User Feedback

### **🎯 NICE TO HAVE (Kann später)**
9. ⚠️ Mehr Heroes/Skills (für Testing)
10. ⚠️ Debug Commands
11. ⚠️ Keybindings für Skills

---

## 🎯 **MVP Definition**

**Ein funktionierendes MVP sollte:**
- ✅ Spieler können Hero auswählen
- ✅ Spieler können Build erstellen
- ✅ Spieler können Build aktivieren
- ✅ Spieler können Skills aktivieren
- ✅ XP wird bei Kills vergeben
- ✅ Level-Ups funktionieren
- ✅ Daten werden gespeichert
- ✅ Menus zeigen Informationen

**Aktuell fehlt:**
- ❌ Commands zum Interagieren (nur Menus)
- ❌ Event Hooks funktionieren möglicherweise nicht richtig
- ❌ Database Path muss korrigiert werden

---

## 🚀 **Nächste Schritte**

1. **Commands hinzufügen** (2-3 Stunden)
2. **Event Hooks korrigieren** (30 Min)
3. **Database Path fixen** (30 Min)
4. **Testing** (1-2 Stunden)

**Total: ~4-6 Stunden für vollständiges MVP**
