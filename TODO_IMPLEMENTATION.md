# TODO: Vollständige Implementation aller vereinfachten Features

## 🔴 Kritisch (Core Systems)

### 1. **Buff System** (Generisch)
- [ ] `IBuffSystem` / `BuffSystem` Interface & Implementation
- [ ] `Buff` Domain Model (Type, Duration, Stacking, Parameters)
- [ ] Integration in `EffectManager` oder separates System
- [ ] Buff Types: Damage, Speed, Armor, Spread, Fire Rate, etc.
- [ ] Stacking Rules (Additive, Multiplicative, Max Stack)

### 2. **Weapon Modifier System**
- [ ] `IWeaponModifier` Interface
- [ ] Weapon Properties: Spread, Fire Rate, Damage, Ammo, Recoil
- [ ] Integration in `GameHelpers.DamagePlayer`
- [ ] Integration in Weapon Events (OnWeaponFire, etc.)
- [ ] Infinite Ammo für Bullet Storm
- [ ] Weapon Spread Modifier für Taunt

### 3. **Damage System Erweiterung**
- [ ] Shield Damage Reduction in `GameHelpers.DamagePlayer`
- [ ] Execution Mark Damage Multiplier in `GameHelpers.DamagePlayer`
- [ ] Taunt Damage Reduction (wenn nicht Taunter angegriffen)
- [ ] Fortified Plating Damage Reduction (unter 50% HP)
- [ ] Effect-basierte Damage Modifiers

### 4. **Spawn/Summon Service**
- [ ] `ISpawnService` / `SpawnService` Interface & Implementation
- [ ] Entity Spawning (Sentry, Drone, etc.)
- [ ] Entity Tracking (SteamId -> Entities)
- [ ] Entity Cleanup (on death, disconnect, map change)
- [ ] Mini Sentry: Auto-Attack Logic
- [ ] Scanner Drone: Reveal Logic

## 🟡 Wichtig (Feature Completion)

### 5. **Taunt Mechanik (CS2-kompatibel)**
- [ ] Taunt Effect: Reduced Damage (wenn nicht Taunter angegriffen)
- [ ] Taunt Effect: Increased Weapon Spread
- [ ] Taunt Tracking: Wer ist Taunter, wer ist Taunted
- [ ] Damage Check: Wenn Taunted, prüfe ob Taunter angegriffen wird

### 6. **Reveal System**
- [ ] `RevealEffect` für Scanner Drone / Global Scan
- [ ] Persistent Reveal (Duration-basiert)
- [ ] Reveal Visual Indicator (Particle, Glow, etc.)
- [ ] Reveal Integration in `MakePlayerInvisible` (override)

### 7. **Assist Tracking**
- [ ] Assist Detection in `PlayerKillHandler`
- [ ] Assist Time Window (z.B. 5 Sekunden)
- [ ] Assist Damage Threshold (z.B. 50 Damage)
- [ ] Shield on Assist Passive Integration

### 8. **Backstab Detection**
- [ ] Backstab Angle Check (180° hinter dem Opfer)
- [ ] Backstab Integration in `PlayerKillHandler`
- [ ] Backstab Momentum Passive: Cooldown Reduction
- [ ] Backstab Visual Feedback

### 9. **Passive Skill Integration**
- [ ] Utility CDR Passive: Cooldown Reduction in `SkillService`
- [ ] Overclock Passive: HP Cost + Power Bonus in `SkillService`
- [ ] Adaptive Armor: Proper Damage Type Tracking
- [ ] Mini Sentry: Entity Spawning via SpawnService

## 🟢 Nice-to-Have (Polish)

### 10. **Collision Disable**
- [ ] Shadow Realm: Collision Disable (falls CS2 API unterstützt)
- [ ] Alternative: Ghost Mode (kein Schaden möglich)

### 11. **Battle Standard Buffs**
- [ ] Echte Buffs für Allies (Damage, Speed)
- [ ] Buff Duration Tracking
- [ ] Buff Visual Indicator

### 12. **Fortress Mode**
- [ ] CC Immunity (Stun, Taunt, etc. ignorieren)
- [ ] Sprint Disable (Movement Speed Cap)

### 13. **Bullet Storm**
- [ ] Fire Rate Multiplier (falls CS2 API unterstützt)
- [ ] Infinite Ammo (Ammo Refill on Tick)

### 14. **Wall Dash / Grapple Hook**
- [ ] Wall Detection für Wall Dash
- [ ] Surface Detection für Grapple Hook
- [ ] Smooth Movement statt Teleport

## 📋 Implementation Order

1. **Buff System** (Basis für alles)
2. **Weapon Modifier System** (für Taunt, Bullet Storm)
3. **Damage System Erweiterung** (Shield, Execution Mark, Taunt)
4. **Spawn Service** (für Sentry, Drone)
5. **Taunt Mechanik** (nutzt Buff + Weapon Modifier)
6. **Reveal System** (nutzt Buff System)
7. **Assist Tracking** (nutzt Event System)
8. **Backstab Detection** (nutzt Event System)
9. **Passive Integration** (nutzt alle obigen Systeme)
10. **Polish** (Collision, Visuals, etc.)
