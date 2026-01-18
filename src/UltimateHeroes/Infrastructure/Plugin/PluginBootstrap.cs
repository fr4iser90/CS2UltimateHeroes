using System;
using System.IO;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Domain.Heroes;
using UltimateHeroes.Domain.Skills;
using UltimateHeroes.Infrastructure.Cooldown;
using UltimateHeroes.Infrastructure.Database.Repositories;
using DatabaseClass = UltimateHeroes.Infrastructure.Database.Database;
using UltimateHeroes.Infrastructure.Effects;
using UltimateHeroes.Infrastructure.Events;
using UltimateHeroes.Infrastructure.Helpers;
using UltimateHeroes.Presentation.Menu;
using UltimateHeroes.Presentation.UI;

namespace UltimateHeroes.Infrastructure.Plugin
{
    /// <summary>
    /// Plugin Bootstrap - Initialisiert alle Services und Dependencies
    /// </summary>
    public class PluginBootstrap
    {
        private readonly string _moduleDirectory;
        private readonly BasePlugin _plugin;
        
        // Services
        public DatabaseClass? Database { get; private set; }
        public IPlayerRepository? PlayerRepository { get; private set; }
        public IBuildRepository? BuildRepository { get; private set; }
        public ICooldownManager? CooldownManager { get; private set; }
        public EffectManager? EffectManager { get; private set; }
        public EventSystem? EventSystem { get; private set; }
        public IPlayerService? PlayerService { get; private set; }
        public IHeroService? HeroService { get; private set; }
        public ISkillService? SkillService { get; private set; }
        public IBuildService? BuildService { get; private set; }
        public IXpService? XpService { get; private set; }
        public ITalentService? TalentService { get; private set; }
        public IMasteryService? MasteryService { get; private set; }
        public IInMatchEvolutionService? InMatchEvolutionService { get; private set; }
        public IRoleInfluenceService? RoleInfluenceService { get; private set; }
        public IBuildIntegrityService? BuildIntegrityService { get; private set; }
        public IBotService? BotService { get; private set; }
        public IShopService? ShopService { get; private set; }
        public IAccountService? AccountService { get; private set; }
        public IBuffService? BuffService { get; private set; }
        public ISpawnService? SpawnService { get; private set; }
        public Infrastructure.Weapons.IWeaponModifier? WeaponModifier { get; private set; }
        
        // Event Handlers
        public Infrastructure.Events.EventHandlers.PlayerKillHandler? PlayerKillHandler { get; private set; }
        public Infrastructure.Events.EventHandlers.PlayerHurtHandler? PlayerHurtHandler { get; private set; }
        
        // Menus and HUD (initialized in UltimateHeroes.cs)
        public HudManager? HudManager { get; set; }
        public HeroMenu? HeroMenu { get; set; }
        public BuildMenu? BuildMenu { get; set; }
        public SkillMenu? SkillMenu { get; set; }
        public TalentMenu? TalentMenu { get; set; }
        public ShopMenu? ShopMenu { get; set; }
        
        public PluginBootstrap(BasePlugin plugin, string moduleDirectory)
        {
            _plugin = plugin;
            _moduleDirectory = moduleDirectory;
        }
        
