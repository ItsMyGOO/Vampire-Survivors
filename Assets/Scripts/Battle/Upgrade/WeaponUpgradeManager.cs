using Battle.Weapon;
using ConfigHandler;
using ECS;
using ECS.Core;
using UnityEngine;

namespace Battle.Upgrade
{
    /// <summary>
    /// 武器升级管理器 - 负责处理武器的升级逻辑
    /// </summary>
    public class WeaponUpgradeManager
    {
        private WeaponUpgradeRuleConfigDB _upgradeRuleDB;
        private WeaponConfigDB _weaponConfigDB;

        public WeaponUpgradeManager()
        {
            _upgradeRuleDB = WeaponUpgradeRuleConfigDB.Instance;
            _weaponConfigDB = WeaponConfigDB.Instance;
        }

        /// <summary>
        /// 升级指定武器
        /// </summary>
        public bool UpgradeWeapon(World world, int playerEntity, string weaponId)
        {
            // 获取武器槽组件
            if (!world.TryGetComponent<WeaponRuntimeStatsComponent>(playerEntity, out var weaponStats))
            {
                Debug.LogError($"[WeaponUpgradeManager] Player entity {playerEntity} 没有 WeaponRuntimeStatsComponent");
                return false;
            }

            // 查找武器
            if (!weaponStats.TryGetRuntime(weaponId, out var weapon))
            {
                Debug.LogWarning($"[WeaponUpgradeManager] 玩家没有武器: {weaponId}");
                return false;
            }

            // 检查升级规则
            if (!_upgradeRuleDB.Data.TryGetValue(weaponId, out var upgradeRule))
            {
                Debug.LogError($"[WeaponUpgradeManager] 找不到武器升级规则: {weaponId}");
                return false;
            }

            // 检查是否已达到最大等级
            if (weapon.level >= upgradeRule.maxLevel)
            {
                Debug.LogWarning($"[WeaponUpgradeManager] 武器 {weaponId} 已达到最大等级 {upgradeRule.maxLevel}");
                return false;
            }

            // 升级
            int oldLevel = weapon.level;
            weapon.level++;
            ApplyUpgradeRules(weapon, upgradeRule, oldLevel, weapon.level);

            // 如果是轨道武器,需要重新生成
            if (_weaponConfigDB.TryGet(weaponId, out var weaponConfig))
            {
                if (weaponConfig.battle.Type == WeaponType.Orbit && weapon.orbitSpawned)
                {
                    // 清除旧的轨道武器
                    DestroyOrbitWeapons(world, playerEntity, weaponId);

                    // 重置生成标志,让系统重新生成
                    weapon.orbitSpawned = false;

                    Debug.Log($"[WeaponUpgradeManager] 轨道武器 {weaponId} 升级,将重新生成");
                }
            }

            Debug.Log($"[WeaponUpgradeManager] 武器 {weaponId} 从 Lv.{oldLevel} 升级到 Lv.{weapon.level}");

            return true;
        }

        /// <summary>
        /// 清除指定武器的所有轨道实体
        /// </summary>
        private void DestroyOrbitWeapons(World world, int playerEntity, string weaponId)
        {
            // 查找所有属于这个武器的轨道实体
            var entitiesToDestroy = new System.Collections.Generic.List<int>();

            foreach (var (entity, orbit) in world.GetComponents<OrbitComponent>())
            {
                if (orbit.centerEntity == playerEntity)
                {
                    // 这里可以添加更精确的判断,比如通过武器ID标记
                    // 暂时清除所有轨道武器
                    entitiesToDestroy.Add(entity);
                }
            }

            foreach (var entity in entitiesToDestroy)
            {
                world.DestroyEntity(entity);
            }

            Debug.Log($"[WeaponUpgradeManager] 清除了 {entitiesToDestroy.Count} 个轨道武器实体");
        }

        /// <summary>
        /// 添加新武器
        /// </summary>
        public bool AddWeapon(World world, int playerEntity, string weaponId)
        {
            if (!world.TryGetComponent<WeaponRuntimeStatsComponent>(playerEntity, out var weaponStats))
            {
                Debug.LogError($"[WeaponUpgradeManager] Player entity {playerEntity} 没有 WeaponRuntimeStatsComponent");
                return false;
            }

            // 检查是否已拥有该武器
            if (weaponStats.HasWeapon(weaponId))
            {
                Debug.LogWarning($"[WeaponUpgradeManager] 玩家已拥有武器 {weaponId}, 将进行升级");
                return UpgradeWeapon(world, playerEntity, weaponId);
            }

            // 获取武器配置
            if (!_weaponConfigDB.Data.TryGetValue(weaponId, out var weaponConfig))
            {
                Debug.LogError($"[WeaponUpgradeManager] 找不到武器配置: {weaponId}");
                return false;
            }

            // 创建新武器数据
            var weapon = weaponStats.AddWeapon(weaponId, 1);
            
            // 初始化运行时属性
            InitializeWeaponStats(weapon, weaponConfig);

            Debug.Log($"[WeaponUpgradeManager] 添加新武器: {weaponId} Lv.1");

            return true;
        }

