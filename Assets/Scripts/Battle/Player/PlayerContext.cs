using Battle.Upgrade;
using ECS.Core;

namespace Battle
{
    /// <summary>
    /// 玩家上下文 - 单例
    /// 存储玩家相关的全局数据
    /// </summary>
    public class PlayerContext
    {
        public static PlayerContext Instance { get; private set; }

        public World World { get; private set; }
        public int PlayerEntity { get; private set; }
        public WeaponUpgradeManager WeaponUpgradeManager { get; set; }
        public UpgradeService UpgradeService { get; private set; }
        public ExpData ExpData { get; set; }
        public PlayerUpgradeState UpgradeState { get; set; }

        public static void Initialize(
            World world,
            int playerEntity,
            WeaponUpgradeManager weaponUpgradeManager,
            UpgradeService upgradeService)
        {
            Instance = new PlayerContext
            {
                World = world,
                PlayerEntity = playerEntity,
                WeaponUpgradeManager = weaponUpgradeManager,
                UpgradeService = upgradeService
            };
        }

        /// <summary>
        /// 清理所有数据
        /// 在退出战斗时调用
        /// </summary>
        public void Clear()
        {
            UnityEngine.Debug.Log("[PlayerContext] 清理数据");

            World = null;
            PlayerEntity = -1;
            WeaponUpgradeManager = null;
            UpgradeService = null;

            // 清空单例
            Instance = null;
        }

        public void BindExpData(ExpData expData)
        {
            ExpData= expData;
        }
    }
}
