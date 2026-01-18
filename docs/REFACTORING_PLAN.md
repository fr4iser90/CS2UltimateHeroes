# 🔧 Refactoring Plan: Code Cleanup & DDD Layer Control

## 📊 Aktuelle Probleme

### 1. **Monolithische Dateien**
- `UltimateHeroes.cs`: **1012 Zeilen** ❌ (sollte < 300 sein)
- `BotService.cs`: **470 Zeilen** ⚠️ (könnte aufgeteilt werden)
- `BuffService.cs`: **431 Zeilen** ⚠️ (könnte aufgeteilt werden)
- `TalentDefinitions.cs`: **364 Zeilen** ⚠️ (könnte aufgeteilt werden)

### 2. **DDD Layer Violations**
- `UltimateHeroes.cs` mischt:
  - Plugin Bootstrap (Infrastructure)
  - Event Handling (Application)
  - Command Handling (Presentation)
  - Configuration (Infrastructure)

### 3. **Hardcoded Values**
- Magic Numbers in Commands
- Hardcoded Strings
- Config-Werte direkt im Code

---

## 🎯 Refactoring-Strategie

### **Phase 1: UltimateHeroes.cs aufteilen**

#### 1.1 Plugin Bootstrap (Infrastructure)
```
Infrastructure/Plugin/
├── PluginBootstrap.cs          # Load(), Service Initialization
├── ServiceContainer.cs         # Dependency Container
└── PluginConfiguration.cs      # Config Class
```

#### 1.2 Event Handlers (Application)
```
Application/EventHandlers/
├── PlayerEventHandler.cs       # OnPlayerSpawn, OnPlayerDeath, etc.
├── RoundEventHandler.cs       # OnRoundStart, OnRoundEnd
└── MapEventHandler.cs          # OnMapStart
```

#### 1.3 Command Handlers (Presentation)
```
Presentation/Commands/
├── CommandRegistry.cs          # Command Registration
├── HeroCommands.cs             # !hero, !selecthero
├── BuildCommands.cs            # !build, !createbuild, !activatebuild
├── SkillCommands.cs            # !skills, !use, !skill1-3, !ultimate
├── TalentCommands.cs           # !talents
├── ShopCommands.cs             # !shop
└── StatsCommands.cs            # !stats, !botstats, !hud
```

#### 1.4 Reflection Helpers (Infrastructure)
```
Infrastructure/Registration/
├── ReflectionHelper.cs         # SetEffectManager, SetSpawnService
└── AutoRegistrationService.cs  # Centralized auto-registration
```

---

### **Phase 2: Services aufteilen**

#### 2.1 BuffService aufteilen
```
Application/Services/Buffs/
├── BuffService.cs              # Core Logic (200 Zeilen)
├── BuffStackingHandler.cs      # Stacking Logic
└── BuffQueryService.cs         # Query Methods
```

#### 2.2 BotService aufteilen
```
Application/Services/Bots/
├── BotService.cs               # Core Logic (200 Zeilen)
├── BotBuildManager.cs          # Build Assignment
├── BotStatsTracker.cs          # Stats Tracking
└── BotConfigurationManager.cs  # Configuration
```

---

### **Phase 3: Configuration & Constants**

#### 3.1 Constants extrahieren
```
Infrastructure/Configuration/
├── GameConstants.cs            # Magic Numbers
├── PluginConstants.cs          # Plugin-spezifische Werte
└── DefaultValues.cs            # Default Config Values
```

#### 3.2 Config auslagern
```
Infrastructure/Configuration/
└── PluginConfig.cs             # Aus UltimateHeroes.cs
```

---

### **Phase 4: DDD Layer Enforcement**

#### 4.1 Dependency Rules
- ✅ Domain darf NICHTS importieren
- ✅ Application darf nur Domain importieren
- ✅ Infrastructure darf Domain + Application importieren
- ✅ Presentation darf alles importieren

#### 4.2 Service Interfaces
- Alle Services haben Interfaces
- Dependency Injection via Constructor
- Keine statischen Helper mehr (außer Reflection)

---

## 📋 Refactoring Checklist

### **UltimateHeroes.cs Aufteilung**
- [ ] PluginBootstrap.cs erstellen
- [ ] ServiceContainer.cs erstellen
- [ ] PluginConfiguration.cs erstellen
- [ ] PlayerEventHandler.cs erstellen
- [ ] RoundEventHandler.cs erstellen
- [ ] MapEventHandler.cs erstellen
- [ ] CommandRegistry.cs erstellen
- [ ] HeroCommands.cs erstellen
- [ ] BuildCommands.cs erstellen
- [ ] SkillCommands.cs erstellen
- [ ] TalentCommands.cs erstellen
- [ ] ShopCommands.cs erstellen
- [ ] StatsCommands.cs erstellen
- [ ] ReflectionHelper.cs erstellen
- [ ] UltimateHeroes.cs auf < 100 Zeilen reduzieren

### **Services Aufteilung**
- [ ] BuffService aufteilen
- [ ] BotService aufteilen
- [ ] TalentDefinitions aufteilen (nach Trees)

### **Configuration**
- [ ] GameConstants.cs erstellen
- [ ] PluginConstants.cs erstellen
- [ ] DefaultValues.cs erstellen
- [ ] Config auslagern

### **DDD Layer Control**
- [ ] Dependency Rules dokumentieren
- [ ] Layer-Violations finden und fixen
- [ ] Statische Helper refactoren

---

## 🎯 Erwartete Verbesserungen

1. **Wartbarkeit**: Jede Datei < 300 Zeilen
2. **Testbarkeit**: Services isoliert testbar
3. **DDD Compliance**: Klare Layer-Trennung
4. **Erweiterbarkeit**: Neue Commands/Events einfach hinzufügbar
5. **Lesbarkeit**: Klare Verantwortlichkeiten
