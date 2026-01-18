# 🛒 Shop Items & Slot Expansion: Brainstorming

## 📊 **Aktuelles System**

### **Status Quo:**
- **Build Slots**: 3 Active + 1 Ultimate + 2 Passive (fest)
- **Power Budget**: 150 (fest)
- **Keine Shop Items**
- **Keine Slot-Erweiterungen**

---

## 💡 **Shop Items: Sollten sie in Builds?**

### **Option A: Shop Items = Separate System (NICHT in Builds)**

**Design:**
- Shop Items sind **temporäre Match-Items** (wie CS2 Economy)
- Kaufe Items **während des Matches** mit Geld/XP
- Items sind **nicht Teil des Builds**
- Items sind **nur für dieses Match** gültig

**Vorteile:**
✅ Klare Trennung: Build = permanent, Items = temporär
✅ Dynamisches Gameplay (kaufe Items je nach Situation)
✅ Keine Komplexität in Build-System
✅ Shop Items können Match-spezifisch sein (z.B. "Extra Armor", "Grenade")

**Nachteile:**
❌ Items sind nicht Teil der Build-Strategie
❌ Keine langfristige Progression mit Items

**Beispiele:**
- "Extra Armor" (+50 Armor für dieses Match)
- "Grenade Pack" (3 zusätzliche Granaten)
- "Speed Boost" (+10% Movement Speed)
- "Damage Boost" (+5% Damage)

---

### **Option B: Shop Items = Teil des Builds**

**Design:**
- Shop Items können **permanent gekauft** werden
- Items werden **in Builds gespeichert**
- Items haben **eigene Slots** (z.B. 2-3 Item Slots)
- Items sind **permanent** (nicht nur für ein Match)

**Vorteile:**
✅ Items sind Teil der Build-Strategie
✅ Langfristige Progression
✅ Mehr Customization

**Nachteile:**
❌ Mehr Komplexität im Build-System
❌ Power Budget muss angepasst werden
❌ Items müssen balanced werden

**Beispiele:**
- "Armor Upgrade" (permanent +25 Armor)
- "Cooldown Reduction" (-10% Cooldown für alle Skills)
- "Damage Amplifier" (+10% Damage)
- "Health Boost" (+20 HP)

---

### **Option C: Hybrid (Shop Items + Match Items)**

**Design:**
- **Permanent Items**: Teil des Builds (2-3 Slots)
- **Match Items**: Temporär, kaufe während Match (separates System)

**Vorteile:**
✅ Beste aus beiden Welten
✅ Strategische Build-Items + dynamische Match-Items

**Nachteile:**
❌ Sehr komplex
❌ Zwei verschiedene Systeme zu balancen

---

## 🎯 **Empfehlung: Option A (Shop Items = Separate System)**

**Warum?**
1. **Klarheit**: Build = permanente Skills, Items = temporäre Match-Buffs
2. **Dynamik**: Items können Match-spezifisch gekauft werden
3. **Einfachheit**: Keine Komplexität im Build-System
4. **CS2-Style**: Ähnlich wie CS2 Economy System

**Shop Items sollten:**
- Während Match gekauft werden (mit Geld/XP)
- Nur für dieses Match gültig sein
- Nicht in Builds gespeichert werden
- Separate UI haben (Shop Menu)

---

## 🔓 **Slot Expansion: Perks/Talents die Slots erhöhen**

### **Konzept:**
Talents/Perks können **Slot-Erweiterungen** freischalten:

**Beispiele:**
- **Talent**: "Extra Active Slot" → 3 Active → 4 Active
- **Talent**: "Dual Ultimate" → 1 Ultimate → 2 Ultimates
- **Talent**: "Passive Master" → 2 Passive → 3 Passive
- **Talent**: "Build Flexibility" → +1 Slot in jeder Kategorie

### **Design Optionen:**

#### **Option 1: Talent-basierte Slot-Erweiterungen**

**Design:**
- Talents können **Slot-Modifikatoren** freischalten
- Modifikatoren werden **in BuildValidator** berücksichtigt
- Max Slots werden **dynamisch** berechnet

**Beispiel:**
```csharp
// Talent: "Extra Active Slot"
MaxActiveSlots = 3 + GetTalentBonus("extra_active_slot"); // 3 + 1 = 4

// Talent: "Dual Ultimate"
MaxUltimateSlots = 1 + GetTalentBonus("dual_ultimate"); // 1 + 1 = 2
```

**Vorteile:**
✅ Langfristige Progression
✅ Strategische Entscheidungen (welche Slots erweitern?)
✅ Flexibles System

**Nachteile:**
❌ Komplexität in BuildValidator
❌ Balance könnte schwierig sein

