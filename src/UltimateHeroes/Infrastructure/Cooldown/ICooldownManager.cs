namespace UltimateHeroes.Infrastructure.Cooldown
{
    /// <summary>
    /// Interface für Cooldown Management
    /// </summary>
    public interface ICooldownManager
    {
        void SetCooldown(string steamId, string skillId, float cooldownSeconds);
        float GetCooldown(string steamId, string skillId);
        bool IsReady(string steamId, string skillId);
        void ClearCooldown(string steamId, string skillId);
        void ClearAllCooldowns(string steamId);
    }
}
