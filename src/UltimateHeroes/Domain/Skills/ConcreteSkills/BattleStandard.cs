using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Helpers;

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
                
                // Create Damage Boost Buff
                if (buffService != null)
                {
                    var damageBuff = new Domain.Buffs.Buff
                    {
                        Id = $"battle_standard_damage_{allySteamId}", // Unique per player
                        DisplayName = "Battle Standard - Damage",
                        Type = Domain.Buffs.BuffType.DamageBoost,
                        Duration = duration,
                        StackingType = Domain.Buffs.BuffStackingType.Refresh,
                        Parameters = new System.Collections.Generic.Dictionary<string, float>
                        {
                            { "multiplier", damageBonus }
                        }
                    };
                    buffService.ApplyBuff(allySteamId, damageBuff);
                    
                    // Create Speed Boost Buff
                    var speedBuff = new Domain.Buffs.Buff
                    {
                        Id = $"battle_standard_speed_{allySteamId}", // Unique per player
                        DisplayName = "Battle Standard - Speed",
                        Type = Domain.Buffs.BuffType.SpeedBoost,
                        Duration = duration,
                        StackingType = Domain.Buffs.BuffStackingType.Refresh,
                        Parameters = new System.Collections.Generic.Dictionary<string, float>
                        {
                            { "multiplier", speedBonus }
                        }
                    };
                    buffService.ApplyBuff(allySteamId, speedBuff);
                }
                
                buffedCount++;
                ally.PrintToChat($" {ChatColors.Gold}[Battle Standard]{ChatColors.Default} Buffed! +{damageBonus * 100:F0}% damage, +{speedBonus * 100:F0}% speed!");
            }
            
            player.PrintToChat($" {ChatColors.Gold}[Battle Standard]{ChatColors.Default} Ultimate! Buffed {buffedCount} allies for {duration:F1}s!");
        }
    }
}
