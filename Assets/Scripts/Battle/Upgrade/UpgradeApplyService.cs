using ECS;
using ECS.Core;
using UnityEngine;

namespace Battle.Upgrade
{
    /// <summary>
    /// 升级应用服务
    /// 职责：将外部升级请求转换为 ECS 意图组件
    /// </summary>
    public class UpgradeApplyService
    {
        private readonly WeaponUpgradeManager _weaponUpgradeManager;
        private readonly World _world;
        private readonly int _playerId;

        public UpgradeApplyService(WeaponUpgradeManager weaponUpgradeManager, World world, int playerId)
        {
            _weaponUpgradeManager = weaponUpgradeManager;
            _world = world;
            _playerId = playerId;
        }

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

        private void ApplyWeapon(string weaponId)
        {
            if (_weaponUpgradeManager == null)
            {
                Debug.LogError("[UpgradeApplyService] WeaponUpgradeManager 未初始化");
                return;
            }

            int currentLevel = _weaponUpgradeManager.GetWeaponLevel(_world, _playerId, weaponId);

            if (currentLevel == 0)
            {
                bool success = _weaponUpgradeManager.AddWeapon(weaponId);
                if (success)
                    Debug.Log($"[UpgradeApplyService] 成功添加武器 - {weaponId}");
                else
                    Debug.LogError($"[UpgradeApplyService] 添加武器失败 - {weaponId}");
            }
            else
            {
                bool success = _weaponUpgradeManager.UpgradeWeapon(weaponId);
                if (success)
                    Debug.Log($"[UpgradeApplyService] 成功升级武器 - {weaponId} 到 Lv.{currentLevel + 1}");
                else
                    Debug.LogError($"[UpgradeApplyService] 升级武器失败 - {weaponId}");
            }
        }

        private void ApplyPassive(string passiveId)
        {
            Debug.Log($"[UpgradeApplyService] 请求添加被动技能 - {passiveId}");

            var intent = new PassiveUpgradeIntentComponent(passiveId, levelDelta: 1);
            _world.AddComponent(_playerId, intent);

            Debug.Log("[UpgradeApplyService] 已创建被动升级意图，等待系统处理");
        }
    }
}