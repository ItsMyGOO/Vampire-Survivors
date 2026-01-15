using Battle.Player;
using ConfigHandler;
using ECS.Core;
using Lua;

namespace Battle.Upgrade
{
    public static class UpgradeBootstrap
    {
        public static void Initialize(World world, int playerId)
        {
            var weaponUpgradeManager = new WeaponUpgradeManager();
            var upgradeService = new UpgradeService();

            UpgradeApplyService.Initialize(weaponUpgradeManager);

            PlayerContext.Initialize(world, playerId, upgradeService);

            var expData = ExpSystem.Instance.CreateExpData();
            PlayerContext.Instance.BindExpData(expData);

            ExpSystem.Instance.Init(LuaMain.Env, world, playerId, upgradeService);
            LuaMain.Register(ExpSystem.Instance);

            world.RegisterService(ExpSystem.Instance);
        }
    }
}