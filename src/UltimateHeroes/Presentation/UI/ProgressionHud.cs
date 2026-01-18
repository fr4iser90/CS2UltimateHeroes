using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Players;

namespace UltimateHeroes.Presentation.UI
{
    /// <summary>
    /// Progression HUD - Zeigt XP, Level, Mastery Progress
    /// </summary>
    public class ProgressionHud
    {
        private readonly IXpService _xpService;
        private readonly IMasteryService? _masteryService;
        private readonly IAccountService? _accountService;
        
        public ProgressionHud(IXpService xpService, IMasteryService? masteryService = null, IAccountService? accountService = null)
        {
            _xpService = xpService;
            _masteryService = masteryService;
            _accountService = accountService;
        }
        
        /// <summary>
        /// Rendert das Progression HUD für einen Spieler
        /// </summary>
        public void Render(CCSPlayerController player, UltimatePlayer playerState)
        {
            if (player == null || !player.IsValid || playerState == null) return;
            player.PrintToCenterHtml(GetHtml(playerState));
        }
        
        /// <summary>
        /// Gibt HTML für Progression HUD zurück
        /// </summary>
        public string GetHtml(UltimatePlayer playerState)
        {
            if (playerState == null) return string.Empty;
            
            var xpProgress = _xpService.GetXpProgress(playerState.SteamId);
            var xpPercent = (int)(xpProgress * 100);
            var currentXp = playerState.CurrentXp;
            var xpToNext = playerState.XpToNextLevel;
            var heroLevel = playerState.HeroLevel;
            
            // Account Level (wenn verfügbar)
            var accountLevel = _accountService?.GetAccountLevelValue(playerState.SteamId) ?? 0;
            var accountXpProgress = _accountService?.GetAccountXpProgress(playerState.SteamId) ?? 0f;
            var accountXpPercent = (int)(accountXpProgress * 100);
            var accountTitle = accountLevel > 0 ? Domain.Progression.AccountLevel.GetTitleForLevel(accountLevel) : "";
            
            return $@"
                <div style='
                    position: fixed;
                    top: 20px;
                    left: 50%;
                    transform: translateX(-50%);
                    background: rgba(0, 0, 0, 0.7);
                    border: 2px solid #4A90E2;
                    border-radius: 8px;
                    padding: 12px 20px;
                    min-width: 300px;
                    z-index: 1000;
                    box-shadow: 0 4px 12px rgba(0,0,0,0.5);
                '>
                    {(accountLevel > 0 ? $@"
                    <div style='display: flex; justify-content: space-between; align-items: center; margin-bottom: 6px;'>
                        <div style='font-size: 14px; color: #FFD700; font-weight: bold;'>
                            Account: Lv.{accountLevel} ({accountTitle})
                        </div>
                        <div style='font-size: 11px; color: #CCCCCC;'>
                            {accountXpPercent}%
                        </div>
                    </div>
                    <div style='
                        width: 100%;
                        height: 12px;
                        background: rgba(100, 100, 100, 0.5);
                        border-radius: 6px;
                        overflow: hidden;
                        margin-bottom: 8px;
                    '>
                        <div style='
                            width: {accountXpPercent}%;
                            height: 100%;
                            background: linear-gradient(90deg, #FFD700 0%, #FFA500 100%);
                            transition: width 0.3s ease;
                        '></div>
                    </div>
                    " : "")}
                    <div style='display: flex; justify-content: space-between; align-items: center; margin-bottom: 8px;'>
                        <div style='font-size: 16px; color: #4A90E2; font-weight: bold;'>
                            Hero Level {heroLevel}
                        </div>
                        <div style='font-size: 12px; color: #CCCCCC;'>
                            {currentXp:F0} / {xpToNext:F0} XP ({xpPercent}%)
                        </div>
                    </div>
                    <div style='
                        width: 100%;
                        height: 20px;
                        background: rgba(100, 100, 100, 0.5);
                        border-radius: 10px;
                        overflow: hidden;
                        position: relative;
                    '>
                        <div style='
                            width: {xpPercent}%;
                            height: 100%;
                            background: linear-gradient(90deg, #4A90E2 0%, #5BA3F5 100%);
                            transition: width 0.3s ease;
                            box-shadow: 0 0 10px rgba(74, 144, 226, 0.5);
                        '></div>
                    </div>
                </div>";
        }
    }
}
