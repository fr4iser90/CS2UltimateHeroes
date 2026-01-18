# 📋 Content Overview: Ultimate Heroes

## 🎭 **Heroes (3)**

### 1. **Vanguard** (`vanguard`)
- **Type**: Tank/Defense
- **Power Weight**: 30
- **Description**: Ein robuster Tank-Hero mit starken Defense-Fähigkeiten
- **Passive Skills**: 
  - `armor_per_kill_passive` - +5 Armor nach jedem Kill
- **Hero Identity**:
  - +10% Defense Skills
  - +5% Support Skills
  - +20% Shield Duration

### 2. **Phantom** (`phantom`)
- **Type**: Stealth/Mobility
- **Power Weight**: 30
- **Description**: Ein geschickter Stealth-Hero mit starken Mobility-Fähigkeiten
- **Passive Skills**:
  - `silent_footsteps_passive` - Lautlose Schritte
- **Hero Identity**:
  - -10% Cooldown für Mobility Skills
  - +10% Stealth Skills
  - -10% Cooldown für Active Skills
  - +15% Backstab Damage

### 3. **Engineer** (`engineer`)
- **Type**: Tech/Utility
- **Power Weight**: 30
- **Description**: Ein technischer Hero mit starken Utility-Fähigkeiten
- **Passive Skills**: 
  - *(Keine Passives aktuell - TODO: MiniSentryPassive, UtilityCooldownReductionPassive)*
- **Hero Identity**:
  - +25% Utility Skills Range
  - +10% Area Skills
  - +30% Sentry Damage

---

## ⚡ **Active Skills (4)**

### 1. **Fireball** (`fireball`)
- **Type**: Active
- **Power Weight**: 25
- **Cooldown**: 10s
- **Max Level**: 5
- **Tags**: Damage, Area
- **Description**: Wirft einen Feuerball in Blickrichtung
- **Stats**:
  - Base Damage: 30 (+5 per level)
  - Base Radius: 150 (+20 per level)
  - Range: 500 units
- **Effects**: 
  - Area Damage in Radius
  - Explosion Particles

### 2. **Blink** (`blink`)
- **Type**: Active
- **Power Weight**: 20
- **Cooldown**: 5s
- **Max Level**: 5
- **Tags**: Mobility
- **Description**: Teleportiert dich in Blickrichtung
- **Stats**:
  - Base Range: 300 (+50 per level)
- **Effects**:
  - Instant Teleport
  - Particles at origin and destination

### 3. **Stealth** (`stealth`)
- **Type**: Active
- **Power Weight**: 30
- **Cooldown**: 15s
- **Max Level**: 5
- **Tags**: Stealth, Utility
- **Description**: Macht dich unsichtbar für kurze Zeit
- **Stats**:
  - Base Duration: 5s (+1s per level)
- **Effects**:
  - Invisibility Effect
  - +Movement Speed während Invisibility

### 4. **Teleport** (`teleport`)
- **Type**: Ultimate
- **Power Weight**: 40
- **Cooldown**: 60s
- **Max Level**: 3
- **Tags**: Mobility, Ultimate
- **Description**: Teleportiert dich zu einer beliebigen Position auf der Map
- **Stats**:
  - Max Range: 2000 units
- **Effects**:
  - Long-range Teleport
  - Particles at origin and destination

---

## 🛡️ **Passive Skills (2)**

### 1. **Armor per Kill** (`armor_per_kill_passive`)
- **Type**: Passive
- **Power Weight**: 0 (Hero Passive)
- **Max Level**: 1
- **Tags**: Defense
- **Description**: Gibt +5 Armor nach jedem Kill
- **Effects**:
  - +5 Armor pro Kill (max 100 Armor)
- **Used by**: Vanguard

### 2. **Silent Footsteps** (`silent_footsteps_passive`)
- **Type**: Passive
- **Power Weight**: 0 (Hero Passive)
- **Max Level**: 1
- **Tags**: Stealth, Utility
- **Description**: Deine Schritte sind lautlos
- **Effects**:
  - Disables footstep sounds
- **Used by**: Phantom

---

## 💚 **Support Skills (1)**

### 1. **Healing Aura** (`healing_aura`)
- **Type**: Passive (Support)
- **Power Weight**: 15
- **Max Level**: 5
- **Tags**: Support, Area
- **Description**: Heilt dich und Teammates in der Nähe
- **Stats**:
  - Base Heal: 2 HP/s (+0.5 per level)
  - Base Radius: 200 (+30 per level)
- **Effects**:
  - Periodic healing in radius
  - Healing particles

---

