# 🎮 Account Level vs Hero Level: Brainstorming

## 📊 **Aktuelles System**

### **Status Quo:**
- **Hero Level**: 1-40 (pro Hero)
- **XP**: Pro Hero getrennt
- **Talents**: Pro Hero getrennt
- **Builds**: Pro Hero getrennt

**Frage:** Soll es auch ein **Account Level** geben?

---

## 💡 **Option A: Nur Hero Level (Aktuell)**

### **Design:**
- Jeder Hero hat eigenes Level (1-40)
- Kein Account-Level
- Progression ist Hero-spezifisch

### **Vorteile:**
✅ Einfach: Keine zusätzliche Komplexität
✅ Klar: Jeder Hero ist eigenständig
✅ Flexibel: Spieler können verschiedene Heroes auf verschiedenen Levels haben

### **Nachteile:**
❌ Keine langfristige Account-Progression
❌ Keine Account-weiten Unlocks
❌ Prestige muss Hero-spezifisch sein

### **Beispiel:**
- Vanguard: Level 25
- Phantom: Level 15
- Engineer: Level 5
- → Kein Account-Level

---

## 💡 **Option B: Account Level + Hero Level (Hybrid)**

### **Design:**
- **Hero Level**: 1-40 (pro Hero, wie aktuell)
- **Account Level**: 1-∞ (über alle Heroes hinweg)
- **Account XP**: Summe aller Hero-XP oder separate Account-XP-Quellen

### **Vorteile:**
✅ Langfristige Progression über mehrere Heroes
✅ Account-weite Unlocks möglich
✅ Prestige kann Account-Level-basiert sein
✅ Kosmetische Belohnungen (Titles, Auren, etc.)
✅ "Veteran"-Status sichtbar

### **Nachteile:**
❌ Mehr Komplexität (2 Level-Systeme)
❌ Balance: Account-Level darf nicht zu mächtig sein
❌ UI: Muss beide Levels anzeigen

### **Beispiel:**
- Vanguard: Level 25
- Phantom: Level 15
- Engineer: Level 5
- **Account Level**: 45 (Summe oder separate XP)

---

## 💡 **Option C: Account Level als Master Level**

### **Design:**
- **Hero Level**: 1-40 (pro Hero)
- **Account Level**: Berechnet aus Hero-Levels (z.B. Durchschnitt oder Summe)
- **Keine separate Account-XP**

### **Vorteile:**
✅ Einfacher als Option B (keine separate XP)
✅ Automatisch berechnet
✅ Zeigt "Gesamt-Fortschritt"

### **Nachteile:**
❌ Weniger Kontrolle über Account-Level
❌ Kann nicht direkt "gegrindet" werden

### **Beispiel:**
- Vanguard: Level 25
- Phantom: Level 15
- Engineer: Level 5
- **Account Level**: 15 (Durchschnitt) oder 45 (Summe)

---

## 🎯 **Empfehlung: Option B (Account Level + Hero Level)**

### **Warum?**

1. **Langfristige Motivation**: Account-Level gibt langfristiges Ziel
2. **Account-Unlocks**: Kosmetische Belohnungen, Titles, etc.
3. **Prestige**: Kann Account-Level-basiert sein (z.B. Prestige bei Account Level 200)
4. **Community**: Spieler können Account-Level vergleichen
5. **Flexibilität**: Hero-Level bleibt für Gameplay, Account-Level für Meta-Progression

### **Design-Vorschlag:**

#### **Account Level System:**
- **Account XP**: Separate XP-Quellen (Match Completion, Daily Quests, etc.)
- **Account Level**: 1-∞ (kein Max, oder sehr hoch wie 1000+)
- **Account Unlocks**: 
  - Titles (z.B. "Veteran", "Master", "Legend")
  - Cosmetics (Auren, UI-Skins)
  - Prestige-Freischaltungen

#### **Hero Level System:**
- **Hero Level**: 1-40 (wie aktuell)
- **Hero XP**: Pro Hero getrennt
- **Hero Unlocks**: Talents, Slots, Builds (wie aktuell)

#### **Beziehung:**
- Account Level **beeinflusst NICHT** Hero-Level
- Account Level **beeinflusst NICHT** Gameplay-Power
- Account Level = **Meta-Progression** (Kosmetik, Prestige, etc.)

---

## 📐 **Konkretes Design (Option B)**

### **Account XP Quellen:**
- **Match Completion**: +10 Account XP (unabhängig von Hero-XP)
- **Hero Level Up**: +Account XP basierend auf Hero-Level (z.B. Level 10 → +50 Account XP)
- **Daily Quests**: +Account XP (später)
- **Achievements**: +Account XP (später)

### **Account Level Kurve:**
- **Level 1-50**: Schnell (100-200 XP pro Level)
- **Level 51-100**: Mittel (200-500 XP pro Level)
- **Level 101+**: Langsam (500+ XP pro Level)
- **Kein Max Level** (oder sehr hoch wie 1000+)

### **Account Unlocks (Beispiele):**
- **Level 10**: Title "Novice"
- **Level 25**: Title "Experienced"
- **Level 50**: Title "Veteran"
- **Level 100**: Title "Master"
- **Level 200**: Prestige freigeschaltet
- **Level 500**: Title "Legend"

### **UI Integration:**
```
Player HUD:
┌─────────────────────────┐
│ Hero: Vanguard (Lv.25)  │
│ Account: Lv.45          │
│ XP: 1250/2000           │
└─────────────────────────┘
```

---

## ❓ **Fragen:**

1. **Account Level**: Soll es ein Account Level geben?
2. **Account XP**: Separate XP-Quellen oder berechnet aus Hero-Levels?
3. **Account Unlocks**: Was soll Account Level freischalten?
4. **Prestige**: Soll Prestige Account-Level-basiert sein?

**Meine Empfehlung:**
- ✅ **Account Level**: Ja (Option B)
- ✅ **Account XP**: Separate Quellen (Match Completion, Hero Level Ups)
- ✅ **Account Unlocks**: Kosmetik, Titles, Prestige-Freischaltungen
- ✅ **Prestige**: Account Level 200+ freischaltet Prestige

**Warum?**
- Langfristige Motivation
- Keine Gameplay-Balance-Probleme (Account Level = Meta-Progression)
- Flexibel erweiterbar (Quests, Achievements, etc.)
