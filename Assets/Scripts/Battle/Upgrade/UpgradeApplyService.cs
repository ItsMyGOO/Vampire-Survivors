using Battle.Player;
using ECS;
using UnityEngine;

namespace Battle.Upgrade
{
    /// <summary>
    /// 升级应用服务 - 重构版
    /// 职责简化为：将外部升级请求转换为 ECS 意图组件
    /// 
    /// 原有的复杂逻辑移至：
    /// - PassiveUpgradeSystem: 处理被动升级
    /// - AttributeCalculationSystem: 计算属性
    /// - AttributeSyncSystem: 同步属性到其他组件
    /// </summary>
    public class UpgradeApplyService
    {
        private WeaponUpgradeManager weaponUpgradeManager;

        public UpgradeApplyService(WeaponUpgradeManager weaponUpgradeManager)
        {
            this.weaponUpgradeManager = weaponUpgradeManager;
        }

        /// <summary>
        /// 应用升级选项
        /// </summary>
        public void ApplyUpgradeOption(UpgradeOption option)
        {
            if (option == null)
            {
                Debug.LogError("[UpgradeApplyService] 升级选项为空");
                return;
            }

            Debug.Log($"[UpgradeApplyService] 应用升级选项: {option.type} - {option.name} (ID: {option.id})");

            switch (option.type)
            {
                case UpgradeOptionType.Weapon:
                    ApplyWeapon(option.id);
                    break;

                case UpgradeOptionType.Passive:
                    ApplyPassive(option.id);
                    break;
            }
        }

        /// <summary>
        /// 应用武器升级（保持原有逻辑）
        /// </summary>
        private void ApplyWeapon(string weaponId)
        {
            if (weaponUpgradeManager == null)
            {
                Debug.LogError("[UpgradeApplyService] WeaponUpgradeManager 未初始化");
                return;
            }

            var world = PlayerContext.Instance.World;
            int player = PlayerContext.Instance.PlayerEntity;

            int currentLevel = weaponUpgradeManager.GetWeaponLevel(world, player, weaponId);

            if (currentLevel == 0)
            {
                bool success = weaponUpgradeManager.AddWeapon(weaponId);
                if (success)
                {
                    Debug.Log($"[UpgradeApplyService] 成功添加武器 - {weaponId}");
                }
                else
                {
                    Debug.LogError($"[UpgradeApplyService] 添加武器失败 - {weaponId}");
                }
            }
            else
            {
                bool success = weaponUpgradeManager.UpgradeWeapon(weaponId);
                if (success)
                {
                    Debug.Log($"[UpgradeApplyService] 成功升级武器 - {weaponId} 到 Lv.{currentLevel + 1}");
                }
                else
                {
                    Debug.LogError($"[UpgradeApplyService] 升级武器失败 - {weaponId}");
                }
            }
        }

        /// <summary>
        /// 应用被动升级 - 重构版
        /// 只负责创建意图组件，具体逻辑由 PassiveUpgradeSystem 处理
        /// </summary>
        private void ApplyPassive(string passiveId)
        {
            Debug.Log($"[UpgradeApplyService] 请求添加被动技能 - {passiveId}");

            var world = PlayerContext.Instance.World;
            int player = PlayerContext.Instance.PlayerEntity;

            // 创建升级意图组件
            var intent = new PassiveUpgradeIntentComponent(passiveId, levelDelta: 1);
            world.AddComponent(player, intent);

            Debug.Log($"[UpgradeApplyService] 已创建被动升级意图，等待系统处理");
        }
    }
}
