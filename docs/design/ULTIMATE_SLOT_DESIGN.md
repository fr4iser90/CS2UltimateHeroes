# ⚡ Ultimate Slot Design: Brainstorming

## 📊 **Aktuelles System**

### **Status Quo:**
- **Ultimate** ist ein `SkillType` (wie Active, Passive)
- **Ultimate** zählt als **Active Skill** in den 3 Slots
- **Max 1 Ultimate** erlaubt pro Build
- **Ultimate Power Weight**: 40 (sehr hoch)
- **Ultimate Cooldown**: 60s (sehr lang)

### **Beispiel:**
- Build: Fireball (25) + Blink (20) + Teleport (40) = 85/100 ✅
- Problem: Ultimate nimmt einen "normalen" Active Slot weg

---

## 💡 **Option A: Ultimate = Teil von Active Slots (Aktuell)**

### **Design:**
```
Build Structure:
- Hero: 1
- Active Slots: 3 (Active + Ultimate gemischt)
- Passive Slots: 2
- Total: 3 Active/Ultimate + 2 Passive
```

### **Regeln:**
- Max 3 Active/Ultimate Skills (gemischt)
- Max 1 Ultimate Skill
- Max 2 Passive Skills

### **Vorteile:**
✅ Flexibel: 3 Active, oder 2 Active + 1 Ultimate
✅ Einfach: Ultimate ist wie ein starker Active Skill
✅ Power Budget bleibt kontrollierbar

### **Nachteile:**
❌ Ultimate "verschwendet" einen Active Slot
❌ Spieler könnten lieber 3 Active Skills haben
❌ Weniger strategische Entscheidung (Ultimate vs 3 Active)

### **Beispiele:**
- 3 Active: Fireball + Blink + Stealth = 75/100 ✅
- 2 Active + 1 Ultimate: Fireball + Blink + Teleport = 85/100 ✅
- 1 Active + 1 Ultimate: Fireball + Teleport = 65/100 ✅

---

## 💡 **Option B: Ultimate = Separater Slot (Empfohlen!)**

### **Design:**
```
Build Structure:
- Hero: 1
- Active Slots: 3 (nur Active Skills)
- Ultimate Slot: 1 (nur Ultimate Skills)
- Passive Slots: 2
- Total: 3 Active + 1 Ultimate + 2 Passive = 6 Skills
```

### **Regeln:**
- Max 3 Active Skills (kein Ultimate)
- Max 1 Ultimate Skill (separater Slot)
- Max 2 Passive Skills

### **Vorteile:**
✅ Ultimate nimmt keinen Active Slot weg
✅ Klare Trennung: Active vs Ultimate
✅ Mehr strategische Entscheidung (welches Ultimate?)
✅ Mehr Flexibilität: 3 Active + 1 Ultimate möglich
✅ UI-freundlich: Separater Ultimate Slot im HUD

### **Nachteile:**
❌ Mehr Komplexität (3 verschiedene Slot-Typen)
❌ Power Budget muss angepasst werden (6 Skills statt 5)
❌ Database Schema muss erweitert werden

### **Power Budget Anpassung:**
- Aktuell: 100 (für 3 Skills)
- Mit Ultimate Slot: 120-150 (für 6 Skills)
- Oder: Ultimate zählt nicht für Power Budget (Bonus)

### **Beispiele:**
- 3 Active + 1 Ultimate: Fireball + Blink + Stealth + Teleport = 115/120 ✅
- 2 Active + 1 Ultimate: Fireball + Blink + Teleport = 85/120 ✅
- 3 Active + 0 Ultimate: Fireball + Blink + Stealth = 75/120 ✅

---

## 💡 **Option C: Ultimate = Optional Bonus**

### **Design:**
```
Build Structure:
- Hero: 1
- Active Slots: 3 (nur Active)
- Ultimate Slot: 1 (optional, zählt nicht für Power Budget)
- Passive Slots: 2
```

### **Regeln:**
- Max 3 Active Skills
- Max 1 Ultimate Skill (optional, kein Power Weight)
- Max 2 Passive Skills
- Ultimate ist "Bonus" - kein Power Budget

### **Vorteile:**
✅ Ultimate ist wirklich "Ultimate" (Bonus)
✅ Power Budget bleibt bei 100-120
✅ Flexibel: Jeder kann ein Ultimate haben