---

#### **Option 2: Level-basierte Slot-Erweiterungen**

**Design:**
- Slots werden **automatisch** bei bestimmten Levels freigeschaltet
- Keine Talent-Entscheidung nötig
- Einfacher zu balancen

**Beispiel:**
- Level 10: +1 Active Slot (3 → 4)
- Level 20: +1 Ultimate Slot (1 → 2)
- Level 30: +1 Passive Slot (2 → 3)

**Vorteile:**
✅ Einfach zu implementieren
✅ Klare Progression
✅ Keine Balance-Probleme

**Nachteile:**
❌ Weniger strategische Entscheidungen
❌ Alle Spieler haben gleiche Slots

---

#### **Option 3: Hybrid (Talents + Level)**

**Design:**
- **Base Slots**: Level-basiert (automatisch)
- **Bonus Slots**: Talent-basiert (Wahl)

**Beispiel:**
- Level 10: +1 Active Slot (automatisch)
- Talent "Dual Ultimate": +1 Ultimate Slot (Wahl)
- Talent "Passive Master": +1 Passive Slot (Wahl)

**Vorteile:**
✅ Beste aus beiden Welten
✅ Automatische Progression + strategische Wahl

**Nachteile:**
❌ Komplexität

---

## 🎯 **Empfehlung: Option 3 (Hybrid)**

**Warum?**
1. **Automatische Progression**: Level-basierte Slots geben klare Ziele
2. **Strategische Wahl**: Talents ermöglichen individuelle Builds
3. **Balance**: Base Slots sind sicher, Bonus Slots sind optional

**Implementierung:**
```csharp
public class BuildSlotLimits
{
    // Base Slots (Level-basiert)
    public int MaxActiveSlots { get; set; } = 3;
    public int MaxUltimateSlots { get; set; } = 1;
    public int MaxPassiveSlots { get; set; } = 2;
    
    // Bonus Slots (Talent-basiert)
    public int BonusActiveSlots { get; set; } = 0;
    public int BonusUltimateSlots { get; set; } = 0;
    public int BonusPassiveSlots { get; set; } = 0;
    
    // Total Slots
    public int TotalActiveSlots => MaxActiveSlots + BonusActiveSlots;
    public int TotalUltimateSlots => MaxUltimateSlots + BonusUltimateSlots;
    public int TotalPassiveSlots => MaxPassiveSlots + BonusPassiveSlots;
}
```

**Talent Beispiele:**
- **"Extra Active Slot"** (Combat Tree, Level 5): +1 Active Slot
- **"Dual Ultimate"** (Combat Tree, Level 10): +1 Ultimate Slot
- **"Passive Master"** (Utility Tree, Level 7): +1 Passive Slot
- **"Build Flexibility"** (Utility Tree, Level 15): +1 Slot in jeder Kategorie

---

## 📐 **Finales Design (Empfehlung)**

### **Shop Items:**
- **Separates System** (nicht in Builds)
- **Temporäre Match-Items** (nur für dieses Match)
- **Kaufe während Match** (mit Geld/XP)
- **Separate UI** (Shop Menu)

### **Slot Expansion:**
- **Base Slots**: Level-basiert (automatisch)
  - Level 10: +1 Active Slot
  - Level 20: +1 Ultimate Slot
  - Level 30: +1 Passive Slot
- **Bonus Slots**: Talent-basiert (Wahl)
  - Talent "Extra Active Slot": +1 Active
  - Talent "Dual Ultimate": +1 Ultimate
  - Talent "Passive Master": +1 Passive
- **BuildValidator**: Berücksichtigt dynamische Slot-Limits

### **Power Budget:**
- **Base Budget**: 150 (für 6 Skills)
- **Mit Slot-Erweiterungen**: Budget erhöht sich proportional
  - +1 Active Slot: +25 Budget
  - +1 Ultimate Slot: +40 Budget
  - +1 Passive Slot: +15 Budget

---

## ❓ **Fragen:**

1. **Shop Items**: Sollen sie in Builds oder separate sein?
2. **Slot Expansion**: Level-basiert, Talent-basiert, oder Hybrid?
3. **Power Budget**: Soll es sich mit Slot-Erweiterungen erhöhen?
4. **Max Limits**: Gibt es Max-Limits für Slots? (z.B. max 5 Active, max 2 Ultimate)

**Meine Empfehlung:**
- Shop Items = Separate System (Option A)
- Slot Expansion = Hybrid (Option 3)
- Power Budget = Proportional erhöhen
- Max Limits = Ja (z.B. max 5 Active, max 2 Ultimate, max 4 Passive)
