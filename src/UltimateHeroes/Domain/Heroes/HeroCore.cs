using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Skills;

namespace UltimateHeroes.Domain.Heroes
{
    /// <summary>
    /// Basis-Klasse für alle Hero Cores
    /// </summary>
    public abstract class HeroCore : IHero
    {
        public abstract string Id { get; }
        public abstract string DisplayName { get; }
        public abstract string Description { get; }
        public abstract int PowerWeight { get; }
        public abstract List<IPassiveSkill> PassiveSkills { get; }
        public abstract HeroIdentity Identity { get; }
        
        /// <summary>
        /// Initialisiert den Hero für einen Spieler
        /// </summary>
        public virtual void Initialize(CCSPlayerController player)
        {
            if (player == null || !player.IsValid) return;
            
            // Aktiviere Passive Skills
            foreach (var passive in PassiveSkills)
            {
                passive.OnPlayerSpawn(player);
            }
        }
        
        /// <summary>
        /// Wird aufgerufen wenn ein Spieler spawnt
        /// </summary>
        public virtual void OnPlayerSpawn(CCSPlayerController player)
        {
            Initialize(player);
        }
        
        /// <summary>
        /// Wird aufgerufen wenn eine Runde startet
        /// </summary>
        public virtual void OnRoundStart(CCSPlayerController player)
        {
            // Kann von konkreten Heroes überschrieben werden
        }
    }
}