### **Nachteile:**
❌ Balance könnte problematisch sein (Ultimate zu stark?)
❌ Power Budget System wird inkonsistent

---

## 🎯 **Empfehlung: Option B (Ultimate = Separater Slot)**

### **Warum?**

1. **Klarheit**: Ultimate ist klar getrennt von Active Skills
2. **Strategie**: Spieler müssen sich entscheiden: Welches Ultimate?
3. **Flexibilität**: 3 Active + 1 Ultimate möglich
4. **UI**: Separater Ultimate Slot im HUD (bereits implementiert!)
5. **Balance**: Ultimate hat hohen Power Weight (40), sollte extra Slot haben

### **Power Budget Anpassung:**

**Option B1: Erhöhtes Power Budget**
- Max Power Budget: **120-150** (für 6 Skills)
- Hero: 30
- Active Skills: 3 × 25-30 = 75-90
- Ultimate: 40
- Passive: 2 × 15 = 30
- **Total: 175-190** → Budget auf 150-180 erhöhen

**Option B2: Ultimate zählt weniger**
- Max Power Budget: **120** (für 6 Skills)
- Ultimate Power Weight reduzieren: 40 → 20-25
- Oder: Ultimate zählt nur 50% für Power Budget

**Option B3: Ultimate = Bonus (kein Power Weight)**
- Max Power Budget: **100** (für 5 Skills: 3 Active + 2 Passive)
- Ultimate: 0 Power Weight (Bonus)
- Problem: Balance könnte aus dem Ruder laufen

### **Empfehlung: Option B1 (Budget 150)**

```
Power Budget: 150
- Hero: 30
- 3 Active: ~75 (25-30 each)
- 1 Ultimate: 40
- 2 Passive: ~30 (15 each)
- Total: ~175 → Budget 150-180
```

---

## 📐 **Finales Design (Option B + B1)**

### **Build Structure:**
```csharp
public class Build
{
    public string HeroCoreId { get; set; }
    public List<string> ActiveSkillIds { get; set; } = new(); // Max 3
    public string? UltimateSkillId { get; set; } = null;      // Max 1 (optional)
    public List<string> PassiveSkillIds { get; set; } = new(); // Max 2
}
```

### **BuildValidator:**
```csharp
// Active Slots
if (activeSkills.Count > 3)
    Error("Too many active skills: {count}/3");

// Ultimate Slot
if (ultimateSkill != null && ultimateSkills.Count > 1)
    Error("Only 1 ultimate skill allowed");

// Passive Slots
if (passiveSkills.Count > 2)
    Error("Too many passive skills: {count}/2");

// Power Budget (150)
var totalPower = heroCore.PowerWeight + 
                 activeSkills.Sum(s => s.PowerWeight) + 
                 (ultimateSkill?.PowerWeight ?? 0) +
                 passiveSkills.Sum(s => s.PowerWeight);
if (totalPower > 150)
    Error("Power Budget exceeded: {total}/150");
```

### **UI Design:**
```
Build Menu:
┌─────────────────────────┐
│ Hero: [Vanguard]        │
│                         │
│ Active Skills (0/3):    │
│ [Slot 1] [Slot 2] [Slot 3]│
│                         │
│ Ultimate (0/1):         │
│ [Ultimate Slot]         │
│                         │
│ Passive Skills (0/2):   │
│ [Passive 1] [Passive 2] │
│                         │
│ Power: 30/150           │
└─────────────────────────┘
```

### **HUD Design:**
```
[1] Fireball    [2] Blink    [3] Stealth
[ULTIMATE] Teleport
```

---

## ❓ **Frage an dich:**

**Soll Ultimate einen separaten Slot bekommen?**

**Ja (Option B):**
- ✅ Klarere Trennung
- ✅ Mehr Flexibilität (3 Active + 1 Ultimate)
- ✅ Strategische Entscheidung
- ❌ Power Budget muss erhöht werden (150-180)
- ❌ Mehr Komplexität

**Nein (Option A - aktuell):**
- ✅ Einfacher (Ultimate = starker Active)
- ✅ Power Budget bleibt bei 100-120
- ✅ Weniger Komplexität
- ❌ Ultimate nimmt Active Slot weg

**Meine Empfehlung: Option B** (Ultimate = Separater Slot) für bessere Strategie und Flexibilität!
