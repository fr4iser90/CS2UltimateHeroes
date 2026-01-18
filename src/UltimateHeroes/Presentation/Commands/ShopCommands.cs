using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using UltimateHeroes.Application.Services;
using UltimateHeroes.Presentation.Menu;

namespace UltimateHeroes.Presentation.Commands
{
    /// <summary>
    /// Command Handler für Shop-bezogene Commands
    /// </summary>
    public class ShopCommands : ICommandHandler
    {
        private readonly IShopService _shopService;
        private readonly IPlayerService _playerService;
        private readonly ShopMenu _shopMenu;
        
        public string CommandName => "shop";
        public string Description => "Open shop menu";
        
        public ShopCommands(IShopService shopService, IPlayerService playerService, ShopMenu shopMenu)
        {
            _shopService = shopService;
            _playerService = playerService;
            _shopMenu = shopMenu;
        }
        
        public void Handle(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid) return;
            _shopMenu.ShowMenu(player);
        }
    }
}
