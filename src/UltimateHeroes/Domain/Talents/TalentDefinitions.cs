using System.Collections.Generic;

namespace UltimateHeroes.Domain.Talents
{
    /// <summary>
    /// Statische Talent Definitions
    /// </summary>
    public static class TalentDefinitions
    {
        public static List<TalentNode> GetCombatTalents()
        {
            return new List<TalentNode>
            {
                new TalentNode
                {
                    Id = "combat_headshot_damage",
                    DisplayName = "Headshot Damage",
                    Description = "Increases headshot damage by 5% per level",
                    TreeType = TalentTreeType.Combat,
                    Row = 1,
                    Column = 1,
                    MaxLevel = 5,
                    Effect = new TalentEffect
                    {
                        Type = TalentEffectType.HeadshotDamage,
                        Parameters = new Dictionary<string, float>
                        {
                            { "headshot_bonus", 0.05f } // +5% per level
                        }
                    }
                },
                new TalentNode
                {
                    Id = "combat_recoil_control",
                    DisplayName = "Recoil Control",
                    Description = "Reduces recoil by 5% per level",
                    TreeType = TalentTreeType.Combat,
                    Row = 1,
                    Column = 2,
                    MaxLevel = 5,
                    Effect = new TalentEffect
                    {
                        Type = TalentEffectType.RecoilReduction,
                        Parameters = new Dictionary<string, float>
                        {
                            { "recoil_reduction", 0.05f } // -5% per level
                        }
                    }
                },
                new TalentNode
                {
                    Id = "combat_armor_penetration",
                    DisplayName = "Armor Penetration",
                    Description = "Increases armor penetration by 2% per level",
                    TreeType = TalentTreeType.Combat,
                    Row = 1,
                    Column = 3,
                    MaxLevel = 5,
                    Effect = new TalentEffect
                    {
                        Type = TalentEffectType.ArmorPenetration,
                        Parameters = new Dictionary<string, float>
                        {
                            { "armor_pen", 0.02f } // +2% per level
                        }
                    }
                },
                new TalentNode
                {
                    Id = "combat_damage_per_kill",
                    DisplayName = "Damage per Kill",
                    Description = "Increases damage by 1 per kill (stacks)",
                    TreeType = TalentTreeType.Combat,
                    Row = 2,
                    Column = 1,
                    MaxLevel = 5,
                    Prerequisites = new List<string> { "combat_headshot_damage" },
                    Effect = new TalentEffect
                    {
                        Type = TalentEffectType.DamageBonus,
                        Parameters = new Dictionary<string, float>
                        {
                            { "damage_per_kill", 1f } // +1 per level
                        }
                    }
                },
                new TalentNode
                {
                    Id = "combat_reload_speed",
                    DisplayName = "Reload Speed",
                    Description = "Increases reload speed by 10% per level",
                    TreeType = TalentTreeType.Combat,
                    Row = 2,
                    Column = 2,
                    MaxLevel = 5,
                    Prerequisites = new List<string> { "combat_recoil_control" },
                    Effect = new TalentEffect
                    {
                        Type = TalentEffectType.ReloadSpeed,
                        Parameters = new Dictionary<string, float>
                        {
                            { "reload_speed", 0.10f } // +10% per level
                        }
                    }
                },
                new TalentNode
                {
                    Id = "combat_weapon_accuracy",
                    DisplayName = "Weapon Accuracy",
                    Description = "Increases weapon accuracy by 5% per level",
                    TreeType = TalentTreeType.Combat,
                    Row = 2,
                    Column = 3,
                    MaxLevel = 5,
                    Prerequisites = new List<string> { "combat_armor_penetration" },
                    Effect = new TalentEffect
                    {
                        Type = TalentEffectType.WeaponAccuracy,
                        Parameters = new Dictionary<string, float>
                        {
                            { "accuracy_bonus", 0.05f } // +5% per level
                        }
                    }
                }
            };
        }
        
