using UltimateHeroes.Application.Services;

namespace UltimateHeroes.Infrastructure.Helpers
{
    /// <summary>
    /// Static Helper für BuffService (für Damage-Tracking in Skills)
    /// </summary>
    public static class BuffServiceHelper
    {
        private static IBuffService? _buffService;
        
        public static void SetBuffService(IBuffService buffService)
        {
            _buffService = buffService;
        }
        
        public static IBuffService? GetBuffService()
        {
            return _buffService;
        }
    }
}
