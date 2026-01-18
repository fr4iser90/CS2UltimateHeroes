# ✅ Core-Systeme Checkliste: Ultimate Heroes

## 📊 **Aktueller Stand**

### ✅ **Bereits vorhanden:**
1. ✅ **IHero Interface** - Hero Core Definition mit PowerWeight, PassiveSkills, HeroIdentity
2. ✅ **ISkill Interface** - Skill Definition mit PowerWeight, Tags, SkillType
3. ✅ **HeroIdentity** - TagModifiers, CooldownReduction, SpecialBonuses
4. ✅ **BuildValidator** - Power Budget Check + Basic Rules Engine
5. ✅ **Basic Plugin Structure** - UltimateHeroes.cs mit Commands

---

## ❌ **FEHLT NOCH - Core Domain Models**

### **1. Build Domain Model** 🏗️
```
❌ Domain/Builds/Build.cs
   - Build Entity (steamid, build_slot, hero_core_id, skill_ids[])
   - Build Name, IsActive, CreatedAt
   - Build Validation Logic

❌ Domain/Builds/BuildSlot.cs
   - Build Slot Management (3-5 Slots)
   - Slot Unlock Progression
```

### **2. Hero Core Implementations** 🎭
```
❌ Domain/Heroes/HeroCore.cs (Base Class)
   - Konkrete Hero-Implementierungen
   - Vanguard, Phantom, Engineer, etc.

❌ Domain/Heroes/ConcreteHeroes/
   - Vanguard.cs
   - Phantom.cs
   - Engineer.cs
   - (weitere Heroes)
```

### **3. Skill Implementations** ⚡
```
❌ Domain/Skills/SkillBase.cs (Base Class)
   - Basis-Implementierung für Skills

❌ Domain/Skills/ConcreteSkills/
   - Fireball.cs
   - Blink.cs
   - Stealth.cs
   - HealingAura.cs
   - (weitere Skills)
```

### **4. Talent System** 🌳
```
❌ Domain/Talents/TalentTree.cs
   - Talent Tree Structure
   - Combat/Utility/Movement Trees

❌ Domain/Talents/TalentNode.cs
   - Talent Node Definition
   - Prerequisites, Max Level

❌ Domain/Talents/PlayerTalents.cs
   - Player Talent Progress
   - Unlocked Talents
```

### **5. Progression System** 📈
```
❌ Domain/Progression/XpSystem.cs
   - XP Calculation
   - XP Sources (Kill, Headshot, Objective, etc.)

❌ Domain/Progression/LevelSystem.cs
   - Hero Level (1-20)
   - Skill Level (1-5)
   - Talent Points

❌ Domain/Progression/SkillMastery.cs
   - Mastery Tracking (Kills, Uses, Damage)
   - Mastery Rewards (Cosmetics, Modifiers)
```

### **6. Player State** 👤
```
❌ Domain/Players/UltimatePlayer.cs
   - Player State (Current Hero, Current Build, Skills)
   - Active Effects, Cooldowns
   - XP, Level, Talents

❌ Domain/Players/PlayerBuild.cs
   - Active Build Reference
   - Skill Instances (mit Level)
```

---

## ❌ **FEHLT NOCH - Application Layer (Services)**

### **1. Core Services** 🔧
```
❌ Application/Services/HeroService.cs
   - Hero Registration
   - Hero Selection
   - Hero Manager

❌ Application/Services/SkillService.cs
   - Skill Registration
   - Skill Activation
   - Skill Manager

❌ Application/Services/BuildService.cs
   - Build Creation
   - Build Saving/Loading
   - Build Switching

❌ Application/Services/TalentService.cs
   - Talent Unlocking
   - Talent Point Allocation
   - Talent Effects Application
```

