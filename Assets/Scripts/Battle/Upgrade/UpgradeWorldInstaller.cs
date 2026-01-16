using Battle.Player;
using ECS.Core;
using Lua;

namespace Battle.Upgrade
{
    public static class UpgradeWorldInstaller
    {
        public static void Install(World world, int playerEntity)
        {
            // 1. System
            var expSystem = new ExpSystem();

            // 2. Services
            var upgradeService = new UpgradeService(LuaMain.Env, world, playerEntity);
            var weaponUpgradeManager = new WeaponUpgradeManager(world, playerEntity);
            var upgradeApplyService = new UpgradeApplyService(weaponUpgradeManager);

            // 3. 事件绑定
            expSystem.OnLevelUp += upgradeService.RollUpgradeOptions;
            upgradeService.OnApplyUpgradeOptions += upgradeApplyService.ApplyUpgradeOption;

            // 4. World 注册（关键）
            world.RegisterService(expSystem);

            // 5. Lua
            LuaMain.Register(upgradeService);

            // 6. PlayerContext 只存引用
            PlayerContext.Instance.ExpSystem = expSystem;
        }
    }
}