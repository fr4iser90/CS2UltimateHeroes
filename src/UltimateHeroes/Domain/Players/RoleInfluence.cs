namespace UltimateHeroes.Domain.Players
{
    /// <summary>
    /// Role Influence - Automatisch erkannte Rolle des Spielers
    /// </summary>
    public enum RoleInfluence
    {
        None,
        DPS,        // Viel Damage
        Support,    // Viele Smokes/Heals
        Initiator,  // Entry Kills
        Clutch      // Clutch Rounds
    }
}
