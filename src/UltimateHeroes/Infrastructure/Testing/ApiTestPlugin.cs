using System;
using System.Reflection;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;

namespace UltimateHeroes.Infrastructure.Testing
{
    /// <summary>
    /// Test Plugin um zu prüfen, welche APIs verfügbar sind
    /// Aktivierung: Setze ENABLE_API_TEST in UltimateHeroes.cs auf true
    /// </summary>
    public class ApiTestPlugin : BasePlugin
    {
        public override string ModuleName => "API Test Plugin";
        public override string ModuleVersion => "1.0.0";
        
        private int _tickCount = 0;

        public override void Load(bool hotReload)
        {
            Console.WriteLine("[API Test] Plugin loaded - Starting API tests...");
            
            // Test alle 5 Sekunden (nicht jeden Tick)
            AddTimer(5.0f, RunTests, TimerFlags.REPEAT);
            
            // Test auch beim ersten Load
            Server.NextFrame(() => RunTests());
        }

        private void RunTests()
        {
            _tickCount++;
            Console.WriteLine($"[API Test] === Test Run #{_tickCount} ===");
            
            var players = Utilities.GetPlayers();
            if (players.Count == 0)
            {
                Console.WriteLine("[API Test] No players online, skipping tests");
                return;
            }
            
            foreach (var player in players)
            {
                if (player == null || !player.IsValid) continue;
                
                TestWeaponProperties(player);
                TestPlayerPawnProperties(player);
                TestCollisionMethods(player);
                TestConVars();
            }
            
            Console.WriteLine("[API Test] === Test Complete ===\n");
        }

        private void TestWeaponProperties(CCSPlayerController player)
        {
            var pawn = player.PlayerPawn.Value;
            if (pawn?.WeaponServices?.ActiveWeapon.Value == null)
            {
                Console.WriteLine("[API Test] No weapon found, skipping weapon tests");
                return;
            }
            
            var weapon = pawn.WeaponServices.ActiveWeapon.Value;
            var steamId = player.AuthorizedSteamID?.SteamId64.ToString() ?? "UNKNOWN";
            
            Console.WriteLine($"[API Test] Testing weapon properties for player {steamId}");
            
            // Test 1: m_flSpread (via Reflection)
            TestPropertyViaReflection(weapon, "m_flSpread", typeof(float), "Weapon Spread");
            
            // Test 2: m_flNextPrimaryAttack (via Reflection)
            TestPropertyViaReflection(weapon, "m_flNextPrimaryAttack", typeof(float), "Fire Rate");
            
            // Test 3: Prüfe ob GetProperty Methode existiert
            TestGetPropertyMethod(weapon, "m_flSpread", typeof(float), "Weapon Spread");
            TestGetPropertyMethod(weapon, "m_flNextPrimaryAttack", typeof(float), "Fire Rate");
        }

        private void TestPlayerPawnProperties(CCSPlayerController player)
        {
            var pawn = player.PlayerPawn.Value;
            if (pawn == null) return;
            
            var steamId = player.AuthorizedSteamID?.SteamId64.ToString() ?? "UNKNOWN";
            Console.WriteLine($"[API Test] Testing player pawn properties for player {steamId}");
            
            // Test 1: m_flJumpPower
            TestPropertyViaReflection(pawn, "m_flJumpPower", typeof(float), "Jump Power");
            TestGetPropertyMethod(pawn, "m_flJumpPower", typeof(float), "Jump Power");
            
            // Test 2: m_flAirControl
            TestPropertyViaReflection(pawn, "m_flAirControl", typeof(float), "Air Control");
            TestGetPropertyMethod(pawn, "m_flAirControl", typeof(float), "Air Control");
            
            // Test 3: m_bPlayFootstepSounds
            TestPropertyViaReflection(pawn, "m_bPlayFootstepSounds", typeof(bool), "Footstep Sounds");
            TestGetPropertyMethod(pawn, "m_bPlayFootstepSounds", typeof(bool), "Footstep Sounds");
            
            // Test 4: m_CollisionGroup
            TestPropertyViaReflection(pawn, "m_CollisionGroup", typeof(int), "Collision Group");
            TestGetPropertyMethod(pawn, "m_CollisionGroup", typeof(int), "Collision Group");
        }

        private void TestCollisionMethods(CCSPlayerController player)
        {
            var pawn = player.PlayerPawn.Value;
            if (pawn == null) return;
            
            Console.WriteLine("[API Test] Testing collision methods");
            
            // Test: SetCollisionGroup Method
            var method = pawn.GetType().GetMethod("SetCollisionGroup", 
                System.Reflection.BindingFlags.Public | 
                System.Reflection.BindingFlags.Instance);
            
            if (method != null)
            {
                Console.WriteLine($"[API Test] ✅ SetCollisionGroup method EXISTS");
                Console.WriteLine($"  - Parameters: {method.GetParameters().Length}");
                foreach (var param in method.GetParameters())
                {
                    Console.WriteLine($"    - {param.Name}: {param.ParameterType.Name}");
                }
            }
            else
            {
                Console.WriteLine("[API Test] ❌ SetCollisionGroup method NOT FOUND");
            }
        }