        public static List<TalentNode> GetUtilityTalents()
        {
            return new List<TalentNode>
            {
                new TalentNode
                {
                    Id = "utility_extra_nade",
                    DisplayName = "Extra Nade",
                    Description = "Gives you an extra grenade",
                    TreeType = TalentTreeType.Utility,
                    Row = 1,
                    Column = 1,
                    MaxLevel = 3,
                    Effect = new TalentEffect
                    {
                        Type = TalentEffectType.ExtraNade,
                        Parameters = new Dictionary<string, float>
                        {
                            { "extra_nade", 1f } // +1 per level
                        }
                    }
                },
                new TalentNode
                {
                    Id = "utility_faster_plant",
                    DisplayName = "Faster Plant",
                    Description = "Reduces plant time by 0.5s per level",
                    TreeType = TalentTreeType.Utility,
                    Row = 1,
                    Column = 2,
                    MaxLevel = 5,
                    Effect = new TalentEffect
                    {
                        Type = TalentEffectType.PlantSpeed,
                        Parameters = new Dictionary<string, float>
                        {
                            { "plant_speed", 0.5f } // -0.5s per level
                        }
                    }
                },
                new TalentNode
                {
                    Id = "utility_defuse_speed",
                    DisplayName = "Defuse Speed",
                    Description = "Increases defuse speed by 10% per level",
                    TreeType = TalentTreeType.Utility,
                    Row = 1,
                    Column = 3,
                    MaxLevel = 5,
                    Effect = new TalentEffect
                    {
                        Type = TalentEffectType.DefuseSpeed,
                        Parameters = new Dictionary<string, float>
                        {
                            { "defuse_speed", 0.10f } // +10% per level
                        }
                    }
                }
            };
        }
        
        public static List<TalentNode> GetMovementTalents()
        {
            return new List<TalentNode>
            {
                new TalentNode
                {
                    Id = "movement_air_control",
                    DisplayName = "Air Control",
                    Description = "Increases air control by 10% per level",
                    TreeType = TalentTreeType.Movement,
                    Row = 1,
                    Column = 1,
                    MaxLevel = 5,
                    Effect = new TalentEffect
                    {
                        Type = TalentEffectType.AirControl,
                        Parameters = new Dictionary<string, float>
                        {
                            { "air_control", 0.10f } // +10% per level
                        }
                    }
                },
                new TalentNode
                {
                    Id = "movement_ladder_speed",
                    DisplayName = "Faster Ladder",
                    Description = "Increases ladder speed by 15% per level",
                    TreeType = TalentTreeType.Movement,
                    Row = 1,
                    Column = 2,
                    MaxLevel = 5,
                    Effect = new TalentEffect
                    {
                        Type = TalentEffectType.LadderSpeed,
                        Parameters = new Dictionary<string, float>
                        {
                            { "ladder_speed", 0.15f } // +15% per level
                        }
                    }
                },
                new TalentNode
                {
                    Id = "movement_silent_drop",
                    DisplayName = "Silent Drop",
                    Description = "Makes your drops silent",
                    TreeType = TalentTreeType.Movement,
                    Row = 1,
                    Column = 3,
                    MaxLevel = 1,
                    Effect = new TalentEffect
                    {
                        Type = TalentEffectType.SilentDrop,
                        Parameters = new Dictionary<string, float>
                        {
                            { "silent_drop", 1f } // On/off
                        }
                    }
                },
                new TalentNode
                {
                    Id = "movement_speed",
                    DisplayName = "Movement Speed",
                    Description = "Increases movement speed by 5% per level",
                    TreeType = TalentTreeType.Movement,
                    Row = 2,
                    Column = 1,
                    MaxLevel = 5,
                    Prerequisites = new List<string> { "movement_air_control" },
                    Effect = new TalentEffect
                    {
                        Type = TalentEffectType.MovementSpeed,
                        Parameters = new Dictionary<string, float>
                        {
                            { "movement_speed", 0.05f } // +5% per level
                        }
                    }
                },
                new TalentNode
                {
                    Id = "movement_jump_height",
                    DisplayName = "Jump Height",
                    Description = "Increases jump height by 10% per level",
                    TreeType = TalentTreeType.Movement,
                    Row = 2,
                    Column = 2,
                    MaxLevel = 5,
                    Prerequisites = new List<string> { "movement_ladder_speed" },
                    Effect = new TalentEffect
                    {
                        Type = TalentEffectType.JumpHeight,
                        Parameters = new Dictionary<string, float>
                        {
                            { "jump_height", 0.10f } // +10% per level
                        }
                    }
                },
                new TalentNode
                {
                    Id = "movement_fall_damage_reduction",
                    DisplayName = "Fall Damage Reduction",
                    Description = "Reduces fall damage by 20% per level",
                    TreeType = TalentTreeType.Movement,
                    Row = 2,
                    Column = 3,
                    MaxLevel = 5,
                    Prerequisites = new List<string> { "movement_silent_drop" },
                    Effect = new TalentEffect
                    {
                        Type = TalentEffectType.FallDamageReduction,
                        Parameters = new Dictionary<string, float>
                        {
                            { "fall_damage_reduction", 0.20f } // -20% per level
                        }
                    }
                }
            };
        }
    }
}
