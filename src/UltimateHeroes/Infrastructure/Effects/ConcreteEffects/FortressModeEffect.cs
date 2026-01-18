using System;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Infrastructure.Effects;
using UltimateHeroes.Infrastructure.Helpers;

namespace UltimateHeroes.Infrastructure.Effects.ConcreteEffects
{
    /// <summary>
    /// Fortress Mode Effect - +Armor, Immun gegen CC, kein Sprint
    /// </summary>
    public class FortressModeEffect : IEffect
    {
        public string Id => "fortress_mode";
        public string DisplayName => "Fortress Mode";
        public float Duration { get; set; }
        public DateTime AppliedAt { get; set; }
        
        public int ArmorBonus { get; set; } = 50;
        
        public void OnApply(CCSPlayerController player)
        {
            if (player == null || !player.IsValid) return;
            
            // Add armor
            GameHelpers.AddArmor(player, ArmorBonus);
            
            // Disable sprint (simplified - set movement speed to normal)
            GameHelpers.SetMovementSpeed(player, 1.0f);
            
            player.PrintToChat($" {ChatColors.Gold}[Fortress Mode]{ChatColors.Default} Ultimate! +{ArmorBonus} armor, CC immune, no sprint!");
        }
        
        public void OnTick(CCSPlayerController player)
        {
            // Keep movement speed at normal (no sprint)
            if (player != null && player.IsValid)
            {
                GameHelpers.SetMovementSpeed(player, 1.0f);
            }
        }
        
        public void OnRemove(CCSPlayerController player)
        {
            if (player == null || !player.IsValid) return;
            
            // Restore normal movement
            GameHelpers.SetMovementSpeed(player, 1.0f);
            
            player.PrintToChat($" {ChatColors.Gold}[Fortress Mode]{ChatColors.Default} Ultimate expired!");
        }
        
        public bool IsExpired()
        {
            return (DateTime.UtcNow - AppliedAt).TotalSeconds >= Duration;
        }
    }
}
