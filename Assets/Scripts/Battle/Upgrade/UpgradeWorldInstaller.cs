using Battle.Player;
using ECS.Core;
using Lua;

namespace Battle.Upgrade
{
    public static class UpgradeWorldInstaller
    {
        public static void Install(World world)
        {
            // 1. System
            var expSystem = new ExpSystem();
            
            var upgradeState = new PlayerUpgradeState();

            // 2. Services
            var upgradeService = new UpgradeService(LuaMain.Env);
            var weaponUpgradeManager = new WeaponUpgradeManager();
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
            PlayerContext.Instance.UpgradeState = upgradeState;
        }
    }
}