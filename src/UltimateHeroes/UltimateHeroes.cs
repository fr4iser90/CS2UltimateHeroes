/*
 * Ultimate Heroes Mod for CS2
 * 
 * Copyright (C) 2024 fr4iser
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using System;
using System.Text.Json.Serialization;

namespace UltimateHeroes
{
    public class Config : BasePluginConfig
    {
        [JsonPropertyName("ConfigVersion")]
        public override int Version { get; set; } = 1;

        [JsonPropertyName("DefaultHero")]
        public string DefaultHero { get; set; } = "vanguard";

        [JsonPropertyName("MaxSkillSlots")]
        public int MaxSkillSlots { get; set; } = 3;

        [JsonPropertyName("MaxPowerBudget")]
        public int MaxPowerBudget { get; set; } = 100;

        [JsonPropertyName("XpPerKill")]
        public float XpPerKill { get; set; } = 10f;
    }

    public class UltimateHeroes : BasePlugin, IPluginConfig<Config>
    {
        public override string ModuleName => "Ultimate Heroes";
        public override string ModuleVersion => "0.1.0";

        public Config Config { get; set; } = null!;

        public override void Load(bool hotReload)
        {
            RegisterListener<Listeners.OnMapStart>(OnMapStart);
            RegisterCommand("css_hero", "Open hero selection menu", OnHeroCommand);
            RegisterCommand("css_build", "Open build menu", OnBuildCommand);
        }

        private void OnMapStart(string mapName)
        {
            Console.WriteLine($"[UltimateHeroes] Map started: {mapName}");
        }

        private void OnHeroCommand(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid) return;

            player.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} Hero system loaded!");
        }

        private void OnBuildCommand(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid) return;

            player.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} Build system loaded!");
        }
    }
}
