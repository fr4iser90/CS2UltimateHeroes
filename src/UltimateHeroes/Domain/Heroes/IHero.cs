using System.Collections.Generic;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Domain.Skills;

namespace UltimateHeroes.Domain.Heroes
{
    /// <summary>
    /// Interface für einen Hero Core
    /// </summary>
    public interface IHero
    {
        string Id { get; }
        string DisplayName { get; }
        string Description { get; }
        
        /// <summary>
        /// Power Weight des Hero Cores (für Power Budget System)
        /// </summary>
        int PowerWeight { get; }
        
        /// <summary>
        /// Passive Fähigkeiten (immer aktiv)
        /// </summary>
        List<IPassiveSkill> PassiveSkills { get; }
        
        /// <summary>
        /// Hero Identity Auras (Modifier für Skills)
        /// </summary>
        HeroIdentity Identity { get; }
        
        /// <summary>
        /// Wird aufgerufen wenn ein Spieler spawnt
        /// </summary>
        void OnPlayerSpawn(CCSPlayerController player);
    }
}
