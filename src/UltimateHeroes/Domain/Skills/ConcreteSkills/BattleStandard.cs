using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.Helpers;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Battle Standard - Ultimate Skill: Platziert Banner → Buffs für Allies im Radius
    /// </summary>
    public class BattleStandard : ActiveSkillBase, IUltimateSkill
    {
        public override string Id => "battle_standard";
        public override string DisplayName => "Battle Standard";
        public override string Description => "Platziert Banner → Buffs für Allies im Radius";
        public override int PowerWeight => 45;
        public override List<SkillTag> Tags => new() { SkillTag.Support, SkillTag.Area, SkillTag.Ultimate };
        public override int MaxLevel => 3;
        
        public override float Cooldown => 90f;
        
        private const float BaseRadius = 600f;
        private const float BaseDuration = 20f;
        private const float BaseDamageBonus = 0.15f; // 15% damage bonus
        private const float BaseSpeedBonus = 0.10f; // 10% speed bonus
        
        // Buff Definitions (wird einmal erstellt, kann wiederverwendet werden)
        private static readonly Domain.Buffs.BuffDefinition DamageBoostBuffDefinition = new()
        {
            Id = "battle_standard_damage", // Will be made unique per player
            DisplayName = "Battle Standard - Damage",
            Type = Domain.Buffs.BuffType.DamageBoost,
            StackingType = Domain.Buffs.BuffStackingType.Refresh
        };
        
        private static readonly Domain.Buffs.BuffDefinition SpeedBoostBuffDefinition = new()
        {
            Id = "battle_standard_speed", // Will be made unique per player
            DisplayName = "Battle Standard - Speed",
            Type = Domain.Buffs.BuffType.SpeedBoost,
            StackingType = Domain.Buffs.BuffStackingType.Refresh
        };
        
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            var pawn = player.PlayerPawn.Value;
            if (pawn.AbsOrigin == null) return;
            
            // Calculate stats based on level
            var radius = BaseRadius + (CurrentLevel * 100);
            var duration = BaseDuration + (CurrentLevel * 5f);
            var damageBonus = BaseDamageBonus + (CurrentLevel * 0.05f);
            var speedBonus = BaseSpeedBonus + (CurrentLevel * 0.03f);
            
            // Spawn banner particle
            var bannerPos = new Vector(pawn.AbsOrigin.X, pawn.AbsOrigin.Y, pawn.AbsOrigin.Z + 50);
            GameHelpers.SpawnParticle(bannerPos, "particles/ui/ui_electric_exp_glow.vpcf", duration);
            
            // Find all allies in radius
            var playersInRadius = GameHelpers.GetPlayersInRadius(pawn.AbsOrigin, radius);
            
            int buffedCount = 0;
            
            // Create Buffs in Skill (not in Service)
            var buffService = Infrastructure.Helpers.BuffServiceHelper.GetBuffService();
            
            foreach (var ally in playersInRadius)
            {
                if (ally == player) continue;
                if (!ally.IsValid || ally.AuthorizedSteamID == null) continue;
                
                var allySteamId = ally.AuthorizedSteamID.SteamId64.ToString();
                
                // Create Buffs from Definitions (generisch)
                if (buffService != null)
                {
                    // Create Damage Boost Buff from Definition
                    var damageBuff = DamageBoostBuffDefinition.CreateBuff(
                        duration,
                        new System.Collections.Generic.Dictionary<string, float>
                        {
                            { "multiplier", damageBonus }
                        }
                    );
                    damageBuff.Id = $"battle_standard_damage_{allySteamId}"; // Make unique per player
                    buffService.ApplyBuff(allySteamId, damageBuff);
                    
                    // Create Speed Boost Buff from Definition
                    var speedBuff = SpeedBoostBuffDefinition.CreateBuff(
                        duration,
                        new System.Collections.Generic.Dictionary<string, float>
                        {
                            { "multiplier", speedBonus }
                        }
                    );
                    speedBuff.Id = $"battle_standard_speed_{allySteamId}"; // Make unique per player
                    buffService.ApplyBuff(allySteamId, speedBuff);
                }
                
                buffedCount++;
                ally.PrintToChat($" {ChatColors.Gold}[Battle Standard]{ChatColors.Default} Buffed! +{damageBonus * 100:F0}% damage, +{speedBonus * 100:F0}% speed!");
            }
            
            player.PrintToChat($" {ChatColors.Gold}[Battle Standard]{ChatColors.Default} Ultimate! Buffed {buffedCount} allies for {duration:F1}s!");
        }
    }
}
