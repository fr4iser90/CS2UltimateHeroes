using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Effects;
using UltimateHeroes.Infrastructure.Effects.ConcreteEffects;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Stealth - Active Skill: Macht Spieler unsichtbar
    /// </summary>
    public class Stealth : ActiveSkillBase
    {
        public override string Id => "stealth";
        public override string DisplayName => "Stealth";
        public override string Description => "Macht dich unsichtbar für kurze Zeit";
        public override int PowerWeight => 30;
        public override List<SkillTag> Tags => new() { SkillTag.Stealth, SkillTag.Utility };
        public override int MaxLevel => 5;
        
        public override float Cooldown => 15f;
        
        private const float BaseDuration = 5f;
        
        // EffectManager wird über SkillService gesetzt
        public static EffectManager? EffectManager { get; set; }
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            // Calculate duration based on level
            var duration = BaseDuration + (CurrentLevel * 1f);
            
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
            
            player.PrintToChat($" {ChatColors.Purple}[Stealth]{ChatColors.Default} Invisible for {duration:F1}s!");
        }
    }
}
