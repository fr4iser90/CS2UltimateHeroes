# 🏗️ Ultimate Heroes - Projekt-Struktur

## 📁 Empfohlene DDD-Struktur

```
CS2UltimateHeroes/
├── docs/                          # Dokumentation
│   ├── ANALYSIS.md
│   ├── CODE_ANALYSIS.md
│   ├── CHAT_SUMMARY.md
│   └── DESIGN.md
│
├── src/                          # Source Code
│   └── UltimateHeroes/
│       ├── Domain/               # Core Business Logic
│       │   ├── Heroes/
│       │   │   ├── Hero.cs
│       │   │   ├── HeroCore.cs
│       │   │   └── HeroIdentity.cs
│       │   ├── Skills/
│       │   │   ├── Skill.cs
│       │   │   ├── PassiveSkill.cs
│       │   │   ├── ActiveSkill.cs
│       │   │   └── UltimateSkill.cs
│       │   ├── Builds/
│       │   │   ├── Build.cs
│       │   │   └── BuildSlot.cs
│       │   ├── Talents/
│       │   │   ├── TalentTree.cs
│       │   │   └── TalentNode.cs
│       │   └── Progression/
│       │       ├── XpSystem.cs
│       │       └── LevelSystem.cs
│       │
│       ├── Application/          # Use Cases
│       │   ├── Services/
│       │   │   ├── HeroService.cs
│       │   │   ├── SkillService.cs
│       │   │   ├── BuildService.cs
│       │   │   └── TalentService.cs
│       │   ├── Validators/
│       │   │   ├── BuildValidator.cs
│       │   │   └── PowerBudgetValidator.cs
│       │   └── Rules/
│       │       └── RulesEngine.cs
│       │
│       ├── Infrastructure/       # External Concerns
│       │   ├── Database/
│       │   │   ├── Database.cs
│       │   │   └── Repositories/
│       │   ├── Events/
│       │   │   ├── EventSystem.cs
│       │   │   └── EventHandlers/
│       │   ├── Helpers/
│       │   │   ├── Geometry.cs
│       │   │   └── RayTracer.cs
│       │   └── Effects/
│       │       └── EffectManager.cs
│       │
│       └── Presentation/         # UI / Commands
│           ├── Menu/
│           │   ├── HeroMenu.cs
│           │   ├── BuildMenu.cs
│           │   └── TalentMenu.cs
│           └── Commands/
│               └── CommandHandlers/
│
├── tests/                        # Tests (später)
│
├── LICENSE
├── README.md
├── shell.nix                     # Nix-Shell Setup
└── build.sh                      # Build-Script
```

## 🎯 Phase 1: MVP-Struktur (Start)

```
UltimateHeroes/
├── Domain/
│   ├── Heroes/
│   │   ├── IHero.cs
│   │   └── HeroCore.cs
│   ├── Skills/
│   │   ├── ISkill.cs
│   │   ├── SkillTag.cs
│   │   └── SkillWeight.cs
│   └── Builds/
│       └── Build.cs
│
├── Application/
│   ├── HeroManager.cs
│   ├── SkillManager.cs
│   └── BuildValidator.cs
│
├── Infrastructure/
│   ├── Database.cs
│   ├── EventSystem.cs
│   └── Helpers/
│
└── UltimateHeroes.cs            # Main Plugin
```

## 📋 Nächste Schritte

1. ✅ Projekt-Struktur erstellen
2. ✅ Core-Interfaces definieren (IHero, ISkill)
3. ✅ Power Budget System
4. ✅ Rules Engine
5. ✅ Database-Schema
