# 📊 Plan: XpService

## 📋 Zweck

Der XpService verwaltet XP und Level für Spieler:
- XP Awarding
- Level Calculation
- Level-Up Handling
- Talent Points Awarding

## 🔗 Abhängigkeiten

- `XpSystem` (Domain/Progression/XpSystem.cs) - später
- `LevelSystem` (Domain/Progression/LevelSystem.cs) - später
- `UltimatePlayer` (Domain/Players/UltimatePlayer.cs) - später
- `IPlayerRepository` (Infrastructure/Database) - später
- `ITalentService` (Application/Services/ITalentService.cs) - später

## 📐 Service Interface

```csharp
namespace UltimateHeroes.Application.Services
{
    public interface IXpService
    {
        // XP Management
        void AwardXp(string steamId, XpSource source, float amount);
        void AwardXp(string steamId, XpSource source);
        float GetCurrentXp(string steamId);
        int GetHeroLevel(string steamId);
        float GetXpProgress(string steamId); // 0.0 - 1.0
        
        // Level Management
        void CheckLevelUp(string steamId);
        void OnLevelUp(string steamId, int newLevel);
        
        // XP History
        List<XpHistory> GetXpHistory(string steamId, int limit = 50);
    }
}
```

## 🎯 Implementierung

### Datei: `Application/Services/XpService.cs`

```csharp
namespace UltimateHeroes.Application.Services
{
    public class XpService : IXpService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ITalentService _talentService;
        private readonly IPlayerService _playerService;
        
        public XpService(
            IPlayerRepository playerRepository,
            ITalentService talentService,
            IPlayerService playerService)
        {
            _playerRepository = playerRepository;
            _talentService = talentService;
            _playerService = playerService;
        }
        
        public void AwardXp(string steamId, XpSource source, float amount)
        {
            var player = _playerService.GetPlayer(steamId);
            if (player == null) return;
            
            // Apply Role Influence Bonus
            var bonus = GetRoleInfluenceBonus(player.CurrentRole, source);
            amount *= (1f + bonus);
            
            // Award XP
            player.CurrentXp += amount;
            
            // Save XP History
            _playerRepository.SaveXpHistory(new XpHistory
            {
                SteamId = steamId,
                Source = source,
                Amount = amount,
                Timestamp = DateTime.UtcNow
            });
            
            // Check Level Up
            CheckLevelUp(steamId);
            
            // Save Player
            _playerRepository.SavePlayer(player);
        }
        
        public void AwardXp(string steamId, XpSource source)
        {
            var amount = GetXpAmount(source);
            AwardXp(steamId, source, amount);
        }
        
        private float GetXpAmount(XpSource source)
        {
            return source switch
            {
                XpSource.Kill => XpSystem.XpPerKill,
                XpSource.Headshot => XpSystem.XpPerHeadshot,
                XpSource.Assist => XpSystem.XpPerAssist,
                XpSource.Objective => XpSystem.XpPerObjective,
                XpSource.RoundWin => XpSystem.XpPerRoundWin,
                XpSource.FlashAssist => XpSystem.XpPerFlashAssist,
                XpSource.ClutchRound => 50f,
                XpSource.FirstBlood => 15f,
                _ => 0f
            };
        }
        
        private float GetRoleInfluenceBonus(RoleInfluence role, XpSource source)
        {
            // Support Role: +20% XP für Support-Aktionen
            if (role == RoleInfluence.Support && (source == XpSource.FlashAssist || source == XpSource.Assist))
            {
                return 0.2f;
            }
            
            // DPS Role: +10% XP für Kills
            if (role == RoleInfluence.DPS && source == XpSource.Kill)
            {
                return 0.1f;
            }
            
            return 0f;
        }
        
        public float GetCurrentXp(string steamId)
        {
            var player = _playerService.GetPlayer(steamId);
            return player?.CurrentXp ?? 0f;
        }
        
        public int GetHeroLevel(string steamId)
        {
            var player = _playerService.GetPlayer(steamId);
            return player?.HeroLevel ?? 1;
        }
        
        public float GetXpProgress(string steamId)
        {
            var player = _playerService.GetPlayer(steamId);
            if (player == null) return 0f;
            
            return XpSystem.GetXpProgress(player.CurrentXp, player.HeroLevel);
        }
        
        public void CheckLevelUp(string steamId)
        {
            var player = _playerService.GetPlayer(steamId);
            if (player == null) return;
            
            var newLevel = XpSystem.GetLevelFromXp(player.CurrentXp);
            
            if (newLevel > player.HeroLevel)
            {
                OnLevelUp(steamId, newLevel);
            }
        }
        
        public void OnLevelUp(string steamId, int newLevel)
        {
            var player = _playerService.GetPlayer(steamId);
            if (player == null) return;
            
            var oldLevel = player.HeroLevel;
            player.HeroLevel = newLevel;
            
            // Award Talent Points
            var talentPoints = newLevel - oldLevel;
            _talentService.AwardTalentPoints(steamId, talentPoints);
            
            // Notify Player
            var playerController = player.PlayerController;
            if (playerController != null && playerController.IsValid)
            {
                playerController.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} Level Up! You are now Level {newLevel}!");
            }
            
            // Save Player
            _playerRepository.SavePlayer(player);
        }
        
        public List<XpHistory> GetXpHistory(string steamId, int limit = 50)
        {
            return _playerRepository.GetXpHistory(steamId, limit);
        }
    }
}
```

## 🔄 Integration

1. **Event Handlers**: Awarded XP bei Events (Kill, Objective, etc.)
2. **PlayerRepository**: Speichert XP/Level
3. **TalentService**: Verleiht Talent Points bei Level-Up
4. **PlayerService**: Updated Player State
5. **UI**: Zeigt XP Bar, Level

## ✅ Tests

- XP wird korrekt vergeben
- Level Calculation funktioniert
- Level-Up wird korrekt erkannt
- Talent Points werden vergeben
- Role Influence Bonus wird angewendet

## 📝 Nächste Schritte

1. ✅ IXpService Interface definieren
2. ✅ XpService.cs implementieren
3. ✅ Integration mit Event Handlers
4. ✅ Integration mit PlayerRepository
5. ✅ Integration mit TalentService
