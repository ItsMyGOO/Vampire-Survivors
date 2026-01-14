using ConfigHandler;
using ECS.Core;
using Game.Battle;
using Lua;

namespace Battle
{
    public static class UpgradeBootstrap
    {
        public static void Initialize(World world, int playerId)
        {
            var weaponUpgradeManager = new WeaponUpgradeManager(
                WeaponUpgradeRuleConfigDB.Instance,
                WeaponConfigDB.Instance);

            PlayerContext.Instance.Initialize(world, playerId);
            PlayerContext.Instance.WeaponUpgradeManager = weaponUpgradeManager;

            var upgradeService = new UpgradeService(
                WeaponUpgradePoolConfigDB.Instance,
                WeaponUpgradeRuleConfigDB.Instance,
                PassiveUpgradePoolConfigDB.Instance);

            UpgradeApplyService.Initialize(weaponUpgradeManager);

            ExpSystem.Instance.Init(LuaMain.Env, PlayerContext.Instance, upgradeService);
            world.RegisterService(ExpSystem.Instance);
        }
    }
}