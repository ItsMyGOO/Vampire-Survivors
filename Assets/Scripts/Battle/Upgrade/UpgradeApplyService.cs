using System;
using Battle.Player;
using ECS.Core;
using UnityEngine;

namespace Battle.Upgrade
{
    public class UpgradeApplyService
    {
        private WeaponUpgradeManager _weaponUpgradeManager;

        /// <summary>
        /// 初始化服务
        /// </summary>
        public UpgradeApplyService(WeaponUpgradeManager weaponUpgradeManager)
        {
            _weaponUpgradeManager = weaponUpgradeManager;
        }

        /// <summary>
        /// 应用升级选项（可由外部UI或测试代码调用）
        /// </summary>
        public void ApplyUpgradeOption(UpgradeOption option)
        {
            if (option == null)
            {
                Debug.LogError("[ExpSystem] 升级选项为空");
                return;
            }

            Debug.Log($"[ExpSystem] 应用升级选项: {option.type} - {option.name} (ID: {option.id})");

            try
            {
                Apply(option);
                Debug.Log($"[ExpSystem] ========== 升级应用完成 ==========\n");
            }
            catch (Exception e)
            {
                Debug.LogError($"[ExpSystem] 应用升级选项时出错: {e.Message}\n{e.StackTrace}");
            }
        }

        private void Apply(UpgradeOption option)
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

        private void ApplyWeapon(World world, int player, string weaponId)
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
                bool success = _weaponUpgradeManager.AddWeapon(weaponId);
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
                // 升级现有武器
                bool success = _weaponUpgradeManager.UpgradeWeapon(weaponId);
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

        private void ApplyPassive(World world, int player, string passiveId)
        {
            Debug.Log($"[UpgradeApplyService] 添加被动技能 - {passiveId}");

            // TODO: 实现被动技能应用逻辑
        }
    }
}