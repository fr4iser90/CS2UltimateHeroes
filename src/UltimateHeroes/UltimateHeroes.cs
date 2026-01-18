/*
 * Ultimate Heroes Mod for CS2
 * 
 * Copyright (C) 2026 fr4iser
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

using System;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using UltimateHeroes.Application.EventHandlers;
using UltimateHeroes.Infrastructure.Configuration;
using UltimateHeroes.Infrastructure.Plugin;
using UltimateHeroes.Infrastructure.Testing;
using UltimateHeroes.Presentation.Commands;

namespace UltimateHeroes
{
    /// <summary>
    /// Main Plugin Entry Point - Refactored with DDD Layer Separation
    /// </summary>
    public class UltimateHeroes : BasePlugin, IPluginConfig<PluginConfiguration>
    {
        public override string ModuleName => "Ultimate Heroes";
        public override string ModuleVersion => "0.1.0";

        public PluginConfiguration Config { get; set; } = null!;
        
        public void OnConfigParsed(PluginConfiguration config)
        {
            Config = config;
            
            // Set config in XpService if bootstrap is already initialized
            if (_bootstrap?.XpService != null)
            {
                ((Application.Services.XpService)_bootstrap.XpService).SetConfig(config);
            }
        }
        
        // ============================================
        // 🔧 PLUGIN BOOTSTRAP (Service Initialization)
        // ============================================
        private PluginBootstrap? _bootstrap;
        
        // ============================================
        // 🎮 EVENT HANDLERS (ausgelagert)
        // ============================================
        private PlayerEventHandler? _playerEventHandler;
        private RoundEventHandler? _roundEventHandler;
        private MapEventHandler? _mapEventHandler;
        private PlayerHurtEventHandler? _playerHurtEventHandler;
        
        // ============================================
        // 📝 COMMAND REGISTRY (ausgelagert)
        // ============================================
        private CommandRegistry? _commandRegistry;
        
        // ============================================
        // 🧪 API TEST PLUGIN (für API-Verfügbarkeitstests)
        // ============================================
        private ApiTestPlugin? _apiTestPlugin;

        public override void Load(bool hotReload)
        {
            try
            {
                Console.WriteLine("[UltimateHeroes] Starting plugin load...");
                
                // ============================================
                // 🔧 PLUGIN BOOTSTRAP (Service Initialization)
                // ============================================
                Console.WriteLine("[UltimateHeroes] Initializing bootstrap...");
                _bootstrap = new PluginBootstrap(this, ModuleDirectory);
                _bootstrap.Initialize();
                Console.WriteLine("[UltimateHeroes] Bootstrap initialized successfully");
            
            // Initialize Menus and HUD
            _bootstrap.HudManager = new Presentation.UI.HudManager(
                _bootstrap.SkillService!,
                _bootstrap.XpService!,
                _bootstrap.PlayerService!,
                _bootstrap.CooldownManager!,
                _bootstrap.MasteryService,
                _bootstrap.AccountService
            );
            _bootstrap.HeroMenu = new Presentation.Menu.HeroMenu(_bootstrap.HeroService!, _bootstrap.PlayerService!);
            _bootstrap.BuildMenu = new Presentation.Menu.BuildMenu(_bootstrap.BuildService!, _bootstrap.PlayerService!);
            _bootstrap.SkillMenu = new Presentation.Menu.SkillMenu(_bootstrap.SkillService!, _bootstrap.PlayerService!, _bootstrap.CooldownManager!);
            _bootstrap.TalentMenu = new Presentation.Menu.TalentMenu(_bootstrap.TalentService!, _bootstrap.PlayerService!);
            _bootstrap.ShopMenu = new Presentation.Menu.ShopMenu(_bootstrap.ShopService!, _bootstrap.PlayerService!);
            
            // ============================================
            // 🧪 API TEST PLUGIN (aktiviert für API-Tests)
            // ============================================
            _apiTestPlugin = new ApiTestPlugin();
            _apiTestPlugin.Load(hotReload);
            Console.WriteLine("[UltimateHeroes] API Test Plugin aktiviert - Teste API-Verfügbarkeit...");
            
            // ============================================
            // 🎮 EVENT HANDLERS (ausgelagert)
            // ============================================
            _playerEventHandler = new PlayerEventHandler(
                _bootstrap.PlayerService!,
                _bootstrap.HeroService!,
                _bootstrap.HudManager!,
                _bootstrap.EventSystem!,
                Config.DefaultHero,
                Config,
                _bootstrap.AccountService
            );
            
            _roundEventHandler = new RoundEventHandler(
                _bootstrap.InMatchEvolutionService!,
                _bootstrap.XpService!,
                _bootstrap.PlayerService!,
                _bootstrap.EventSystem!
            );
            
            _mapEventHandler = new MapEventHandler(
                _bootstrap.XpService!,
                _bootstrap.ShopService!,
                _bootstrap.SpawnService!,
                _bootstrap.EffectManager!,
                _bootstrap.BuffService!,
                _bootstrap.HudManager!,
                _bootstrap.BotService!,
                _bootstrap.InMatchEvolutionService!,
                this,
                Config
            );
            
            _playerHurtEventHandler = new PlayerHurtEventHandler(_bootstrap.EventSystem!);
            
            // ============================================
            // 📝 COMMAND HANDLERS (ausgelagert)
            // ============================================
            _commandRegistry = new CommandRegistry(this);
            
            // Register Command Handlers
            _commandRegistry.RegisterHandler(new HeroCommands(_bootstrap.HeroService!, _bootstrap.PlayerService!, _bootstrap.HeroMenu!));
            _commandRegistry.RegisterHandler(new SelectHeroCommand(_bootstrap.HeroService!, _bootstrap.PlayerService!));
            _commandRegistry.RegisterHandler(new BuildCommands(_bootstrap.BuildService!, _bootstrap.PlayerService!, _bootstrap.BuildMenu!));
            _commandRegistry.RegisterHandler(new CreateBuildCommand(_bootstrap.BuildService!, _bootstrap.PlayerService!));
            _commandRegistry.RegisterHandler(new ActivateBuildCommand(_bootstrap.BuildService!, _bootstrap.PlayerService!));
            _commandRegistry.RegisterHandler(new SkillCommands(_bootstrap.SkillService!, _bootstrap.PlayerService!, _bootstrap.CooldownManager!, _bootstrap.SkillMenu!));
            _commandRegistry.RegisterHandler(new UseSkillCommand(_bootstrap.SkillService!, _bootstrap.PlayerService!));
            _commandRegistry.RegisterHandler(new SkillSlotCommand(_bootstrap.SkillService!, _bootstrap.PlayerService!, 0)); // skill1
            _commandRegistry.RegisterHandler(new SkillSlotCommand(_bootstrap.SkillService!, _bootstrap.PlayerService!, 1)); // skill2
            _commandRegistry.RegisterHandler(new SkillSlotCommand(_bootstrap.SkillService!, _bootstrap.PlayerService!, 2)); // skill3
            _commandRegistry.RegisterHandler(new UltimateCommand(_bootstrap.SkillService!, _bootstrap.PlayerService!));
            _commandRegistry.RegisterHandler(new TalentCommands(_bootstrap.TalentService!, _bootstrap.PlayerService!, _bootstrap.TalentMenu!));
            _commandRegistry.RegisterHandler(new ShopCommands(_bootstrap.ShopService!, _bootstrap.PlayerService!, _bootstrap.ShopMenu!));
            _commandRegistry.RegisterHandler(new StatsCommand(_bootstrap.PlayerService!, _bootstrap.XpService!));
            _commandRegistry.RegisterHandler(new LeaderboardCommand(_bootstrap.PlayerService!, _bootstrap.AccountService, Config));
            _commandRegistry.RegisterHandler(new BotStatsCommand(_bootstrap.BotService!));
            _commandRegistry.RegisterHandler(new HudCommand(_bootstrap.HudManager!));
            
            // Admin Commands
            _commandRegistry.RegisterHandler(new AdminReloadCommand(this));
            _commandRegistry.RegisterHandler(new AdminGiveXpCommand(_bootstrap.XpService!, _bootstrap.PlayerService!));
            _commandRegistry.RegisterHandler(new AdminSetLevelCommand(_bootstrap.XpService!, _bootstrap.PlayerService!));
            _commandRegistry.RegisterHandler(new AdminResetPlayerCommand(_bootstrap.PlayerService!, _bootstrap.HeroService!));
            _commandRegistry.RegisterHandler(new AdminGiveHeroCommand(_bootstrap.HeroService!, _bootstrap.PlayerService!));
            _commandRegistry.RegisterHandler(new AdminListPlayersCommand(_bootstrap.PlayerService!));
            
            // ============================================
            // 📋 MENU API INITIALIZATION
            // ============================================
            Presentation.Menu.MenuAPI.Load(this);
            Console.WriteLine("[UltimateHeroes] MenuAPI initialized");
            
            // ============================================
            // 🎯 COUNTER-STRIKE SHARP EVENTS
            // ============================================
            RegisterListener<Listeners.OnMapStart>(OnMapStart);
            RegisterListener<Listeners.OnClientDisconnect>(OnClientDisconnect);
            RegisterEventHandler<EventPlayerConnect>(OnPlayerConnect);
            RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn);
            RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
            RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt);
            RegisterEventHandler<EventRoundStart>(OnRoundStart);
            RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
            
                Console.WriteLine("[UltimateHeroes] Plugin loaded successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UltimateHeroes] ❌ ERROR loading plugin: {ex.Message}");
                Console.WriteLine($"[UltimateHeroes] Stack trace: {ex.StackTrace}");
                throw; // Re-throw to let CounterStrikeSharp know the plugin failed to load
            }
        }
        
        // ============================================
        // 🎮 EVENT HANDLERS (delegiert an ausgelagerte Handler)
        // ============================================
        
        private void OnMapStart(string mapName)
        {
            _mapEventHandler?.OnMapStart(mapName);
        }
        
        private HookResult OnPlayerConnect(EventPlayerConnect @event, GameEventInfo info)
        {
            if (@event.Userid != null && @event.Userid.IsValid)
            {
                _playerEventHandler?.OnClientConnect(@event.Userid.Slot);
            }
            return HookResult.Continue;
        }
        
        private void OnClientDisconnect(int playerSlot)
        {
            _playerEventHandler?.OnClientDisconnect(playerSlot);
        }
        
        private HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
        {
            if (@event.Userid != null && @event.Userid.IsValid)
            {
                _playerEventHandler?.OnPlayerSpawn(@event.Userid);
            }
            return HookResult.Continue;
        }
        
        private HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
        {
            return _playerEventHandler?.OnPlayerDeath(@event, info) ?? HookResult.Continue;
        }
        
        private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            return _roundEventHandler?.OnRoundStart(@event, info) ?? HookResult.Continue;
        }
        
        private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
        {
            return _roundEventHandler?.OnRoundEnd(@event, info) ?? HookResult.Continue;
        }
        
        private HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
        {
            return _playerHurtEventHandler?.OnPlayerHurt(@event, info) ?? HookResult.Continue;
        }
    }
}