### **2. Rules Engine** ⚖️
```
❌ Application/Rules/RulesEngine.cs
   - Erweiterte Rules (nicht nur in BuildValidator)
   - Tag-based Rules
   - Combination Rules
   - Diminishing Returns

❌ Application/Rules/RuleDefinitions.cs
   - Rule Definitions (Max 1 Ultimate, Max 2 Mobility, etc.)
   - Configurable Rules
```

### **3. Progression Services** 📊
```
❌ Application/Services/XpService.cs
   - XP Awarding
   - Level Calculation
   - Progression Tracking

❌ Application/Services/MasteryService.cs
   - Mastery Tracking
   - Mastery Rewards
   - Mastery Effects
```

### **4. In-Match Systems** 🎲
```
❌ Application/Services/InMatchEvolution.cs
   - Mini-Upgrade System
   - Kill Streak Rewards
   - Objective Rewards

❌ Application/Services/AdaptiveBalance.cs
   - Meta Analysis
   - Dynamic Skill Buffs/Nerfs
   - Counter-System Activation
```

### **5. Advanced Systems** 🎯
```
❌ Application/Services/RoleInfluenceService.cs
   - Role Detection (DPS, Support, Initiator, Clutch)
   - Role-based XP Bonuses
   - Role-based Recommendations

❌ Application/Services/BuildIntegrityService.cs
   - Diminishing Returns (CC, Flash, Stealth)
   - Anti-Toxic Build Detection
   - Camping Detection
```

---

## ❌ **FEHLT NOCH - Infrastructure Layer**

### **1. Database** 💾
```
❌ Infrastructure/Database/Database.cs
   - SQLite Connection
   - Database Initialization
   - Schema Creation

❌ Infrastructure/Database/Schema.sql
   - heroes (steamid, hero_core, hero_level, ...)
   - builds (steamid, build_slot, hero_core, skill1, skill2, skill3, ...)
   - skills (steamid, skill_id, skill_level, ...)
   - talents (steamid, talent_id, talent_level, ...)
   - mastery (steamid, skill_id, kills, uses, damage, ...)
   - xp_history (steamid, xp_source, amount, timestamp)

❌ Infrastructure/Database/Repositories/
   - IHeroRepository.cs + HeroRepository.cs
   - IBuildRepository.cs + BuildRepository.cs
   - ISkillRepository.cs + SkillRepository.cs
   - ITalentRepository.cs + TalentRepository.cs
   - IPlayerRepository.cs + PlayerRepository.cs
```

### **2. Event System** 📡
```
❌ Infrastructure/Events/EventSystem.cs
   - Event Registration
   - Event Dispatching
   - Event Handlers

❌ Infrastructure/Events/EventHandlers/
   - PlayerHurtHandler.cs
   - PlayerKillHandler.cs
   - PlayerSpawnHandler.cs
   - RoundStartHandler.cs
   - ObjectiveHandler.cs
```

### **3. Effect System** ✨
```
❌ Infrastructure/Effects/EffectManager.cs
   - Effect Registration
   - Effect Application
   - Effect Removal
   - Effect Stacking

❌ Infrastructure/Effects/Effects/
   - StunEffect.cs
   - HealEffect.cs
   - DamageOverTimeEffect.cs
   - SpeedBoostEffect.cs
   - (weitere Effects)
```

### **4. Cooldown System** ⏱️
```
❌ Infrastructure/Cooldown/CooldownManager.cs
   - Cooldown Tracking
   - Cooldown Reduction (Hero Identity)
   - Cooldown UI Updates
```

### **5. Helpers** 🛠️
```
❌ Infrastructure/Helpers/Geometry.cs
   - 3D Math, Distance, Angles
   - Ray Tracing

❌ Infrastructure/Helpers/GameHelpers.cs
   - Heal, Damage, Particles
   - Player Utilities
   - Weapon Utilities
```

---

## ❌ **FEHLT NOCH - Presentation Layer**

