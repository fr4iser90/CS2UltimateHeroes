using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Effects;
using UltimateHeroes.Infrastructure.Effects.ConcreteEffects;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Fade on Kill - Passive Skill: Kurzzeit-Stealth nach Kill (0.5-1s)
    /// </summary>
    public class FadeOnKillPassive : PassiveSkillBase
    {
        public override string Id => "fade_on_kill_passive";
        public override string DisplayName => "Fade on Kill";
        public override string Description => "Kurzzeit-Stealth nach Kill";
        public override int PowerWeight => 15;
        public override List<SkillTag> Tags => new() { SkillTag.Stealth };
        public override int MaxLevel => 5;
        
        private const float BaseDuration = 0.5f;
        
        // EffectManager wird über SkillService gesetzt
        public static EffectManager? EffectManager { get; set; }
        
        public override void OnPlayerSpawn(CCSPlayerController player)
        {
            // Passive is active, no spawn action needed
        }
        
        public override void OnPlayerHurt(CCSPlayerController player, int damage)
        {
            // No action on hurt
        }
        
        public override void OnPlayerKill(CCSPlayerController player, CCSPlayerController victim)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            // Calculate duration based on level
            var duration = BaseDuration + (CurrentLevel * 0.1f); // 0.5s - 1.0s
            
            // Apply Invisibility Effect
            if (EffectManager != null)
            {
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                var effect = new InvisibilityEffect
                {
                    Duration = duration
                };
                EffectManager.ApplyEffect(steamId, effect);
            }
            
            player.PrintToChat($" {ChatColors.Purple}[Fade on Kill]{ChatColors.Default} Faded for {duration:F1}s!");
        }
    }
}
