using UltimateHeroes.Application.Services;

namespace UltimateHeroes.Infrastructure.Helpers
{
    /// <summary>
    /// Static Helper für SkillService (für Damage-Tracking in Skills)
    /// </summary>
    public static class SkillServiceHelper
    {
        private static ISkillService? _skillService;
        
        public static void SetSkillService(ISkillService skillService)
        {
            _skillService = skillService;
        }
        
        public static void TrackSkillDamage(string steamId, string skillId, float damage)
        {
            _skillService?.TrackSkillDamage(steamId, skillId, damage);
        }
    }
}
