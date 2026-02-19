using ECS.Core;
using Lua;

namespace Battle.Upgrade
{
    public static class UpgradeWorldInstaller
    {
        public static void Install(World world, int playerEntity)
        {
            // 1. Systems & Services
            var expSystem = new ExpSystem();
            var weaponUpgradeManager = new WeaponUpgradeManager(world, playerEntity);
            var upgradeApplyService = new UpgradeApplyService(weaponUpgradeManager, world, playerEntity);
            var upgradeService = new UpgradeService(LuaMain.Env, world, playerEntity);

            // 2. 事件绑定
            expSystem.OnLevelUp += upgradeService.RollUpgradeOptions;
            upgradeService.OnApplyUpgradeOptions += upgradeApplyService.ApplyUpgradeOption;

            // 3. World 服务注册（供系统通过 TryGetService 访问）
            world.RegisterService(expSystem);
            world.RegisterService(upgradeService);
            world.RegisterService(upgradeApplyService);

            // 4. Lua
            LuaMain.Register(upgradeService);
        }
    }
}