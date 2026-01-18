using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Effects;
using UltimateHeroes.Infrastructure.Effects.ConcreteEffects;
using UltimateHeroes.Application.Helpers;

namespace UltimateHeroes.Domain.Skills.ConcreteSkills
{
    /// <summary>
    /// Execution Mark - Ultimate Skill: Markierter Gegner nimmt extrem erhöhten Schaden
    /// </summary>
    public class ExecutionMark : ActiveSkillBase, IUltimateSkill
    {
        public override string Id => "execution_mark";
        public override string DisplayName => "Execution Mark";
        public override string Description => "Markierter Gegner nimmt extrem erhöhten Schaden";
        public override int PowerWeight => 50;
        public override List<SkillTag> Tags => new() { SkillTag.Damage, SkillTag.Ultimate };
        public override int MaxLevel => 3;
        
        public override float Cooldown => 100f;
        
        private const float BaseDuration = 15f;
        private const float BaseDamageMultiplier = 2.0f;
        private const float BaseRange = 800f;
        
        // Buff Definition (wird einmal erstellt, kann wiederverwendet werden)
        private static readonly Domain.Buffs.BuffDefinition ExecutionMarkBuffDefinition = new()
        {
            Id = "execution_mark",
            DisplayName = "Execution Mark",
            Type = Domain.Buffs.BuffType.ExecutionMark,
            StackingType = Domain.Buffs.BuffStackingType.Refresh
        };
        
        // BuffService wird über Helper gesetzt
        public override void Activate(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null) return;
            
            var pawn = player.PlayerPawn.Value;
            if (pawn.AbsOrigin == null || pawn.EyeAngles == null) return;
            
            // Calculate stats based on level
            var duration = BaseDuration + (CurrentLevel * 5f);
            var damageMultiplier = BaseDamageMultiplier + (CurrentLevel * 0.3f); // 2.0x - 2.6x
            var range = BaseRange + (CurrentLevel * 200);
            
            // Calculate target position (in front of player)
            var targetPos = GameHelpers.CalculatePositionInFront(player, range, 0);
            
            if (targetPos == Vector.Zero) return;
            
            // Find closest enemy in range
            CCSPlayerController? target = null;
            float closestDistance = float.MaxValue;
            
            foreach (var enemy in Utilities.GetPlayers())
            {
                if (enemy == player) continue;
                if (!enemy.IsValid || enemy.PlayerPawn.Value == null || enemy.AuthorizedSteamID == null) continue;
                
                var enemyPawn = enemy.PlayerPawn.Value;
                if (enemyPawn.AbsOrigin == null) continue;
                
                var distance = Vector.Distance(targetPos, enemyPawn.AbsOrigin);
                if (distance < range && distance < closestDistance)
                {
                    closestDistance = distance;
                    target = enemy;
                }
            }
            
            if (target == null || !target.IsValid || target.AuthorizedSteamID == null)
            {
                player.PrintToChat($" {ChatColors.Red}[Execution Mark]{ChatColors.Default} No target found!");
                return;
            }
            
            // Create Execution Mark Buff from Definition (generisch)
            var buffService = Infrastructure.Helpers.BuffServiceHelper.GetBuffService();
            if (buffService != null)
            {
                var targetSteamId = target.AuthorizedSteamID.SteamId64.ToString();
                var executionMarkBuff = ExecutionMarkBuffDefinition.CreateBuff(
                    duration,
                    new System.Collections.Generic.Dictionary<string, float>
                    {
                        { "damage_multiplier", damageMultiplier }
                    }
                );
                buffService.ApplyBuff(targetSteamId, executionMarkBuff);
            }
            
            // Spawn mark particle
            if (target.PlayerPawn.Value?.AbsOrigin != null)
            {
                var markPos = target.PlayerPawn.Value.AbsOrigin;
                GameHelpers.SpawnParticle(markPos, "particles/ui/ui_electric_exp_glow.vpcf", duration);
            }
            
            player.PrintToChat($" {ChatColors.Gold}[Execution Mark]{ChatColors.Default} Ultimate! Marked {target.PlayerName} for {duration:F1}s!");
            target.PrintToChat($" {ChatColors.Red}[Execution Mark]{ChatColors.Default} You are marked! Take {damageMultiplier:F1}x damage!");
        }
    }
}