### **1. Menu System** 🎨
```
❌ Presentation/Menu/MenuManager.cs
   - Menu Registration
   - Menu Navigation

❌ Presentation/Menu/HeroMenu.cs
   - Hero Selection Menu

❌ Presentation/Menu/BuildMenu.cs
   - Build Editor
   - Build Selection
   - Build Naming

❌ Presentation/Menu/SkillMenu.cs
   - Skill Browser
   - Skill Selection

❌ Presentation/Menu/TalentMenu.cs
   - Talent Tree Display
   - Talent Point Allocation
```

### **2. Commands** 💬
```
❌ Presentation/Commands/CommandHandlers/
   - HeroCommandHandler.cs
   - BuildCommandHandler.cs
   - SkillCommandHandler.cs
   - TalentCommandHandler.cs
   - ShopCommandHandler.cs
```

### **3. UI/HUD** 📺
```
❌ Presentation/UI/SkillHud.cs
   - Active Skills Display
   - Cooldown Indicators
   - Ultimate Ready Indicator

❌ Presentation/UI/ProgressionHud.cs
   - XP Bar
   - Level Display
   - Mastery Progress
```

---

## ❌ **FEHLT NOCH - Advanced Features**

### **1. Shop System** 🛒
```
❌ Domain/Items/ShopItem.cs
❌ Application/Services/ShopService.cs
❌ Presentation/Menu/ShopMenu.cs
```

### **2. Server Events** 🎪
```
❌ Application/Services/ServerEventService.cs
   - Event Scheduling
   - Event Effects (Double XP, Chaos Mode, etc.)
```

### **3. Spectator/Streamer Hooks** 📹
```
❌ Infrastructure/Streaming/StreamerHooks.cs
   - Live Build Overlay
   - Skill Usage Feed
   - Damage Breakdown
```

---

## 📋 **Priorität: Was zuerst?**

### **🔥 Phase 1: MVP (KRITISCH)**
1. ✅ Build Domain Model (Build.cs)
2. ✅ Database Schema + Repositories
3. ✅ Hero Core Implementations (mind. 3 Heroes)
4. ✅ Skill Implementations (mind. 5 Skills)
5. ✅ Event System Integration
6. ✅ Menu System (Hero + Build Selection)
7. ✅ XP System (Basic)

### **⚡ Phase 2: Core Features**
1. ✅ Talent System
2. ✅ Skill Mastery
3. ✅ Cooldown Manager
4. ✅ Effect System
5. ✅ Rules Engine (erweitert)

### **🎯 Phase 3: Advanced Features**
1. ✅ In-Match Evolution
2. ✅ Adaptive Balance
3. ✅ Role Influence
4. ✅ Build Integrity Checks
5. ✅ Shop System

### **🌟 Phase 4: Polish**
1. ✅ Streamer Hooks
2. ✅ Server Events
3. ✅ Web-UI Integration (später)

---

## 🎯 **Zusammenfassung**

**Du hast bereits:**
- ✅ Core Interfaces (IHero, ISkill)
- ✅ HeroIdentity System
- ✅ Basic BuildValidator
- ✅ Plugin Structure

**Du brauchst noch:**
- ❌ **~15-20 Domain Models** (Build, Hero Cores, Skills, Talents, etc.)
- ❌ **~10-15 Services** (HeroService, SkillService, BuildService, etc.)
- ❌ **~5-8 Infrastructure Components** (Database, Events, Effects, etc.)
- ❌ **~5-8 Presentation Components** (Menus, Commands, UI)

**Geschätzte Zeilen Code noch:**
- Domain: ~3,000-4,000 Zeilen
- Application: ~2,000-3,000 Zeilen
- Infrastructure: ~2,000-3,000 Zeilen
- Presentation: ~1,500-2,000 Zeilen
- **Total: ~8,500-12,000 Zeilen noch zu schreiben**

---

**Status: Du hast die Foundation (10-15%), aber die meisten Core-Systeme fehlen noch!** 🚀