## 🌳 **Talent Trees (3 Trees, 15 Talents)**

### **Combat Tree** (6 Talents)
1. **Headshot Damage** (`combat_headshot_damage`)
   - Row 1, Col 1
   - Max Level: 5
   - Effect: +5% Headshot Damage per level

2. **Recoil Control** (`combat_recoil_control`)
   - Row 1, Col 2
   - Max Level: 5
   - Effect: -5% Recoil per level

3. **Armor Penetration** (`combat_armor_penetration`)
   - Row 1, Col 3
   - Max Level: 5
   - Effect: +2% Armor Pen per level

4. **Damage per Kill** (`combat_damage_per_kill`)
   - Row 2, Col 1
   - Max Level: 5
   - Prerequisites: Headshot Damage
   - Effect: +1 Damage per kill (stacks)

5. **Reload Speed** (`combat_reload_speed`)
   - Row 2, Col 2
   - Max Level: 5
   - Prerequisites: Recoil Control
   - Effect: +10% Reload Speed per level

6. **Weapon Accuracy** (`combat_weapon_accuracy`)
   - Row 2, Col 3
   - Max Level: 5
   - Prerequisites: Armor Penetration
   - Effect: +5% Accuracy per level

### **Utility Tree** (3 Talents)
1. **Extra Nade** (`utility_extra_nade`)
   - Row 1, Col 1
   - Max Level: 3
   - Effect: +1 Grenade per level

2. **Faster Plant** (`utility_faster_plant`)
   - Row 1, Col 2
   - Max Level: 5
   - Effect: -0.5s Plant Time per level

3. **Defuse Speed** (`utility_defuse_speed`)
   - Row 1, Col 3
   - Max Level: 5
   - Effect: +10% Defuse Speed per level

### **Movement Tree** (6 Talents)
1. **Air Control** (`movement_air_control`)
   - Row 1, Col 1
   - Max Level: 5
   - Effect: +10% Air Control per level

2. **Faster Ladder** (`movement_ladder_speed`)
   - Row 1, Col 2
   - Max Level: 5
   - Effect: +15% Ladder Speed per level

3. **Silent Drop** (`movement_silent_drop`)
   - Row 1, Col 3
   - Max Level: 1
   - Effect: Silent Drops

4. **Movement Speed** (`movement_speed`)
   - Row 2, Col 1
   - Max Level: 5
   - Prerequisites: Air Control
   - Effect: +5% Movement Speed per level ✅ (implementiert)

5. **Jump Height** (`movement_jump_height`)
   - Row 2, Col 2
   - Max Level: 5
   - Prerequisites: Faster Ladder
   - Effect: +10% Jump Height per level

6. **Fall Damage Reduction** (`movement_fall_damage_reduction`)
   - Row 2, Col 3
   - Max Level: 5
   - Prerequisites: Silent Drop
   - Effect: -20% Fall Damage per level

---

## 🤖 **Bot Build Presets (5)**

### 1. **DPS Build** (`dps`)
- Hero: Vanguard
- Skills: Fireball

### 2. **Mobility Build** (`mobility`)
- Hero: Phantom
- Skills: Blink, Teleport

### 3. **Stealth Build** (`stealth`)
- Hero: Phantom
- Skills: Stealth, SilentFootsteps

### 4. **Support Build** (`support`)
- Hero: Engineer
- Skills: HealingAura

### 5. **Balanced Build** (`balanced`)
- Hero: Vanguard
- Skills: Fireball, Blink

---

## 📊 **Zusammenfassung**

- **Heroes**: 3 (Vanguard, Phantom, Engineer)
- **Active Skills**: 4 (Fireball, Blink, Stealth, Teleport)
- **Passive Skills**: 2 (ArmorPerKill, SilentFootsteps)
- **Support Skills**: 1 (HealingAura)
- **Total Skills**: 7
- **Talent Trees**: 3 (Combat, Utility, Movement)
- **Total Talents**: 15
- **Bot Presets**: 5

---

## 🎯 **Power Budget System**

- **Max Power Budget**: 100
- **Hero Power Weight**: 30 (alle Heroes)
- **Skill Power Weights**:
  - Fireball: 25
  - Blink: 20
  - Stealth: 30
  - Teleport: 40 (Ultimate)
  - HealingAura: 15
  - Passives: 0 (Hero Passives)

**Beispiel Builds**:
- Vanguard + Fireball = 30 + 25 = 55/100 ✅
- Phantom + Blink + Stealth = 30 + 20 + 30 = 80/100 ✅
- Engineer + Teleport + HealingAura = 30 + 40 + 15 = 85/100 ✅

