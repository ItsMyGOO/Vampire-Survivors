using Battle.Player;
using ECS.Core;
using UnityEngine;

namespace Battle.Upgrade
{
    public static class UpgradeApplyService
    {
        private static WeaponUpgradeManager _weaponUpgradeManager;

        /// <summary>
        /// 初始化服务
        /// </summary>
        public static void Initialize(WeaponUpgradeManager weaponUpgradeManager)
        {
            _weaponUpgradeManager = weaponUpgradeManager;
        }

        public static void Apply(UpgradeOption option)
        {
            var world = PlayerContext.Instance.World;
            int player = PlayerContext.Instance.PlayerEntity;

            switch (option.type)
            {
                case UpgradeOptionType.Weapon:
                    ApplyWeapon(world, player, option.id);
                    break;

                case UpgradeOptionType.Passive:
                    ApplyPassive(world, player, option.id);
                    break;
            }
        }

        private static void ApplyWeapon(World world, int player, string weaponId)
        {
            if (_weaponUpgradeManager == null)
            {
                Debug.LogError("[UpgradeApplyService] WeaponUpgradeManager 未初始化");
                return;
            }

            // 检查玩家是否已拥有该武器
            int currentLevel = _weaponUpgradeManager.GetWeaponLevel(world, player, weaponId);

            if (currentLevel == 0)
            {
                // 添加新武器
                bool success = _weaponUpgradeManager.AddWeapon(world, player, weaponId);
                if (success)
                {
                    Debug.Log($"[UpgradeApplyService] 成功添加武器 - {weaponId}");
                    
                    // 更新玩家升级状态
                    var upgradeState = PlayerContext.Instance.UpgradeState;
                    upgradeState.AddOrUpgradeWeapon(weaponId);
                }
                else
                {
                    Debug.LogError($"[UpgradeApplyService] 添加武器失败 - {weaponId}");
                }
            }
            else
            {
                // 升级现有武器
                bool success = _weaponUpgradeManager.UpgradeWeapon(world, player, weaponId);
                if (success)
                {
                    Debug.Log($"[UpgradeApplyService] 成功升级武器 - {weaponId} 到 Lv.{currentLevel + 1}");
                    
                    // 更新玩家升级状态
                    var upgradeState = PlayerContext.Instance.UpgradeState;
                    upgradeState.AddOrUpgradeWeapon(weaponId);
                }
                else
                {
                    Debug.LogError($"[UpgradeApplyService] 升级武器失败 - {weaponId}");
                }
            }
        }

        private static void ApplyPassive(World world, int player, string passiveId)
        {
            Debug.Log($"[UpgradeApplyService] 添加被动技能 - {passiveId}");
            
            // 更新玩家升级状态
            var upgradeState = PlayerContext.Instance.UpgradeState;
            upgradeState.AddOrUpgradePassive(passiveId);
            
            // TODO: 实现被动技能应用逻辑
        }
    }
}
