# 🎯 Skill Slot Design: Brainstorming & Empfehlung

## 📊 **Aktuelles System**

### **Status Quo:**
- **3 Skill Slots** für alle Skill-Typen (Active, Passive, Ultimate)
- **Passive Skills** können in Builds sein (z.B. HealingAura mit PowerWeight 15)
- **Hero Passives** sind automatisch (PowerWeight 0, nicht in Slots)
- **BuildValidator** prüft:
  - Max 3 Skills total
  - Max 1 Ultimate
  - Tag-based Limits (Mobility: 2, CC: 2, Stealth: 1, etc.)
  - Power Budget (100 max)

### **Problem:**
- Passive Skills "verschwenden" einen Slot, obwohl sie nicht aktiviert werden müssen
- HealingAura nimmt einen Slot weg, obwohl sie automatisch läuft
- Spieler könnten lieber 3 Active Skills haben statt 2 Active + 1 Passive

---

## 💡 **Option 1: Getrennte Slot-Systeme**

### **Design:**
```
Build Structure:
- Hero: 1 (automatisch)
- Active Slots: 3 (nur Active/Ultimate)
- Passive Slots: 2 (nur Passive)
- Total: 3 Active + 2 Passive = 5 Skills
```

### **Vorteile:**
✅ Klare Trennung zwischen Active und Passive
✅ Passive "verschwenden" keine Active Slots
✅ Mehr Flexibilität (3 Active + 2 Passive möglich)
✅ Klareres UI (Active Skills vs Passive Skills)

### **Nachteile:**
❌ Mehr Komplexität (2 verschiedene Slot-Typen)
❌ Power Budget könnte aus dem Ruder laufen (5 Skills statt 3)
❌ BuildValidator muss angepasst werden
❌ UI muss 2 verschiedene Bereiche zeigen

### **Power Budget Beispiel:**
- Vanguard (30) + Fireball (25) + Blink (20) + Stealth (30) + HealingAura (15) + ArmorPassive (0) = 120/100 ❌
- Müsste Power Budget erhöhen oder Passive Weight reduzieren

---

## 💡 **Option 2: Passive als "Bonus" (Empfohlen!)**

### **Design:**
```
Build Structure:
- Hero: 1 (automatisch)
- Active Slots: 3 (nur Active/Ultimate)
- Passive Slots: 1-2 (separat, zählen nicht als "Active Slot")
- Total: 3 Active + 1-2 Passive
```

### **Regeln:**
- **Active Slots**: Max 3 (nur Active/Ultimate Skills)
- **Passive Slots**: Max 2 (nur Passive Skills, optional)
- **Power Budget**: Beide zählen, aber Passive haben niedrigere Weights
- **BuildValidator**: 
  - Prüft Active Slots separat (Max 3)
  - Prüft Passive Slots separat (Max 2)
  - Prüft Power Budget für alle zusammen

### **Vorteile:**
✅ Passive nehmen keine Active Slots weg
✅ Flexibel: 3 Active + 0-2 Passive
✅ Klare Trennung im UI
✅ Power Budget bleibt kontrollierbar
✅ BuildValidator kann beide separat validieren

### **Nachteile:**
❌ Etwas komplexer als aktuell
❌ BuildValidator muss erweitert werden
❌ UI muss 2 Bereiche zeigen

### **Power Budget Beispiel:**
- Vanguard (30) + Fireball (25) + Blink (20) + Stealth (30) = 105/100 ❌
- Vanguard (30) + Fireball (25) + Blink (20) + HealingAura (15) = 90/100 ✅
- Vanguard (30) + Fireball (25) + Blink (20) + HealingAura (15) + ArmorPassive (0) = 90/100 ✅

---

## 💡 **Option 3: Passive zählen nicht als Slot (Einfachste Lösung)**

### **Design:**
```
Build Structure:
- Hero: 1 (automatisch)
- Active Slots: 3 (nur Active/Ultimate)
- Passive: Unbegrenzt (nur Power Budget Limit)
- Total: 3 Active + X Passive (Power Budget entscheidet)
```

### **Regeln:**
- **Active Slots**: Max 3 (nur Active/Ultimate)
- **Passive Skills**: Kein Slot-Limit, nur Power Budget
- **BuildValidator**:
  - Max 3 Active/Ultimate Skills
  - Passive Skills zählen nicht für Slot-Limit
  - Power Budget prüft alle Skills

