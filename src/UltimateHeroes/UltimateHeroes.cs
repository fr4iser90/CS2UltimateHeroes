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
using UltimateHeroes.Infrastructure.Effects;
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
        private EffectManager? _effectManager;
        private EventSystem? _eventSystem;
        private IPlayerService? _playerService;
        private IHeroService? _heroService;
        private ISkillService? _skillService;
        private IBuildService? _buildService;
        private IXpService? _xpService;
        private ITalentService? _talentService;
        private Application.Services.IMasteryService? _masteryService;
        
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
            var dbPath = Path.Combine(ModuleDirectory, "ultimateheroes.db");
            _database = new Database(dbPath);
            
            // Initialize Repositories
            _playerRepository = new PlayerRepository(_database);
            _buildRepository = new BuildRepository(_database);
            var talentRepository = new Infrastructure.Database.Repositories.TalentRepository(_database);
            var masteryRepository = new Infrastructure.Database.Repositories.MasteryRepository(_database);
            
            // Initialize Infrastructure
            _cooldownManager = new CooldownManager();
            _effectManager = new EffectManager();
            _eventSystem = new EventSystem();
            
            // Initialize Services
            _playerService = new PlayerService(_playerRepository);
            _heroService = new HeroService();
            _masteryService = new Application.Services.MasteryService(masteryRepository);
            _skillService = new SkillService(_cooldownManager, _playerService, _effectManager, _masteryService);
            
            // Set SkillService in Helper for Damage Tracking
            Infrastructure.Helpers.SkillServiceHelper.SetSkillService(_skillService);
            var buildValidator = new BuildValidator();
            _buildService = new BuildService(_buildRepository, _heroService, _skillService, buildValidator, _playerService);
            _talentService = new TalentService(talentRepository);
            _xpService = new XpService(_playerRepository, _playerService, _talentService);
            
            // Set TalentService in PlayerService for Talent Modifiers
            _playerService.SetTalentService(_talentService);
            
            // Register Heroes
            _heroService.RegisterHeroes(new System.Collections.Generic.List<Domain.Heroes.IHero>
            {
                new Vanguard(),
                new Domain.Heroes.ConcreteHeroes.Phantom(),
                new Domain.Heroes.ConcreteHeroes.Engineer()
            });
            
            // Register Skills
            _skillService.RegisterSkills(new System.Collections.Generic.List<Domain.Skills.ISkill>
            {
                new Fireball(),
                new Domain.Skills.ConcreteSkills.Blink(),
                new Domain.Skills.ConcreteSkills.Stealth(),
                new Domain.Skills.ConcreteSkills.HealingAura(),
                new Domain.Skills.ConcreteSkills.Teleport(),
                new Domain.Skills.ConcreteSkills.ArmorPerKillPassive(),
                new Domain.Skills.ConcreteSkills.SilentFootstepsPassive()
            });
            
            // Register Event Handlers
            _playerKillHandler = new PlayerKillHandler(_xpService, _playerService, _masteryService);
            _playerHurtHandler = new PlayerHurtHandler(_playerService);
            _eventSystem.RegisterHandler<PlayerKillEvent>(_playerKillHandler);
            _eventSystem.RegisterHandler<PlayerHurtEvent>(_playerHurtHandler);
            
            // Initialize Menu System
            Presentation.Menu.MenuAPI.Load(this);
            
            // Initialize Menus
            _heroMenu = new HeroMenu(_heroService, _playerService);
            _buildMenu = new BuildMenu(_buildService, _playerService);
            _skillMenu = new SkillMenu(_skillService, _playerService, _cooldownManager);
            _talentMenu = new Presentation.Menu.TalentMenu(_talentService, _playerService);
            
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
            RegisterCommand("css_talents", "Open talents menu", OnTalentsCommand);
            RegisterCommand("css_selecthero", "Select a hero", OnSelectHeroCommand);
            RegisterCommand("css_createbuild", "Create a build", OnCreateBuildCommand);
            RegisterCommand("css_activatebuild", "Activate a build", OnActivateBuildCommand);
            RegisterCommand("css_use", "Use a skill", OnUseSkillCommand);
            RegisterCommand("css_stats", "Show player stats", OnStatsCommand);
            
            // Skill Keybindings (can be bound to keys)
            RegisterCommand("css_skill1", "Activate skill slot 1", OnSkill1Command);
            RegisterCommand("css_skill2", "Activate skill slot 2", OnSkill2Command);
            RegisterCommand("css_skill3", "Activate skill slot 3", OnSkill3Command);
            RegisterCommand("css_ultimate", "Activate ultimate skill", OnUltimateCommand);
            
            Console.WriteLine("[UltimateHeroes] Plugin loaded successfully!");
        }

        private void OnMapStart(string mapName)
        {
            Console.WriteLine($"[UltimateHeroes] Map started: {mapName}");
            
            // Start effect tick timer (every 0.5 seconds)
            AddTimer(0.5f, () =>
            {
                _effectManager?.TickEffects();
            }, TimerFlags.REPEAT);
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
            var playerState = _playerService?.GetPlayer(steamId);
            
            // Set default hero if no hero is selected
            if (playerState != null && playerState.CurrentHero == null)
            {
                var defaultHero = _heroService?.GetHero(Config.DefaultHero);
                if (defaultHero != null)
                {
                    _heroService?.SetPlayerHero(steamId, Config.DefaultHero);
                    playerState.CurrentHero = defaultHero;
                    _playerService?.SavePlayer(playerState);
                }
            }
            
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
        
        private void OnTalentsCommand(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid) return;
            _talentMenu?.ShowMenu(player);
        }
        
        private void OnSelectHeroCommand(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var args = commandInfo.GetCommandString.Split(' ');
            
            if (args.Length < 2)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Usage: !selecthero <hero_id>");
                player.PrintToChat($" {ChatColors.Gray}Example: !selecthero vanguard");
                return;
            }
            
            var heroId = args[1].ToLower();
            var hero = _heroService?.GetHero(heroId);
            
            if (hero == null)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Hero '{heroId}' not found!");
                _heroMenu?.ShowMenu(player);
                return;
            }
            
            try
            {
                _heroService?.SetPlayerHero(steamId, heroId);
                var playerState = _playerService?.GetPlayer(steamId);
                if (playerState != null)
                {
                    playerState.CurrentHero = hero;
                    _playerService?.SavePlayer(playerState);
                }
                
                player.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} Hero selected: {ChatColors.LightBlue}{hero.DisplayName}");
            }
            catch (Exception ex)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Error: {ex.Message}");
            }
        }
        
        private void OnCreateBuildCommand(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var args = commandInfo.GetCommandString.Split(' ');
            
            if (args.Length < 4)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Usage: !createbuild <slot> <hero> <skill1> [skill2] [skill3] <name>");
                player.PrintToChat($" {ChatColors.Gray}Example: !createbuild 1 vanguard fireball \"My Build\"");
                return;
            }
            
            if (!int.TryParse(args[1], out var buildSlot) || buildSlot < 1 || buildSlot > 5)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Build slot must be 1-5!");
                return;
            }
            
            var heroId = args[2].ToLower();
            var skillIds = new List<string>();
            
            // Parse skills and name (name is last argument, might contain spaces)
            for (int i = 3; i < args.Length; i++)
            {
                if (i == args.Length - 1)
                {
                    // Last argument is the build name
                    break;
                }
                skillIds.Add(args[i].ToLower());
            }
            
            var buildName = args[args.Length - 1];
            
            try
            {
                var build = _buildService?.CreateBuild(steamId, buildSlot, heroId, skillIds, buildName);
                if (build != null)
                {
                    player.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} Build '{buildName}' created in slot {buildSlot}!");
                }
            }
            catch (Exception ex)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Error: {ex.Message}");
            }
        }
        
        private void OnActivateBuildCommand(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var args = commandInfo.GetCommandString.Split(' ');
            
            if (args.Length < 2)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Usage: !activatebuild <slot>");
                return;
            }
            
            if (!int.TryParse(args[1], out var buildSlot) || buildSlot < 1 || buildSlot > 5)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Build slot must be 1-5!");
                return;
            }
            
            try
            {
                _buildService?.ActivateBuild(steamId, buildSlot, player);
                var build = _buildService?.GetBuild(steamId, buildSlot);
                if (build != null)
                {
                    player.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} Build '{build.BuildName}' activated!");
                }
            }
            catch (Exception ex)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Error: {ex.Message}");
            }
        }
        
        private void OnUseSkillCommand(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var args = commandInfo.GetCommandString.Split(' ');
            
            if (args.Length < 2)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Usage: !use <skill_id>");
                player.PrintToChat($" {ChatColors.Gray}Example: !use fireball");
                return;
            }
            
            var skillId = args[1].ToLower();
            
            try
            {
                if (_skillService?.CanActivateSkill(steamId, skillId) == true)
                {
                    _skillService?.ActivateSkill(steamId, skillId, player);
                }
                else
                {
                    var cooldown = _skillService?.GetSkillCooldown(steamId, skillId) ?? 0f;
                    if (cooldown > 0)
                    {
                        player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Skill on cooldown: {cooldown:F1}s remaining");
                    }
                    else
                    {
                        player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Skill '{skillId}' not found or not available!");
                    }
                }
            }
            catch (Exception ex)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Error: {ex.Message}");
            }
        }
        
        private void OnStatsCommand(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var playerState = _playerService?.GetPlayer(steamId);
            
            if (playerState == null)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Player not found!");
                return;
            }
            
            var xpProgress = _xpService?.GetXpProgress(steamId) ?? 0f;
            var xpPercent = (int)(xpProgress * 100);
            
            player.PrintToChat($" {ChatColors.Green}╔════════════════════════════════╗");
            player.PrintToChat($" {ChatColors.Green}║{ChatColors.Default}   Ultimate Heroes - Stats      {ChatColors.Green}║");
            player.PrintToChat($" {ChatColors.Green}╚════════════════════════════════╝");
            player.PrintToChat($" {ChatColors.Yellow}Level: {ChatColors.LightBlue}{playerState.HeroLevel}{ChatColors.Default} | XP: {ChatColors.LightBlue}{playerState.CurrentXp:F0}{ChatColors.Default} ({xpPercent}%)");
            player.PrintToChat($" {ChatColors.Yellow}Current Hero: {ChatColors.LightBlue}{playerState.CurrentHero?.DisplayName ?? "None"}");
            player.PrintToChat($" {ChatColors.Yellow}Active Build: {ChatColors.LightBlue}{playerState.CurrentBuild?.BuildName ?? "None"}");
            player.PrintToChat($"");
            player.PrintToChat($" {ChatColors.Yellow}Match Stats:");
            player.PrintToChat($"   Kills: {ChatColors.LightBlue}{playerState.Kills}{ChatColors.Default} | Deaths: {ChatColors.LightBlue}{playerState.Deaths}{ChatColors.Default} | Assists: {ChatColors.LightBlue}{playerState.Assists}{ChatColors.Default}");
            player.PrintToChat($"   Headshots: {ChatColors.LightBlue}{playerState.Headshots}");
        }
        
        private void OnSkill1Command(CCSPlayerController? player, CommandInfo commandInfo)
        {
            ActivateSkillSlot(player, 0);
        }
        
        private void OnSkill2Command(CCSPlayerController? player, CommandInfo commandInfo)
        {
            ActivateSkillSlot(player, 1);
        }
        
        private void OnSkill3Command(CCSPlayerController? player, CommandInfo commandInfo)
        {
            ActivateSkillSlot(player, 2);
        }
        
        private void OnUltimateCommand(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var playerState = _playerService?.GetPlayer(steamId);
            
            if (playerState == null || playerState.ActiveSkills.Count == 0)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} No active skills!");
                return;
            }
            
            // Find ultimate skill
            var ultimateSkill = playerState.ActiveSkills.FirstOrDefault(s => s.Type == Domain.Skills.SkillType.Ultimate);
            
            if (ultimateSkill == null)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} No ultimate skill available!");
                return;
            }
            
            try
            {
                if (_skillService?.CanActivateSkill(steamId, ultimateSkill.Id) == true)
                {
                    _skillService?.ActivateSkill(steamId, ultimateSkill.Id, player);
                }
                else
                {
                    var cooldown = _skillService?.GetSkillCooldown(steamId, ultimateSkill.Id) ?? 0f;
                    if (cooldown > 0)
                    {
                        player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Ultimate on cooldown: {cooldown:F1}s");
                    }
                }
            }
            catch (Exception ex)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Error: {ex.Message}");
            }
        }
        
        private void ActivateSkillSlot(CCSPlayerController? player, int slotIndex)
        {
            if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return;
            
            var steamId = player.AuthorizedSteamID.SteamId64.ToString();
            var playerState = _playerService?.GetPlayer(steamId);
            
            if (playerState == null || playerState.ActiveSkills.Count == 0)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} No active skills!");
                return;
            }
            
            if (slotIndex < 0 || slotIndex >= playerState.ActiveSkills.Count)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Skill slot {slotIndex + 1} is empty!");
                return;
            }
            
            var skill = playerState.ActiveSkills[slotIndex];
            
            // Skip passive skills
            if (skill.Type == Domain.Skills.SkillType.Passive)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Skill {skill.DisplayName} is passive!");
                return;
            }
            
            try
            {
                if (_skillService?.CanActivateSkill(steamId, skill.Id) == true)
                {
                    _skillService?.ActivateSkill(steamId, skill.Id, player);
                }
                else
                {
                    var cooldown = _skillService?.GetSkillCooldown(steamId, skill.Id) ?? 0f;
                    if (cooldown > 0)
                    {
                        player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} {skill.DisplayName} on cooldown: {cooldown:F1}s");
                    }
                }
            }
            catch (Exception ex)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} Error: {ex.Message}");
            }
        }
    }
}