        public void Initialize()
        {
            // Initialize Database
            var dbPath = Path.Combine(_moduleDirectory, "ultimateheroes.db");
            Database = new DatabaseClass(dbPath);
            
            // Initialize Repositories
            PlayerRepository = new PlayerRepository(Database);
            BuildRepository = new BuildRepository(Database);
            var talentRepository = new TalentRepository(Database);
            var masteryRepository = new MasteryRepository(Database);
            var accountRepository = new AccountRepository(Database);
            
            // Initialize Infrastructure
            CooldownManager = new CooldownManager();
            EffectManager = new EffectManager();
            EventSystem = new EventSystem();
            
            // Initialize Services
            PlayerService = new PlayerService(PlayerRepository);
            HeroService = new HeroService();
            MasteryService = new MasteryService(masteryRepository);
            SkillService = new SkillService(CooldownManager, PlayerService, EffectManager, MasteryService);
            
            // Set SkillService in Helper for Damage Tracking
            SkillServiceHelper.SetSkillService(SkillService);
            
            // Initialize TalentService first (needed for BuildService)
            TalentService = new TalentService(talentRepository);
            
            // Initialize ShopService
            ShopService = new ShopService();
            
            // Initialize AccountService
            AccountService = new AccountService(accountRepository);
            
            // Initialize BuffService
            BuffService = new BuffService();
            BuffServiceHelper.SetBuffService(BuffService);
            
            // Initialize GameHelpersService and set in Helper
            var gameHelpersService = new Application.Helpers.GameHelpersService();
            GameHelper.SetGameHelpers(gameHelpersService);
            
            // Initialize SpawnService
            SpawnService = new SpawnService();
            
            // Initialize WeaponModifier
            WeaponModifier = new Infrastructure.Weapons.WeaponModifier(BuffService);
            
            var buildValidator = new Domain.Builds.Validation.BuildValidator();
            BuildService = new BuildService(BuildRepository, HeroService, SkillService, buildValidator, PlayerService, TalentService);
            XpService = new XpService(PlayerRepository, PlayerService, TalentService, AccountService);
            InMatchEvolutionService = new InMatchEvolutionService(PlayerService);
            RoleInfluenceService = new RoleInfluenceService(PlayerService);
            BuildIntegrityService = new BuildIntegrityService(SkillService);
            BotService = new BotService(PlayerService, HeroService, SkillService, BuildService, XpService);
            
            // Set TalentService in PlayerService for Talent Modifiers
            ((PlayerService)PlayerService).SetTalentService(TalentService);
            
            // Auto-register Heroes/Skills via Reflection
            ((HeroService)HeroService).RegisterHeroesViaReflection();
            ((SkillService)SkillService).RegisterSkillsViaReflection();
            
            // Auto-set Dependencies via Reflection
            SetEffectManagerForSkillsViaReflection();
            SetSpawnServiceForSkillsViaReflection();
            
            // Initialize Assist Tracking
            var assistTracking = new Infrastructure.Events.AssistTracking();
            
            // Register Event Handlers
            PlayerKillHandler = new Infrastructure.Events.EventHandlers.PlayerKillHandler(
                XpService, PlayerService, assistTracking, MasteryService, InMatchEvolutionService, BotService, BuffService, SkillService);
            PlayerHurtHandler = new Infrastructure.Events.EventHandlers.PlayerHurtHandler(PlayerService, assistTracking);
            EventSystem.RegisterHandler<Infrastructure.Events.EventHandlers.PlayerKillEvent>(PlayerKillHandler);
            EventSystem.RegisterHandler<Infrastructure.Events.EventHandlers.PlayerHurtEvent>(PlayerHurtHandler);
            
            // Note: Menu/UI Initialization is done in UltimateHeroes.cs (Presentation Layer)
        }
        
        private void SetEffectManagerForSkillsViaReflection()
        {
            if (EffectManager == null || SkillService == null) return;
            
            var skillType = typeof(ISkill);
            var skillTypes = System.Reflection.Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => skillType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && 
                           t != typeof(SkillBase) && 
                           t != typeof(ActiveSkillBase) && 
                           t != typeof(PassiveSkillBase));
            
            foreach (var type in skillTypes)
            {
                try
                {
                    var effectManagerProperty = type.GetProperty("EffectManager", 
                        System.Reflection.BindingFlags.Public | 
                        System.Reflection.BindingFlags.Static | 
                        System.Reflection.BindingFlags.SetProperty);
                    
                    if (effectManagerProperty != null && effectManagerProperty.PropertyType == typeof(EffectManager))
                    {
                        effectManagerProperty.SetValue(null, EffectManager);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[PluginBootstrap] Failed to set EffectManager for {type.Name}: {ex.Message}");
                }
            }
        }
        
        private void SetSpawnServiceForSkillsViaReflection()
        {
            if (SpawnService == null || SkillService == null) return;
            
            var skillType = typeof(ISkill);
            var skillTypes = System.Reflection.Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => skillType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && 
                           t != typeof(SkillBase) && 
                           t != typeof(ActiveSkillBase) && 
                           t != typeof(PassiveSkillBase));
            
            foreach (var type in skillTypes)
            {
                try
                {
                    var spawnServiceProperty = type.GetProperty("SpawnService", 
                        System.Reflection.BindingFlags.Public | 
                        System.Reflection.BindingFlags.Static | 
                        System.Reflection.BindingFlags.SetProperty);
                    
                    if (spawnServiceProperty != null && spawnServiceProperty.PropertyType == typeof(ISpawnService))
                    {
                        spawnServiceProperty.SetValue(null, SpawnService);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[PluginBootstrap] Failed to set SpawnService for {type.Name}: {ex.Message}");
                }
            }
        }
    }
}