        /// <summary>
        /// 初始化武器运行时状态
        /// 只设置 level，不写任何基础数值
        /// </summary>
        private void InitializeWeaponStats(
            WeaponRuntimeStats weapon,
            WeaponConfig weaponConfig)
        {
            // 初始等级
            weapon.level = 1;

            // 清空所有修正（防止重复初始化）
            weapon.ResetModifiers();

#if UNITY_EDITOR
            Debug.Log($"[WeaponUpgradeManager] 初始化武器 {weapon.weaponId} RuntimeStats (level=1)");
#endif
        }


        /// <summary>
        /// 应用升级规则（只修改 RuntimeStats）
        /// </summary>
        private void ApplyUpgradeRules(
            WeaponRuntimeStats weapon,
            WeaponUpgradeRuleDef upgradeRule,
            int fromLevel,
            int toLevel)
        {
            foreach (var rule in upgradeRule.rules)
            {
                if (!ShouldApplyRule(rule, fromLevel, toLevel))
                    continue;

                ApplySingleRule(weapon, rule);
            }

#if UNITY_EDITOR
            Debug.Log(
                $"[WeaponUpgradeManager] 武器 {weapon.weaponId} 升级到 Lv.{toLevel}");
#endif
        }


        /// <summary>
        /// 判断是否应该应用某个规则
        /// </summary>
        private bool ShouldApplyRule(StatUpgradeRule rule, int fromLevel, int toLevel)
        {
            // 如果没有设置 every，则每级都应用
            if (rule.every == 0)
                return true;

            // 检查当前等级是否是应该应用的等级
            return toLevel % rule.every == 0;
        }

        /// <summary>
        /// 应用单个升级规则到 RuntimeStats
        /// </summary>
        private void ApplySingleRule(
            WeaponRuntimeStats stats,
            StatUpgradeRule rule)
        {
            switch (rule.stat)
            {
                case "Damage":
                    if (rule.op == "Add")
                    {
                        stats.damageAdd += rule.value;
                    }
                    else if (rule.op == "Mul")
                    {
                        stats.damageMul *= rule.value;
                    }

                    break;

                case "Count":
                    if (rule.op == "Add")
                    {
                        stats.countAdd += (int)rule.value;
                    }

                    break;

                case "Cooldown":
                    if (rule.op == "Mul")
                    {
                        stats.fireRateMul *= rule.value;
                    }

                    break;

                case "Radius":
                    if (rule.op == "Add")
                    {
                        stats.orbitRadiusAdd += rule.value;
                    }

                    break;

                case "Speed":
                    if (rule.op == "Add")
                    {
                        stats.speedAdd += rule.value;
                    }

                    break;

                case "Range":
                    if (rule.op == "Add")
                    {
                        stats.rangeAdd += rule.value;
                    }

                    break;

                case "Knockback":
                    if (rule.op == "Add")
                    {
                        stats.knockbackAdd += rule.value;
                    }

                    break;

                default:
                    Debug.LogWarning(
                        $"[WeaponUpgradeManager] 未知升级属性: {rule.stat}");
                    break;
            }
        }


        /// <summary>
        /// 获取武器当前等级
        /// </summary>
        public int GetWeaponLevel(World world, int playerEntity, string weaponId)
        {
            if (!world.TryGetComponent<WeaponRuntimeStatsComponent>(playerEntity, out var weaponStats))
                return 0;

            if (!weaponStats.TryGetRuntime(weaponId, out var weapon))
                return 0;
                
            return weapon.level;
        }

        /// <summary>
        /// 获取武器最大等级
        /// </summary>
        public int GetWeaponMaxLevel(string weaponId)
        {
            if (!_upgradeRuleDB.Data.TryGetValue(weaponId, out var upgradeRule))
                return 0;

            return upgradeRule.maxLevel;
        }

        /// <summary>
        /// 检查武器是否可以升级
        /// </summary>
        public bool CanUpgradeWeapon(World world, int playerEntity, string weaponId)
        {
            int currentLevel = GetWeaponLevel(world, playerEntity, weaponId);
            if (currentLevel == 0)
                return false;

            int maxLevel = GetWeaponMaxLevel(weaponId);
            return currentLevel < maxLevel;
        }

        /// <summary>
        /// 获取武器运行时属性
        /// </summary>
        public bool TryGetWeaponStats(World world, int playerEntity, string weaponId,
            out WeaponRuntimeStats stats)
        {
            stats = null;

            if (!world.TryGetComponent<WeaponRuntimeStatsComponent>(playerEntity, out var statsComponent))
                return false;

            return statsComponent.TryGetRuntime(weaponId, out stats);
        }
    }
}
