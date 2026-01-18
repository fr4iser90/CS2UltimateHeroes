using System;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using ConVar = CounterStrikeSharp.API.Core.ConVar;

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
            
            // Test weapon spread ConVar
            var spreadConVar = ConVar.Find("weapon_accuracy_nospread");
            if (spreadConVar != null)
            {
                Console.WriteLine($"[API Test] ✅ weapon_accuracy_nospread ConVar EXISTS");
                Console.WriteLine($"  - Value: {spreadConVar.GetPrimitiveValue<int>()}");
            }
            else
            {
                Console.WriteLine("[API Test] ❌ weapon_accuracy_nospread ConVar NOT FOUND");
            }
            
            // Test jump ConVar
            var jumpConVar = ConVar.Find("sv_jump_impulse");
            if (jumpConVar != null)
            {
                Console.WriteLine($"[API Test] ✅ sv_jump_impulse ConVar EXISTS");
                Console.WriteLine($"  - Value: {jumpConVar.GetPrimitiveValue<float>()}");
            }
            else
            {
                Console.WriteLine("[API Test] ❌ sv_jump_impulse ConVar NOT FOUND");
            }
            
            // Test footstep ConVar
            var footstepConVar = ConVar.Find("sv_footsteps");
            if (footstepConVar != null)
            {
                Console.WriteLine($"[API Test] ✅ sv_footsteps ConVar EXISTS");
                Console.WriteLine($"  - Value: {footstepConVar.GetPrimitiveValue<int>()}");
            }
            else
            {
                Console.WriteLine("[API Test] ❌ sv_footsteps ConVar NOT FOUND");
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
