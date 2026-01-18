using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Effects;
using UltimateHeroes.Infrastructure.Effects.ConcreteEffects;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Fortress Mode - Ultimate Skill: +Armor, Immun gegen CC, kein Sprint
    /// </summary>
    public class FortressMode : ActiveSkillBase, IUltimateSkill
    {
        public override string Id => "fortress_mode";
        public override string DisplayName => "Fortress Mode";
        public override string Description => "+Armor, Immun gegen CC, kein Sprint";
        public override int PowerWeight => 50;
        public override List<SkillTag> Tags => new() { SkillTag.Defense, SkillTag.Ultimate };
        public override int MaxLevel => 3;
        
        public override float Cooldown => 100f;
        
        private const float BaseDuration = 15f;
        private const int BaseArmorBonus = 50;
        
        // EffectManager wird über SkillService gesetzt
        public static EffectManager? EffectManager { get; set; }
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            // Calculate duration and armor based on level
            var duration = BaseDuration + (CurrentLevel * 5f);
            var armorBonus = BaseArmorBonus + (CurrentLevel * 25);
            
            // Apply Fortress Mode Effect
            if (EffectManager != null)
            {
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                var effect = new FortressModeEffect
                {
                    Duration = duration,
                    ArmorBonus = armorBonus
                };
                EffectManager.ApplyEffect(steamId, effect);
            }
            
            player.PrintToChat($" {ChatColors.Gold}[Fortress Mode]{ChatColors.Default} Ultimate! +{armorBonus} armor for {duration:F1}s!");
        }
    }
}
