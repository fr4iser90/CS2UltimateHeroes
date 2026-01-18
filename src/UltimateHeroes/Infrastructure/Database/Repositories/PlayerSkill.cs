namespace UltimateHeroes.Infrastructure.Database.Repositories
{
    /// <summary>
    /// DTO für Player Skills (Database Mapping)
    /// </summary>
    public class PlayerSkill
    {
        public string SteamId { get; set; } = string.Empty;
        public string SkillId { get; set; } = string.Empty;
        public int SkillLevel { get; set; } = 1;
    }
}