        private void TestConVars()
        {
            Console.WriteLine("[API Test] Testing ConVars");
            
            // Try different possible ConVar API locations via Reflection
            TestConVarViaReflection("CounterStrikeSharp.API.Core.ConVar", "weapon_accuracy_nospread");
            TestConVarViaReflection("CounterStrikeSharp.API.ConVar", "weapon_accuracy_nospread");
            TestConVarViaReflection("CounterStrikeSharp.API.Modules.Cvars.ConVar", "weapon_accuracy_nospread");
            
            // Try via Server class methods
            TestConVarViaServer("weapon_accuracy_nospread");
            TestConVarViaServer("sv_jump_impulse");
            TestConVarViaServer("sv_footsteps");
        }
        
        private void TestConVarViaReflection(string namespacePath, string conVarName)
        {
            try
            {
                var type = Type.GetType($"{namespacePath}, CounterStrikeSharp.API");
                if (type != null)
                {
                    var findMethod = type.GetMethod("Find", new[] { typeof(string) });
                    if (findMethod != null && findMethod.IsStatic)
                    {
                        var conVar = findMethod.Invoke(null, new object[] { conVarName });
                        if (conVar != null)
                        {
                            Console.WriteLine($"[API Test] ✅ ConVar API found at: {namespacePath}");
                            Console.WriteLine($"[API Test] ✅ {conVarName} ConVar EXISTS");
                            
                            // Try to get value
                            var getValueMethod = conVar.GetType().GetMethod("GetPrimitiveValue");
                            if (getValueMethod != null)
                            {
                                var genericMethod = getValueMethod.MakeGenericMethod(typeof(int));
                                var value = genericMethod.Invoke(conVar, null);
                                Console.WriteLine($"  - Value: {value}");
                            }
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Silently try next namespace
            }
        }
        
        private void TestConVarViaServer(string conVarName)
        {
            try
            {
                // Try Server.ExecuteCommand or similar methods
                var serverType = typeof(Server);
                var methods = serverType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                
                foreach (var method in methods)
                {
                    if (method.Name.Contains("ConVar") || method.Name.Contains("Cvar") || method.Name.Contains("Get"))
                    {
                        Console.WriteLine($"[API Test] Found potential ConVar method: {method.Name}");
                        try
                        {
                            // Try to call it
                            var result = method.Invoke(null, new object[] { conVarName });
                            if (result != null)
                            {
                                Console.WriteLine($"[API Test] ✅ {conVarName} accessible via {method.Name}: {result}");
                            }
                        }
                        catch
                        {
                            // Try next method
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Test] Server-based ConVar test failed: {ex.Message}");
            }
        }

        private void TestPropertyViaReflection(object obj, string propertyName, Type expectedType, string displayName)
        {
            try
            {
                var property = obj.GetType().GetProperty(propertyName,
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);
                
                if (property != null)
                {
                    var value = property.GetValue(obj);
                    Console.WriteLine($"[API Test] ✅ {displayName} ({propertyName}) EXISTS via Reflection");
                    Console.WriteLine($"  - Type: {property.PropertyType.Name}");
                    Console.WriteLine($"  - Value: {value}");
                }
                else
                {
                    Console.WriteLine($"[API Test] ❌ {displayName} ({propertyName}) NOT FOUND via Reflection");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Test] ❌ {displayName} ({propertyName}) ERROR: {ex.Message}");
            }
        }

        private void TestGetPropertyMethod(object obj, string propertyName, Type expectedType, string displayName)
        {
            try
            {
                // Prüfe ob GetProperty<T> Methode existiert
                var getPropertyMethod = obj.GetType().GetMethod("GetProperty",
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance);
                
                if (getPropertyMethod != null)
                {
                    // Prüfe ob es generisch ist
                    if (getPropertyMethod.IsGenericMethod)
                    {
                        var genericMethod = getPropertyMethod.MakeGenericMethod(expectedType);
                        var value = genericMethod.Invoke(obj, new object[] { propertyName });
                        Console.WriteLine($"[API Test] ✅ {displayName} ({propertyName}) ACCESSIBLE via GetProperty<T>");
                        Console.WriteLine($"  - Value: {value}");
                    }
                    else
                    {
                        var value = getPropertyMethod.Invoke(obj, new object[] { propertyName });
                        Console.WriteLine($"[API Test] ✅ {displayName} ({propertyName}) ACCESSIBLE via GetProperty");
                        Console.WriteLine($"  - Value: {value}");
                    }
                }
                else
                {
                    Console.WriteLine($"[API Test] ❌ GetProperty method NOT FOUND for {displayName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API Test] ❌ GetProperty test ERROR for {displayName}: {ex.Message}");
            }
        }
    }
}