### **Vorteile:**
✅ Sehr einfach zu implementieren
✅ Passive "verschwenden" keine Slots
✅ Flexibel: Viele Passive möglich (wenn Power Budget erlaubt)
✅ Minimal Änderungen am Code

### **Nachteile:**
❌ Könnte zu viele Passive erlauben (Balance-Probleme)
❌ Power Budget könnte ausgenutzt werden (nur Passive = OP?)
❌ Weniger klare Trennung

### **Power Budget Beispiel:**
- Vanguard (30) + Fireball (25) + Blink (20) + HealingAura (15) + ArmorPassive (0) + SilentFootsteps (0) = 90/100 ✅
- Problem: Zu viele Passives könnten OP sein

---

## 💡 **Option 4: Hybrid (Aktuell + Verbesserung)**

### **Design:**
```
Build Structure:
- Hero: 1 (automatisch)
- Skill Slots: 3 (Active/Passive/Ultimate gemischt)
- Passive Limit: Max 1 Passive Skill pro Build
- Total: 3 Skills (max 1 Passive)
```

### **Regeln:**
- **Skill Slots**: Max 3 (können gemischt sein)
- **Passive Limit**: Max 1 Passive Skill pro Build
- **BuildValidator**:
  - Max 3 Skills total
  - Max 1 Passive Skill
  - Max 1 Ultimate Skill
  - Tag-based Limits

### **Vorteile:**
✅ Minimal Änderungen (nur 1 Regel hinzufügen)
✅ Verhindert "Passive Spam"
✅ Flexibel: 2 Active + 1 Passive oder 3 Active
✅ Einfach zu verstehen

### **Nachteile:**
❌ Passive nimmt immer noch einen Slot weg
❌ Weniger flexibel als Option 2

---

## 🎯 **Empfehlung: Option 2 (Passive als "Bonus")**

### **Warum Option 2?**

1. **Klarheit**: Active und Passive sind klar getrennt
2. **Flexibilität**: 3 Active + 0-2 Passive möglich
3. **Balance**: Power Budget bleibt kontrollierbar
4. **UI-Freundlich**: 2 Bereiche im Build Menu
5. **Zukunftssicher**: Kann später erweitert werden

### **Implementierung:**

```csharp
public class Build
{
    public string HeroCoreId { get; set; }
    public List<string> ActiveSkillIds { get; set; } = new(); // Max 3
    public List<string> PassiveSkillIds { get; set; } = new(); // Max 2
    // ... rest
}
```

### **BuildValidator Anpassung:**

```csharp
// Active Slots Check
if (activeSkills.Count > 3)
{
    result.Errors.Add("Too many active skills: {activeSkills.Count}/3");
}

// Passive Slots Check
if (passiveSkills.Count > 2)
{
    result.Errors.Add("Too many passive skills: {passiveSkills.Count}/2");
}

// Power Budget Check (alle zusammen)
var totalPower = heroCore.PowerWeight + 
                 activeSkills.Sum(s => s.PowerWeight) + 
                 passiveSkills.Sum(s => s.PowerWeight);
```

### **UI Anpassung:**

```
Build Menu:
┌─────────────────────────┐
│ Hero: [Vanguard]        │
│                         │
│ Active Skills (0/3):    │
│ [Slot 1] [Slot 2] [Slot 3]│
│                         │
│ Passive Skills (0/2):    │
│ [Passive 1] [Passive 2] │
│                         │
│ Power: 30/100           │
└─────────────────────────┘
```

---

## 📝 **Alternative: Option 3 (Einfachste)**

Wenn du **schnell** eine Lösung willst ohne große Änderungen:

- **Nur BuildValidator anpassen**: Passive Skills zählen nicht für Slot-Limit
- **Max 3 Active/Ultimate Skills**
- **Passive Skills**: Unbegrenzt (nur Power Budget)
- **Minimal Code-Änderungen**

---

## ❓ **Frage an dich:**

Welche Option gefällt dir am besten?

1. **Option 2** (Passive als Bonus) - Empfohlen, aber mehr Arbeit
2. **Option 3** (Passive zählen nicht) - Einfach, aber könnte Balance-Probleme geben
3. **Option 4** (Max 1 Passive) - Minimal Änderungen, gute Balance
4. **Aktuelles System behalten** - Alles bleibt wie es ist

**Meine Empfehlung: Option 2** für langfristige Flexibilität, oder **Option 4** für schnelle Lösung.
