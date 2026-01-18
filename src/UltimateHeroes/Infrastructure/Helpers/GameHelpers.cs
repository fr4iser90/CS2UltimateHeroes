using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace UltimateHeroes.Infrastructure.Helpers
{
    /// <summary>
    /// Helper-Methoden für Gameplay-Funktionen
    /// </summary>
    public static class GameHelpers
    {
        /// <summary>
        /// Berechnet Position vor dem Spieler in Blickrichtung
        /// </summary>
        public static Vector CalculatePositionInFront(CCSPlayerController player, float distance, float heightOffset = 0)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null)
                return Vector.Zero;

            var pawn = player.PlayerPawn.Value;
            if (pawn.AbsOrigin == null || pawn.EyeAngles == null)
                return Vector.Zero;

            var origin = pawn.AbsOrigin;
            var angles = pawn.EyeAngles;

            // Convert angles to radians
            var pitch = angles.X * (System.Math.PI / 180.0);
            var yaw = angles.Y * (System.Math.PI / 180.0);

            // Calculate forward direction
            var forwardX = System.Math.Cos(pitch) * System.Math.Cos(yaw);
            var forwardY = System.Math.Cos(pitch) * System.Math.Sin(yaw);
            var forwardZ = -System.Math.Sin(pitch);

            // Calculate destination
            var destX = origin.X + (float)forwardX * distance;
            var destY = origin.Y + (float)forwardY * distance;
            var destZ = origin.Z + (float)forwardZ * distance + heightOffset;

            return new Vector(destX, destY, destZ);
        }

        /// <summary>
        /// Findet alle Spieler in einem Radius
        /// </summary>
        public static List<CCSPlayerController> GetPlayersInRadius(Vector position, float radius)
        {
            var players = new List<CCSPlayerController>();
            
            foreach (var player in Utilities.GetPlayers().Where(p => p != null && p.IsValid && p.PlayerPawn.Value != null))
            {
                var pawn = player.PlayerPawn.Value;
                if (pawn.AbsOrigin == null) continue;

                var distance = Vector.Distance(position, pawn.AbsOrigin);
                if (distance <= radius)
                {
                    players.Add(player);
                }
            }

            return players;
        }

        /// <summary>
        /// Heilt einen Spieler
        /// </summary>
        public static void HealPlayer(CCSPlayerController player, int amount)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null)
                return;

            var pawn = player.PlayerPawn.Value;
            if (pawn.Health == null) return;

            var currentHealth = pawn.Health.Value;
            var maxHealth = pawn.MaxHealth;
            var newHealth = System.Math.Min(currentHealth + amount, maxHealth);

            pawn.Health.Value = newHealth;
        }

        /// <summary>
        /// Fügt einem Spieler Schaden zu (mit Talent + Item Modifiers)
        /// </summary>
        public static void DamagePlayer(CCSPlayerController player, int damage, CCSPlayerController? attacker = null, Dictionary<string, float>? attackerModifiers = null, Dictionary<string, float>? itemModifiers = null)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null)
                return;

            var pawn = player.PlayerPawn.Value;
            if (pawn.Health == null) return;

            // Apply talent modifiers to damage (if attacker has modifiers)
            if (attackerModifiers != null)
            {
                if (attackerModifiers.TryGetValue("damage_bonus", out var damageBonus))
                {
                    damage = (int)(damage * (1f + damageBonus));
                }
                
                if (attackerModifiers.TryGetValue("headshot_bonus", out var headshotBonus))
                {
                    // Headshot bonus is applied separately when headshot is detected
                }
            }
            
            // Apply item modifiers (shop items)
            if (itemModifiers != null)
            {
                if (itemModifiers.TryGetValue("damage_boost", out var itemDamageBoost))
                {
                    damage = (int)(damage * (1f + itemDamageBoost));
                }
            }

            var currentHealth = pawn.Health.Value;
            var newHealth = System.Math.Max(currentHealth - damage, 0);

            pawn.Health.Value = newHealth;

            // TODO: Trigger hurt event if needed
        }

        /// <summary>
        /// Macht einen Spieler unsichtbar
        /// </summary>
        public static void MakePlayerInvisible(CCSPlayerController player, bool invisible = true)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null)
                return;

            var pawn = player.PlayerPawn.Value;
            
            // Set render mode to transparent
            if (invisible)
            {
                pawn.RenderMode = RenderMode_t.kRenderTransAlpha;
                pawn.Render = System.Drawing.Color.FromArgb(0, 255, 255, 255); // Fully transparent
            }
            else
            {
                pawn.RenderMode = RenderMode_t.kRenderNormal;
                pawn.Render = System.Drawing.Color.FromArgb(255, 255, 255, 255); // Fully visible
            }
        }

        /// <summary>
        /// Spawnt ein Particle-System
        /// </summary>
        public static void SpawnParticle(Vector position, string particleName, float duration = 5f)
        {
            var particle = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system");
            if (particle == null || !particle.IsValid) return;

            particle.EffectName = particleName;
            particle.Teleport(position, new QAngle(), new Vector());
            particle.StartActive = true;
            particle.DispatchSpawn();

            Server.NextFrame(() =>
            {
                if (particle != null && particle.IsValid)
                {
                    particle.Remove();
                }
            });
        }

        /// <summary>
        /// Teleportiert einen Spieler zu einer Position
        /// </summary>
        public static void TeleportPlayer(CCSPlayerController player, Vector destination, QAngle? rotation = null)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null)
                return;

            var pawn = player.PlayerPawn.Value;
            var finalRotation = rotation ?? pawn.AbsRotation;

            pawn.Teleport(destination, finalRotation, new Vector());
        }
        
        /// <summary>
        /// Setzt Armor eines Spielers
        /// </summary>
        public static void SetArmor(CCSPlayerController player, int armor, int maxArmor = 100)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null)
                return;

            var pawn = player.PlayerPawn.Value;
            pawn.ArmorValue = System.Math.Min(armor, maxArmor);
        }
        
        /// <summary>
        /// Fügt Armor hinzu
        /// </summary>
        public static void AddArmor(CCSPlayerController player, int amount, int maxArmor = 100)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null)
                return;

            var pawn = player.PlayerPawn.Value;
            var newArmor = System.Math.Min(pawn.ArmorValue + amount, maxArmor);
            pawn.ArmorValue = newArmor;
        }
        
        /// <summary>
        /// Setzt Movement Speed Multiplier
        /// </summary>
        public static void SetMovementSpeed(CCSPlayerController player, float speedMultiplier)
        {
            if (player == null || !player.IsValid || player.PlayerPawn.Value == null)
                return;

            var pawn = player.PlayerPawn.Value;
            if (pawn.MovementServices != null)
            {
                pawn.MovementServices.MoveSpeedFactor = speedMultiplier;
            }
        }
    }
}
