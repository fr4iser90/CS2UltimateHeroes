using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Effects;
using UltimateHeroes.Infrastructure.Effects.ConcreteEffects;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Shadow Realm - Ultimate Skill: Vollständige Unsichtbarkeit, keine Collision, kein Schaden möglich
    /// </summary>
    public class ShadowRealm : ActiveSkillBase, IUltimateSkill
    {
        public override string Id => "shadow_realm";
        public override string DisplayName => "Shadow Realm";
        public override string Description => "Vollständige Unsichtbarkeit, keine Collision, kein Schaden möglich";
        public override int PowerWeight => 55;
        public override List<SkillTag> Tags => new() { SkillTag.Stealth, SkillTag.Ultimate };
        public override int MaxLevel => 3;
        
        public override float Cooldown => 120f;
        
        private const float BaseDuration = 8f;
        
        // EffectManager wird über SkillService gesetzt
        public static EffectManager? EffectManager { get; set; }
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            // Calculate duration based on level
            var duration = BaseDuration + (CurrentLevel * 2f);
            
            // Apply Shadow Realm Effect
            if (EffectManager != null)
            {
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                var effect = new ShadowRealmEffect
                {
                    Duration = duration
                };
                EffectManager.ApplyEffect(steamId, effect);
            }
            
            player.PrintToChat($" {ChatColors.Gold}[Shadow Realm]{ChatColors.Default} Ultimate! Shadow Realm for {duration:F1}s!");
        }
    }
}
