using System.Collections.Generic;

namespace UltimateHeroes.Domain.Skills
{
    /// <summary>
    /// Interface für Skills (können in Slots)
    /// </summary>
    public interface ISkill
    {
        string Id { get; }
        string DisplayName { get; }
        string Description { get; }
        
        /// <summary>
        /// Skill-Typ (Passive, Active, Ultimate)
        /// </summary>
        SkillType Type { get; }
        
        /// <summary>
        /// Power Weight (für Power Budget System)
        /// </summary>
        int PowerWeight { get; }
        
        /// <summary>
        /// Skill-Tags (für Rules Engine)
        /// </summary>
        List<SkillTag> Tags { get; }
        
        /// <summary>
        /// Max Level
        /// </summary>
        int MaxLevel { get; }
    }
    
    /// <summary>
    /// Passive Skill (läuft immer)
    /// </summary>
    public interface IPassiveSkill : ISkill
    {
        void OnPlayerSpawn(CounterStrikeSharp.API.Core.CCSPlayerController player);
        void OnPlayerHurt(CounterStrikeSharp.API.Core.CCSPlayerController player, int damage);
        void OnPlayerKill(CounterStrikeSharp.API.Core.CCSPlayerController player, CounterStrikeSharp.API.Core.CCSPlayerController victim);
    }
    
    /// <summary>
    /// Active Skill (mit Cooldown)
    /// </summary>
    public interface IActiveSkill : ISkill
    {
        float Cooldown { get; }
        void Activate(CounterStrikeSharp.API.Core.CCSPlayerController player);
    }
    
    /// <summary>
    /// Ultimate Skill (lange Cooldown)
    /// </summary>
    public interface IUltimateSkill : IActiveSkill
    {
        // Ultimate-spezifische Properties
    }
    
    public enum SkillType
    {
        Passive,
        Active,
        Ultimate
    }
    
    public enum SkillTag
    {
        Damage,
        Mobility,
        CrowdControl,
        Ultimate,
        Stealth,
        Area,
        Support,
        Defense,
        Utility
    }
}
