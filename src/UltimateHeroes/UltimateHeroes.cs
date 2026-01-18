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
        
        [JsonPropertyName("BotSettings")]
        public BotSettingsConfig BotSettings { get; set; } = new();
    }
    
    public class BotSettingsConfig
    {
        [JsonPropertyName("Enabled")]
        public bool Enabled { get; set; } = true;
        
        [JsonPropertyName("DefaultLevelMode")]
        public string DefaultLevelMode { get; set; } = "MatchPlayerAverage"; // Level0, MaxLevel, Random, Fixed, MatchPlayerAverage
        
        [JsonPropertyName("DefaultBuildMode")]
        public string DefaultBuildMode { get; set; } = "Random"; // Random, Predefined, Pool, Rotate, Balanced
        
        [JsonPropertyName("TrackStats")]
        public bool TrackStats { get; set; } = true;
        
        [JsonPropertyName("AutoAssignBuild")]
        public bool AutoAssignBuild { get; set; } = true;
        
        [JsonPropertyName("BuildChangeInterval")]
        public float BuildChangeInterval { get; set; } = 300f; // Sekunden
        
        [JsonPropertyName("PredefinedBuilds")]
        public List<string> PredefinedBuilds { get; set; } = new() { "dps", "mobility", "stealth", "support", "balanced" };
        
        [JsonPropertyName("BuildPool")]
        public List<string> BuildPool { get; set; } = new();

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
        private Application.Services.IInMatchEvolutionService? _inMatchEvolutionService;
        private Application.Services.IRoleInfluenceService? _roleInfluenceService;
        private Application.Services.IBuildIntegrityService? _buildIntegrityService;
        private Application.Services.IBotService? _botService;
        private Application.Services.IShopService? _shopService;
        private Application.Services.IAccountService? _accountService;
        
        // Event Handlers
        private PlayerKillHandler? _playerKillHandler;
        private PlayerHurtHandler? _playerHurtHandler;
        
        // Menus
        private HeroMenu? _heroMenu;
        private BuildMenu? _buildMenu;
        private SkillMenu? _skillMenu;
        private Presentation.Menu.ShopMenu? _shopMenu;
        
        // HUD
        private Presentation.UI.HudManager? _hudManager;

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
            var accountRepository = new Infrastructure.Database.Repositories.AccountRepository(_database);
            
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
            
            // Initialize TalentService first (needed for BuildService)
            _talentService = new TalentService(talentRepository);
            
            // Initialize ShopService
            _shopService = new Application.Services.ShopService();
            
            // Initialize AccountService
            _accountService = new Application.Services.AccountService(accountRepository);
            
            var buildValidator = new BuildValidator();
            _buildService = new BuildService(_buildRepository, _heroService, _skillService, buildValidator, _playerService, _talentService);
            _xpService = new XpService(_playerRepository, _playerService, _talentService, _accountService);
            _inMatchEvolutionService = new Application.Services.InMatchEvolutionService(_playerService);
            _roleInfluenceService = new Application.Services.RoleInfluenceService(_playerService);
            _buildIntegrityService = new Application.Services.BuildIntegrityService(_skillService);
            _botService = new Application.Services.BotService(_playerService, _heroService, _skillService, _buildService, _xpService);
            
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
            _playerKillHandler = new PlayerKillHandler(_xpService, _playerService, _masteryService, _inMatchEvolutionService, _botService);
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
            _shopMenu = new Presentation.Menu.ShopMenu(_shopService, _playerService);
            
            // Initialize HUD
            _hudManager = new Presentation.UI.HudManager(
                _skillService,
                _xpService,
                _playerService,
                _cooldownManager,
                _masteryService,
                _accountService
            );
            
            // Register CounterStrikeSharp Events
            RegisterListener<Listeners.OnMapStart>(OnMapStart);
            RegisterListener<Listeners.OnClientConnect>(OnClientConnect);
            RegisterListener<Listeners.OnClientDisconnect>(OnClientDisconnect);
            RegisterListener<Listeners.OnPlayerSpawn>(OnPlayerSpawn);
            RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
            RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt);
            RegisterEventHandler<EventRoundStart>(OnRoundStart);
            RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
            
            // Register Commands
            RegisterCommand("css_hero", "Open hero selection menu", OnHeroCommand);
            RegisterCommand("css_build", "Open build menu", OnBuildCommand);
            RegisterCommand("css_skills", "Open skills menu", OnSkillsCommand);
            RegisterCommand("css_talents", "Open talents menu", OnTalentsCommand);
            RegisterCommand("css_shop", "Open shop menu", OnShopCommand);
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
            RegisterCommand("css_hud", "Toggle HUD display", OnHudCommand);
            RegisterCommand("css_botstats", "Show bot statistics for balancing", OnBotStatsCommand);
            
            Console.WriteLine("[UltimateHeroes] Plugin loaded successfully!");
        }

        private void OnMapStart(string mapName)
        {
            Console.WriteLine($"[UltimateHeroes] Map started: {mapName}");
            
            // Award Match Completion XP für alle Spieler vom vorherigen Match
            // (wird aufgerufen wenn neue Map startet = vorheriges Match endete)
            var gameMode = Application.Services.GameModeDetector.DetectCurrentMode();
            var allPlayers = Utilities.GetPlayers();
            foreach (var player in allPlayers)
            {
                if (player == null || !player.IsValid || player.AuthorizedSteamID == null) continue;
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                // Note: Win/Loss wird hier nicht erkannt, daher false
                // TODO: Track match result before map change
                _xpService?.AwardMatchCompletion(steamId, gameMode, false);
            }
            
            // Reset Shop Items für alle Spieler (neues Match)
            _shopService?.ResetAllPlayersForNewMatch();
            
            // Start effect tick timer (every 0.5 seconds)
            AddTimer(0.5f, () =>
            {
                _effectManager?.TickEffects();
            }, TimerFlags.REPEAT);
            
            // Start HUD update timer (every 0.5 seconds)
            AddTimer(0.5f, () =>
            {
                _hudManager?.UpdateHud();
            }, TimerFlags.REPEAT);
            
            // Start Bot Build Change timer (every 30 seconds)
            if (Config.BotSettings.Enabled && Config.BotSettings.AutoAssignBuild)
            {
                AddTimer(30f, () =>
                {
                    var players = Utilities.GetPlayers();
                    foreach (var player in players)
                    {
                        if (player == null || !player.IsValid) continue;
                        if (_botService != null && _botService.IsBot(player))
                        {
                            string botSteamId = player.AuthorizedSteamID?.SteamId64.ToString() ?? "BOT_" + player.Slot;
                            _botService.CheckBuildChange(botSteamId);
                        }
                    }
                }, TimerFlags.REPEAT);
            }
            
            // Start In-Match Evolution timer (for time-based modes)
            var gameMode = Application.Services.GameModeDetector.DetectCurrentMode();
            if (Application.Services.GameModeDetector.IsTimeBased(gameMode))
            {
                float matchStartTime = Server.CurrentTime;
                AddTimer(1f, () =>
                {
                    var players = Utilities.GetPlayers();
                    foreach (var player in players)
                    {
                        if (player == null || !player.IsValid || player.AuthorizedSteamID == null) continue;
                        var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                        float minutesElapsed = (Server.CurrentTime - matchStartTime) / 60f;
                        _inMatchEvolutionService?.OnTimeUpdate(steamId, minutesElapsed);
                    }
                }, TimerFlags.REPEAT);
            }
            
            // Reset match progress for all players on map start
            var allPlayers = Utilities.GetPlayers();
            foreach (var player in allPlayers)
            {
                if (player == null || !player.IsValid || player.AuthorizedSteamID == null) continue;
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                _inMatchEvolutionService?.ResetMatchProgress(steamId);
            }
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
            
            // Disable HUD for player
            _hudManager?.DisableHud(player);
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
            
            // Enable HUD for player
            _hudManager?.EnableHud(player);
            
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
            
            // Update player stats and disable HUD when player dies
            if (victim != null && victim.IsValid && victim.AuthorizedSteamID != null)
            {
                var victimSteamId = victim.AuthorizedSteamID.SteamId64.ToString();
                var playerState = _playerService?.GetPlayer(victimSteamId);
                if (playerState != null)
                {
                    playerState.OnDeath();
                    _playerService?.SavePlayer(playerState);
                }
                
                // Disable HUD when player dies (will be re-enabled on spawn)
                _hudManager?.DisableHud(victim);
            }
            
            return HookResult.Continue;
        }
        
        private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            var roundNumber = @event.RoundNum;
            var players = Utilities.GetPlayers();
            
            foreach (var player in players)
            {
                if (player == null || !player.IsValid || player.AuthorizedSteamID == null) continue;
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                _inMatchEvolutionService?.OnRoundStart(steamId, roundNumber);
            }
            
            return HookResult.Continue;
        }
        
        private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
        {
            var winner = @event.Winner;
            var players = Utilities.GetPlayers();
            var gameMode = Application.Services.GameModeDetector.DetectCurrentMode();
            
            foreach (var player in players)
            {
                if (player == null || !player.IsValid || player.AuthorizedSteamID == null) continue;
                var steamId = player.AuthorizedSteamID.SteamId64.ToString();
                bool won = player.TeamNum == winner;
                
                // In-Match Evolution
                _inMatchEvolutionService?.OnRoundEnd(steamId, won);
                
                // Award Round Win XP (wenn gewonnen)
                if (won)
                {
                    _xpService?.AwardXp(steamId, Domain.Progression.XpSource.RoundWin);
                }
            }
            
            // Check if match ended (last round)
            // Note: In CS2, we need to check if this is the final round
            // For now, we'll award Match Completion on map change or explicit match end event
            
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
        
        private void OnHudCommand(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid) return;
            
            if (_hudManager == null)
            {
                player.PrintToChat($" {ChatColors.Red}[Ultimate Heroes]{ChatColors.Default} HUD system not initialized!");
                return;
            }
            
            var isEnabled = _hudManager.IsHudEnabled(player);
            
            if (isEnabled)
            {
                _hudManager.DisableHud(player);
                player.PrintToChat($" {ChatColors.Yellow}[Ultimate Heroes]{ChatColors.Default} HUD disabled. Use {ChatColors.LightBlue}!hud{ChatColors.Default} to enable.");
            }
            else
            {
                _hudManager.EnableHud(player);
                player.PrintToChat($" {ChatColors.Green}[Ultimate Heroes]{ChatColors.Default} HUD enabled. Use {ChatColors.LightBlue}!hud{ChatColors.Default} to disable.");
            }
        }
        
        private void OnBotStatsCommand(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid || _botService == null) return;
            
            var allBotStats = _botService.GetAllBotStats();
            
            if (allBotStats.Count == 0)
            {
                player.PrintToChat($" {ChatColors.Yellow}[Bot Stats]{ChatColors.Default} No bot statistics available yet.");
                return;
            }
            
            player.PrintToChat($" {ChatColors.Green}╔════════════════════════════════╗");
            player.PrintToChat($" {ChatColors.Green}║{ChatColors.Default}   Bot Statistics (Balancing)  {ChatColors.Green}║");
            player.PrintToChat($" {ChatColors.Green}╚════════════════════════════════╝");
            
            // Aggregate stats
            int totalKills = 0;
            int totalDeaths = 0;
            float totalDamage = 0f;
            int totalRoundsWon = 0;
            int totalRoundsLost = 0;
            
            foreach (var stats in allBotStats)
            {
                totalKills += stats.TotalKills;
                totalDeaths += stats.TotalDeaths;
                totalDamage += stats.TotalDamage;
                totalRoundsWon += stats.RoundsWon;
                totalRoundsLost += stats.RoundsLost;
            }
            
            float avgKDRatio = totalDeaths > 0 ? (float)totalKills / totalDeaths : totalKills;
            float avgWinRate = (totalRoundsWon + totalRoundsLost) > 0 ? (float)totalRoundsWon / (totalRoundsWon + totalRoundsLost) : 0f;
            
            player.PrintToChat($" {ChatColors.Yellow}Total Bots: {ChatColors.LightBlue}{allBotStats.Count}");
            player.PrintToChat($" {ChatColors.Yellow}K/D Ratio: {ChatColors.LightBlue}{avgKDRatio:F2}");
            player.PrintToChat($" {ChatColors.Yellow}Win Rate: {ChatColors.LightBlue}{avgWinRate * 100:F1}%");
            player.PrintToChat($" {ChatColors.Yellow}Total Kills: {ChatColors.LightBlue}{totalKills}");
            player.PrintToChat($" {ChatColors.Yellow}Total Deaths: {ChatColors.LightBlue}{totalDeaths}");
            player.PrintToChat($" {ChatColors.Yellow}Total Damage: {ChatColors.LightBlue}{totalDamage:F0}");
            
            // Show best/worst performing builds
            var buildStats = new Dictionary<string, (int kills, int deaths, int wins, int losses)>();
            foreach (var botStat in allBotStats)
            {
                foreach (var buildPerf in botStat.BuildPerformances)
                {
                    if (!buildStats.ContainsKey(buildPerf.Key))
                    {
                        buildStats[buildPerf.Key] = (0, 0, 0, 0);
                    }
                    var current = buildStats[buildPerf.Key];
                    buildStats[buildPerf.Key] = (
                        current.kills + buildPerf.Value.Kills,
                        current.deaths + buildPerf.Value.Deaths,
                        current.wins + buildPerf.Value.Wins,
                        current.losses + buildPerf.Value.Losses
                    );
                }
            }
            
            if (buildStats.Count > 0)
            {
                player.PrintToChat($"");
                player.PrintToChat($" {ChatColors.Yellow}Build Performance:");
                foreach (var kvp in buildStats.OrderByDescending(x => x.Value.kills))
                {
                    var kd = kvp.Value.deaths > 0 ? (float)kvp.Value.kills / kvp.Value.deaths : kvp.Value.kills;
                    player.PrintToChat($" {ChatColors.Gray}Build {kvp.Key}: K/D {kd:F2} ({kvp.Value.kills}K/{kvp.Value.deaths}D)");
                }
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
