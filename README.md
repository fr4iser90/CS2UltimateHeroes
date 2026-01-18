# Ultimate Heroes Mod for CS2

Ein modulares Hero-System für Counter-Strike 2 mit kombinierbaren Skills, Build-System und Talent-Tree.

## 🎯 Konzept

- **Hero Core**: Basis-Hero mit 1-2 passiven Fähigkeiten
- **Skill Slots**: 2-3 Slots für kombinierbare Skills
- **Build-System**: Speichere und wechsle zwischen Builds
- **Power Budget**: Balance durch Weight-System
- **Talent-Tree**: Nicht-lineare Progression

## 📋 Features

### Core-Systems
- ✅ Modular Hero + Skill System
- ✅ Power Budget / Weight System
- ✅ Skill Tags + Rules Engine
- ✅ Hero Identity Auras
- ✅ Build-System (3-5 Slots)
- ✅ Talent-Tree (Combat/Utility/Movement)
- ✅ In-Match Evolution (Mini-Upgrades)
- ✅ Skill Mastery System

### Gameplay
- ✅ XP-System mit Database
- ✅ Shop-System
- ✅ Menu-System
- ✅ Event-System
- ✅ Effect-System

## 🏗️ Architektur

**Domain-Driven Design (DDD) Struktur:**

```
UltimateHeroes/
├── Domain/              # Core Business Logic
│   ├── Heroes/         # Hero Aggregates
│   ├── Skills/         # Skill Aggregates
│   ├── Builds/         # Build Aggregates
│   └── Talents/        # Talent Aggregates
├── Application/         # Use Cases / Services
│   ├── HeroService.cs
│   ├── SkillService.cs
│   └── BuildService.cs
├── Infrastructure/     # External Concerns
│   ├── Database/
│   ├── Events/
│   └── Helpers/
└── Presentation/       # UI / Commands
    ├── Menu/
    └── Commands/
```

## 🔗 Credits & Inspiration

**Inspiration:**
This project was inspired by the [WarcraftPlugin](https://github.com/Wngui/CS2WarcraftMod) by WnGui.
**Note:** This is not a fork - it's a new project with a different architecture and design philosophy.

## 📄 License

GPL-3.0 (see LICENSE file)

## 🚀 Setup

```bash
# Nix-Shell 
nix-shell

# Oder manuell
dotnet restore
dotnet build -c Release
```

## 📖 Commands

```
!hero      - Hero-Auswahl
!build     - Build-Editor
!skills    - Skill-Browser
!talents   - Talent-Tree
!shop      - Shop
```

---

**Status:** 🚧 In Development
