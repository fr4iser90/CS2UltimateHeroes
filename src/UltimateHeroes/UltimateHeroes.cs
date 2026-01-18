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
using System.IO;
using System.Text.Json.Serialization;
using UltimateHeroes.Application;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Heroes.ConcreteHeroes;
using UltimateHeroes.Domain.Skills.ConcreteSkills;
using UltimateHeroes.Infrastructure.Cooldown;
using UltimateHeroes.Infrastructure.Database;
using UltimateHeroes.Infrastructure.Database.Repositories;
using UltimateHeroes.Infrastructure.Events;
using UltimateHeroes.Infrastructure.Events.EventHandlers;
using UltimateHeroes.Presentation.Menu;

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
        
        // Services
        private Database? _database;
        private IPlayerRepository? _playerRepository;
        private IBuildRepository? _buildRepository;
        private ICooldownManager? _cooldownManager;
        private EventSystem? _eventSystem;
        private IPlayerService? _playerService;
        private IHeroService? _heroService;
        private ISkillService? _skillService;
        private IBuildService? _buildService;
        private IXpService? _xpService;
        
        // Event Handlers
        private PlayerKillHandler? _playerKillHandler;
        private PlayerHurtHandler? _playerHurtHandler;
        
        // Menus
        private HeroMenu? _heroMenu;
        private BuildMenu? _buildMenu;
        private SkillMenu? _skillMenu;

        public override void Load(bool hotReload)
        {
            // Initialize Database
            var dbPath = Path.Combine(Server.GameDirectory, "csgo", "addons", "counterstrikesharp", "plugins", "UltimateHeroes", "ultimateheroes.db");
            _database = new Database(dbPath);
            
            // Initialize Repositories
            _playerRepository = new PlayerRepository(_database);
            _buildRepository = new BuildRepository(_database);
            
            // Initialize Infrastructure
            _cooldownManager = new CooldownManager();
            _eventSystem = new EventSystem();
            
            // Initialize Services
            _playerService = new PlayerService(_playerRepository);
            _heroService = new HeroService();
            _skillService = new SkillService(_cooldownManager, _playerService);
            var buildValidator = new BuildValidator();
            _buildService = new BuildService(_buildRepository, _heroService, _skillService, buildValidator, _playerService);
            _xpService = new XpService(_playerRepository, _playerService);
            
            // Register Heroes
            _heroService.RegisterHeroes(new System.Collections.Generic.List<Domain.Heroes.IHero>
            {
                new Vanguard()
            });
            
            // Register Skills
            _skillService.RegisterSkills(new System.Collections.Generic.List<Domain.Skills.ISkill>
            {
                new Fireball(),
                new Domain.Skills.ConcreteSkills.ArmorPerKillPassive()
            });
            
            // Register Event Handlers
            _playerKillHandler = new PlayerKillHandler(_xpService, _playerService);
            _playerHurtHandler = new PlayerHurtHandler(_playerService);
            _eventSystem.RegisterHandler<PlayerKillEvent>(_playerKillHandler);
            _eventSystem.RegisterHandler<PlayerHurtEvent>(_playerHurtHandler);
            
            // Initialize Menus
            _heroMenu = new HeroMenu(_heroService, _playerService);
            _buildMenu = new BuildMenu(_buildService, _playerService);
            _skillMenu = new SkillMenu(_skillService, _playerService, _cooldownManager);
            
            // Register CounterStrikeSharp Events
            RegisterListener<Listeners.OnMapStart>(OnMapStart);
            RegisterListener<Listeners.OnClientConnect>(OnClientConnect);
            RegisterListener<Listeners.OnClientDisconnect>(OnClientDisconnect);
            RegisterListener<Listeners.OnPlayerSpawn>(OnPlayerSpawn);
            RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
            RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt);
            
            // Register Commands
            RegisterCommand("css_hero", "Open hero selection menu", OnHeroCommand);
            RegisterCommand("css_build", "Open build menu", OnBuildCommand);
            RegisterCommand("css_skills", "Open skills menu", OnSkillsCommand);
            
            Console.WriteLine("[UltimateHeroes] Plugin loaded successfully!");
        }

        private void OnMapStart(string mapName)
        {
            Console.WriteLine($"[UltimateHeroes] Map started: {mapName}");
        }
        
        private void OnClientConnect(int playerSlot)
        {
            var player = Utilities.GetPlayerFromSlot(playerSlot);
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            _playerService?.OnPlayerConnect(steamId, player);
        }
        
        private void OnClientDisconnect(int playerSlot)
        {
            var player = Utilities.GetPlayerFromSlot(playerSlot);
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            _playerService?.OnPlayerDisconnect(steamId);
        }
        
        private void OnPlayerSpawn(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            _playerService?.OnPlayerSpawn(steamId, player);
        }
        
        private HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
        {
            var attacker = @event.Attacker;
            var victim = @event.Userid;
            
            if (attacker != null && attacker.IsValid && attacker.AuthorizedSteamID != null &&
                victim != null && victim.IsValid && victim.AuthorizedSteamID != null)
            {
                var killerSteamId = attacker.AuthorizedSteamID.SteamId64.ToString();
                var victimSteamId = victim.AuthorizedSteamID.SteamId64.ToString();
                var isHeadshot = @event.Headshot;
                
                var killEvent = new PlayerKillEvent
                {
                    KillerSteamId = killerSteamId,
                    VictimSteamId = victimSteamId,
                    Killer = attacker,
                    Victim = victim,
                    IsHeadshot = isHeadshot
                };
                
                _eventSystem?.Dispatch(killEvent);
            }
            
            return HookResult.Continue;
        }
        
        private HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
        {
            var attacker = @event.Attacker;
            var victim = @event.Userid;
            
            if (attacker != null && attacker.IsValid && attacker.AuthorizedSteamID != null &&
                victim != null && victim.IsValid && victim.AuthorizedSteamID != null)
            {
                var attackerSteamId = attacker.AuthorizedSteamID.SteamId64.ToString();
                var victimSteamId = victim.AuthorizedSteamID.SteamId64.ToString();
                var damage = @event.DmgHealth;
                
                var hurtEvent = new PlayerHurtEvent
                {
                    AttackerSteamId = attackerSteamId,
                    VictimSteamId = victimSteamId,
                    Attacker = attacker,
                    Player = victim,
                    Damage = damage
                };
                
                _eventSystem?.Dispatch(hurtEvent);
            }
            
            return HookResult.Continue;
        }

        private void OnHeroCommand(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid) return;
            _heroMenu?.ShowMenu(player);
        }

        private void OnBuildCommand(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid) return;
            _buildMenu?.ShowMenu(player);
        }
        
        private void OnSkillsCommand(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid) return;
            _skillMenu?.ShowMenu(player);
        }
    }
}
