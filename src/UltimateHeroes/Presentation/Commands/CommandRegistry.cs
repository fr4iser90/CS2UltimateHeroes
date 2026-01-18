using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;

namespace UltimateHeroes.Presentation.Commands
{
    /// <summary>
    /// Registry für Command Handler (Strategy Pattern)
    /// </summary>
    public class CommandRegistry
    {
        private readonly Dictionary<string, ICommandHandler> _handlers = new();
        private readonly BasePlugin _plugin;
        
        public CommandRegistry(BasePlugin plugin)
        {
            _plugin = plugin;
        }
        
        public void RegisterHandler(ICommandHandler handler)
        {
            try
            {
                _handlers[handler.CommandName] = handler;
                _plugin.AddCommand($"css_{handler.CommandName}", handler.Description, (player, info) => 
                {
                    handler.Handle(player, info);
                });
                Console.WriteLine($"[UltimateHeroes] Registered command: css_{handler.CommandName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UltimateHeroes] ❌ Failed to register command {handler.CommandName}: {ex.Message}");
                throw;
            }
        }
        
        public ICommandHandler? GetHandler(string commandName)
        {
            return _handlers.GetValueOrDefault(commandName);
        }
        
        /// <summary>
        /// Automatically registers all ICommandHandler implementations via Reflection
        /// </summary>
        public void RegisterHandlersViaReflection()
        {
            var handlerType = typeof(ICommandHandler);
            var handlerTypes = System.Reflection.Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => handlerType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
            
            foreach (var type in handlerTypes)
            {
                try
                {
                    // Command Handlers brauchen Dependencies - werden via DI gesetzt
                    // Für jetzt: Manuell registrieren
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"[CommandRegistry] Failed to register handler {type.Name}: {ex.Message}");
                }
            }
        }
    }
}
