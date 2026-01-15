using ConfigHandler;
using ECS.Core;
using Lua;

namespace Battle.Upgrade
{
    public static class UpgradeBootstrap
    {
        public static void Initialize(World world, int playerId)
        {
            var weaponUpgradeManager = new WeaponUpgradeManager(
                WeaponUpgradeRuleConfigDB.Instance,
                WeaponConfigDB.Instance);


            var upgradeService = new UpgradeService(
                WeaponUpgradePoolConfigDB.Instance,
                WeaponUpgradeRuleConfigDB.Instance,
                PassiveUpgradePoolConfigDB.Instance);

            UpgradeApplyService.Initialize(weaponUpgradeManager);
            
            PlayerContext.Initialize(world, playerId, weaponUpgradeManager, upgradeService);
            
            var expData = ExpSystem.Instance.CreateExpData();
            PlayerContext.Instance.BindExpData(expData);

            ExpSystem.Instance.Init(LuaMain.Env, PlayerContext.Instance, upgradeService);
            LuaMain.Register(ExpSystem.Instance);

            world.RegisterService(ExpSystem.Instance);
        }
    }
}