---

## 🔧 **Build Validator Rules**

- Max 1 Ultimate Skill
- Max 2 Mobility Skills
- Max 2 CrowdControl Skills
- Max 1 Stealth Skill
- Max 3 Damage Skills
- Max 3 Support Skills
- Max 2 Defense Skills

**Warnings**:
- CC + Stealth = +25% Cooldown Penalty
- 3+ Damage Skills = High Power Consumption
- Mobility + Stealth = Synergy Bonus
- Support + Defense = Tank Build Detected

---

## 📝 **Hinweise**

- **HealingAura** ist als Passive implementiert, aber zählt als Support Skill
- **SilentFootsteps** ist noch nicht vollständig implementiert (TODO: Disable footstep sounds)
- **Engineer** hat noch keine Passives (TODO: MiniSentryPassive, UtilityCooldownReductionPassive)
- Alle Skills haben **Mastery Tracking** (Kills, Uses, Damage, Escapes)
- Alle Talents haben **Level-Up Support** (Level 1-5, je nach Talent)

⚡ Neue Active Skills (Normal Skills)
🔥 Damage / Combat

Shockwave

Typ: Active

Tags: Damage, CrowdControl, Area

Effekt: Kegelförmige Druckwelle, knockt Gegner zurück

Synergie: Vanguard, Combat Tree

Piercing Shot

Tags: Damage

Effekt: Schuss durchdringt X Gegner

Scaling: Durchschläge + Damage pro Level

Grenade Toss

Tags: Damage, Area

Effekt: Verzögerte Explosion

Synergie: Utility Tree (Extra Nade)

🛡️ Defense / Tank

Energy Shield

Tags: Defense, Utility

Effekt: Temporärer Schild (HP oder % Damage Reduction)

Dauer skaliert mit Level

Taunt Pulse

Tags: Defense, CrowdControl

Effekt: Zwingt Gegner im Radius, dich anzugreifen

Vanguard-Core Skill

🧠 Utility / Control

EMP Pulse

Tags: Utility, CrowdControl

Effekt: Disable Skills / Sentries / Ultimates für X Sekunden

Engineer-Signature

Scanner Drone

Tags: Utility, Area

Effekt: Revealt Gegner im Radius

Counter zu Stealth

🏃 Mobility

Wall Dash

Tags: Mobility

Effekt: Dash entlang von Wänden

Grapple Hook

Tags: Mobility, Utility

Effekt: Zieh dich an Oberflächen oder Gegner heran

🛡️ Neue Passive Skills
🧱 Vanguard / Tank

Fortified Plating

Effekt: +X% Damage Reduction unter 50% HP

Shield on Assist

Effekt: Schild bei Assist (nicht Kill)

Fördert Teamplay

👻 Phantom / Stealth

Backstab Momentum

Effekt: Backstab reduziert Cooldowns

Fade on Kill

Effekt: Kurzzeit-Stealth nach Kill (0.5–1s)

🛠️ Engineer / Utility

Mini Sentry Passive ✅ (passt perfekt zu deinem TODO)

Effekt: Platziert automatisch Mini-Sentry nach Cooldown

Utility Cooldown Reduction

Effekt: -X% Cooldown auf Utility Skills

Overclock

Effekt: Skills werden stärker, kosten aber HP oder erzeugen Heat

🌿 Generic Passives

Life on Kill

Effekt: +HP pro Kill

Adaptive Armor

Effekt: Erhöht Armor gegen zuletzt erlittenen Schadenstyp

🌟 Neue Ultimate Skills
💥 Damage Ultimates

Meteor Strike

Tags: Damage, Area, Ultimate

Effekt: Verzögerter Map-Impact Einschlag

Counterplay durch Delay

Bullet Storm

Effekt: Massive Feuerrate + Infinite Ammo für X Sekunden

🛡️ Tank / Control Ultimates

Fortress Mode

Vanguard-Ultimate

Effekt:

+Armor

Immun gegen CC

Kein Sprint

Battle Standard

Effekt: Platziert Banner → Buffs für Allies im Radius

🧠 Utility / Tactical Ultimates

Time Dilation

Effekt: Gegner verlangsamt, Team normal

Extrem stark, gut mit hohem Cooldown

Global Scan

Effekt: Revealt alle Gegner kurzzeitig

👻 Stealth Ultimates

Shadow Realm

Effekt:

Vollständige Unsichtbarkeit

Keine Collision

Kein Schaden möglich (reines Movement)

Execution Mark

Effekt: Markierter Gegner nimmt extrem erhöhten Schaden