using System;
using System.Collections.Generic;
using ConfigHandler;
using ECS;
using ECS.Core;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// 武器升级管理器 - 负责处理武器的升级逻辑
    /// </summary>
    public class WeaponUpgradeManager
    {
        private WeaponUpgradeRuleConfigDB _upgradeRuleDB;
        private WeaponConfigDB _weaponConfigDB;

        public WeaponUpgradeManager(
            WeaponUpgradeRuleConfigDB upgradeRuleDB,
            WeaponConfigDB weaponConfigDB)
        {
            _upgradeRuleDB = upgradeRuleDB;
            _weaponConfigDB = weaponConfigDB;
        }

        /// <summary>
        /// 升级指定武器
        /// </summary>
        public bool UpgradeWeapon(World world, int playerEntity, string weaponId)
        {
            // 获取武器槽组件
            if (!world.TryGetComponent<WeaponSlotsComponent>(playerEntity, out var weaponSlots))
            {
                Debug.LogError($"[WeaponUpgradeManager] Player entity {playerEntity} 没有 WeaponSlotsComponent");
                return false;
            }

            // 查找武器
            var weaponData = weaponSlots.weapons.Find(w => w.weapon_type == weaponId);
            if (weaponData == null)
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
            if (weaponData.level >= upgradeRule.maxLevel)
            {
                Debug.LogWarning($"[WeaponUpgradeManager] 武器 {weaponId} 已达到最大等级 {upgradeRule.maxLevel}");
                return false;
            }

            // 升级
            int oldLevel = weaponData.level;
            weaponData.level++;

            // 获取或创建运行时属性组件
            if (!world.TryGetComponent<WeaponRuntimeStatsComponent>(playerEntity, out var runtimeStats))
            {
                runtimeStats = new WeaponRuntimeStatsComponent();
                world.AddComponent(playerEntity, runtimeStats);
            }

            // 应用升级效果到运行时属性
            ApplyUpgradeRules(runtimeStats, weaponId, upgradeRule, oldLevel, weaponData.level);

            Debug.Log($"[WeaponUpgradeManager] 武器 {weaponId} 从 Lv.{oldLevel} 升级到 Lv.{weaponData.level}");

            return true;
        }

        /// <summary>
        /// 添加新武器
        /// </summary>
        public bool AddWeapon(World world, int playerEntity, string weaponId)
        {
            if (!world.TryGetComponent<WeaponSlotsComponent>(playerEntity, out var weaponSlots))
            {
                Debug.LogError($"[WeaponUpgradeManager] Player entity {playerEntity} 没有 WeaponSlotsComponent");
                return false;
            }

            // 检查是否已拥有该武器
            var existingWeapon = weaponSlots.weapons.Find(w => w.weapon_type == weaponId);
            if (existingWeapon != null)
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
            var newWeapon = new WeaponSlotsComponent.WeaponData
            {
                weapon_type = weaponId,
                level = 1,
                cooldown = 0f,
                fire_rate = weaponConfig.battle.Type == WeaponType.Projectile 
                    ? weaponConfig.battle.projectile.interval 
                    : 1.0f,
                orbitSpawned = false
            };

            weaponSlots.weapons.Add(newWeapon);

            // 初始化运行时属性
            if (!world.TryGetComponent<WeaponRuntimeStatsComponent>(playerEntity, out var runtimeStats))
            {
                runtimeStats = new WeaponRuntimeStatsComponent();
                world.AddComponent(playerEntity, runtimeStats);
            }

            InitializeWeaponStats(runtimeStats, weaponId, weaponConfig);

            Debug.Log($"[WeaponUpgradeManager] 添加新武器: {weaponId} Lv.1");

            return true;
        }

        /// <summary>
        /// 初始化武器的基础属性
        /// </summary>
        private void InitializeWeaponStats(
            WeaponRuntimeStatsComponent runtimeStats,
            string weaponId,
            WeaponConfig weaponConfig)
        {
            var stats = runtimeStats.GetOrCreateStats(weaponId);
            var battle = weaponConfig.battle;

            stats.level = 1;
            stats.damage = battle.baseStats.damage;
            stats.projectileCount = battle.baseStats.count;
            stats.knockback = battle.baseStats.knockback;

            if (battle.Type == WeaponType.Projectile)
            {
                stats.fireRate = battle.projectile.interval;
                stats.projectileSpeed = battle.projectile.speed;
                stats.projectileRange = battle.projectile.range;
            }
            else if (battle.Type == WeaponType.Orbit)
            {
                stats.orbitRadius = battle.orbit.radius;
                stats.orbitSpeed = battle.orbit.speed;
                stats.orbitCount = battle.baseStats.count;
            }

            Debug.Log($"[WeaponUpgradeManager] 初始化武器 {weaponId} 属性 - 伤害:{stats.damage}, 数量:{stats.projectileCount}");
        }

        /// <summary>
        /// 应用升级规则
        /// </summary>
        private void ApplyUpgradeRules(
            WeaponRuntimeStatsComponent runtimeStats,
            string weaponId,
            WeaponUpgradeRuleDef upgradeRule,
            int fromLevel,
            int toLevel)
        {
            var stats = runtimeStats.GetOrCreateStats(weaponId);
            stats.level = toLevel;

            foreach (var rule in upgradeRule.rules)
            {
                // 检查是否需要在这个等级应用规则
                if (!ShouldApplyRule(rule, fromLevel, toLevel))
                    continue;

                ApplySingleRule(stats, rule);
            }

            Debug.Log($"[WeaponUpgradeManager] 武器 {weaponId} 升级后属性 - " +
                     $"伤害:{stats.GetFinalDamage()}, 数量:{stats.GetFinalProjectileCount()}");
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
        /// 应用单个规则到武器属性
        /// </summary>
        private void ApplySingleRule(WeaponRuntimeStatsComponent.WeaponStats stats, StatUpgradeRule rule)
        {
            switch (rule.stat)
            {
                case "Damage":
                    if (rule.op == "Add")
                    {
                        stats.damageAdd += rule.value;
                        Debug.Log($"[WeaponUpgradeManager] Damage +{rule.value}, 当前总伤害: {stats.GetFinalDamage()}");
                    }
                    else if (rule.op == "Mul")
                    {
                        stats.damageMultiply *= rule.value;
                        Debug.Log($"[WeaponUpgradeManager] Damage x{rule.value}, 当前总伤害: {stats.GetFinalDamage()}");
                    }
                    break;

                case "Count":
                    if (rule.op == "Add")
                    {
                        stats.countAdd += (int)rule.value;
                        Debug.Log($"[WeaponUpgradeManager] Count +{rule.value}, 当前数量: {stats.GetFinalProjectileCount()}");
                    }
                    break;

                case "Cooldown":
                    if (rule.op == "Mul")
                    {
                        stats.fireRateMultiply *= rule.value;
                        Debug.Log($"[WeaponUpgradeManager] FireRate x{rule.value}, 当前间隔: {stats.GetFinalFireRate()}");
                    }
                    break;

                case "Radius":
                    if (rule.op == "Add")
                    {
                        stats.radiusAdd += rule.value;
                        Debug.Log($"[WeaponUpgradeManager] Radius +{rule.value}, 当前半径: {stats.GetFinalOrbitRadius()}");
                    }
                    break;

                default:
                    Debug.LogWarning($"[WeaponUpgradeManager] 未知的升级属性: {rule.stat}");
                    break;
            }
        }

        /// <summary>
        /// 获取武器当前等级
        /// </summary>
        public int GetWeaponLevel(World world, int playerEntity, string weaponId)
        {
            if (!world.TryGetComponent<WeaponSlotsComponent>(playerEntity, out var weaponSlots))
                return 0;

            var weaponData = weaponSlots.weapons.Find(w => w.weapon_type == weaponId);
            return weaponData?.level ?? 0;
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
            out WeaponRuntimeStatsComponent.WeaponStats stats)
        {
            stats = null;
            
            if (!world.TryGetComponent<WeaponRuntimeStatsComponent>(playerEntity, out var runtimeStats))
                return false;

            return runtimeStats.TryGetStats(weaponId, out stats);
        }
    }
}
