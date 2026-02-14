using System;
using Battle.Player;
using ECS.Core;
using UnityEngine;
using ConfigHandler;
using Battle.Weapon;
using ECS;

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

            // 1. 读取被动配置
            var db = PassiveUpgradePoolConfigDB.Instance;
            if (db == null)
            {
                Debug.LogError("[UpgradeApplyService] PassiveUpgradePoolConfigDB 未初始化");
                return;
            }

            PassiveUpgradePoolDef def;
            try
            {
                def = db.Get(passiveId);
            }
            catch (Exception e)
            {
                Debug.LogError($"[UpgradeApplyService] 找不到被动配置: {passiveId}\n{e.Message}");
                return;
            }

            // 2. 获取 / 创建被动等级组件
            var passiveState = world.GetComponent<PassiveUpgradeStateComponent>(player);
            if (passiveState == null)
            {
                passiveState = new PassiveUpgradeStateComponent();
                world.AddComponent(player, passiveState);
            }

            var currentLevel = passiveState.GetLevel(passiveId);
            if (def.excludeIfMax && currentLevel >= def.maxLevel)
            {
                Debug.LogWarning($"[UpgradeApplyService] 被动 {passiveId} 已达最大等级");
                return;
            }

            var newLevel = passiveState.AddLevel(passiveId, 1);

            // 3. 将被动转成属性修改
            var playerAttr = world.GetComponent<PlayerAttributeComponent>(player);
            if (playerAttr == null)
            {
                playerAttr = new PlayerAttributeComponent();
                world.AddComponent(player, playerAttr);
            }

            var attrType = ConvertPassiveToAttribute(def.attribute);
            var deltaValue = def.valuePerLevel;
            var modifier = new AttributeModifier(attrType, deltaValue);

            playerAttr.ApplyModifier(modifier);
            Debug.Log(
                $"[UpgradeApplyService] 被动 {passiveId} 等级 {currentLevel} -> {newLevel}, " +
                $"属性 {attrType} += {deltaValue}");

            // 4. 将属性同步到 ECS 组件 / 其他系统
            ApplyAttributeToECS(world, player, playerAttr, modifier);
        }

        /// <summary>
        /// PassiveAttributeType → AttributeType 映射
        /// </summary>
        private AttributeType ConvertPassiveToAttribute(PassiveAttributeType type)
        {
            switch (type)
            {
                case PassiveAttributeType.MaxHealth:         return AttributeType.MaxHealth;
                case PassiveAttributeType.MoveSpeed:         return AttributeType.MoveSpeed;
                case PassiveAttributeType.AttackDamage:      return AttributeType.AttackDamage;
                case PassiveAttributeType.AttackSpeed:       return AttributeType.AttackSpeed;
                case PassiveAttributeType.CriticalChance:    return AttributeType.CriticalChance;
                case PassiveAttributeType.AreaSize:          return AttributeType.AreaSize;
                case PassiveAttributeType.ProjectileCount:   return AttributeType.ProjectileCount;
                case PassiveAttributeType.PickupRange:       return AttributeType.PickupRange;
                case PassiveAttributeType.CooldownReduction: return AttributeType.CooldownReduction;
                case PassiveAttributeType.Duration:          return AttributeType.Duration;
                case PassiveAttributeType.ExpGain:           return AttributeType.ExpGain;
                case PassiveAttributeType.ProjectileSpeed:   return AttributeType.ProjectileSpeed;
                default:
                    return AttributeType.MaxHealth;
            }
        }

        /// <summary>
        /// 根据属性修改器，将最新属性应用到 ECS 组件及其它系统。
        /// 这样可以在不重写所有 System 的前提下，实现被动对角色数值的影响。
        /// </summary>
        private void ApplyAttributeToECS(World world, int player, PlayerAttributeComponent attr, AttributeModifier modifier)
        {
            switch (modifier.attributeType)
            {
                case AttributeType.MaxHealth:
                {
                    var health = world.GetComponent<HealthComponent>(player);
                    if (health != null)
                    {
                        health.max += modifier.value;
                        health.current += modifier.value;
                    }
                    break;
                }

                case AttributeType.HealthRegen:
                {
                    var health = world.GetComponent<HealthComponent>(player);
                    if (health != null)
                    {
                        health.regen += modifier.value;
                    }
                    break;
                }

                case AttributeType.MoveSpeed:
                {
                    var vel = world.GetComponent<VelocityComponent>(player);
                    if (vel != null)
                    {
                        // 用属性做源 of truth，赋值同步，避免累乘爆炸
                        vel.speed = attr.moveSpeed;
                    }
                    break;
                }

                case AttributeType.PickupRange:
                {
                    var pickup = world.GetComponent<PickupRangeComponent>(player);
                    if (pickup != null)
                    {
                        pickup.radius += modifier.value;
                    }
                    break;
                }

                case AttributeType.ExpGain:
                {
                    // 直接作用到经验系统倍率
                    if (PlayerContext.Instance?.ExpSystem != null)
                    {
                        PlayerContext.Instance.ExpSystem.ExpData.exp_multiplier += modifier.value;
                    }
                    break;
                }

                case AttributeType.AttackDamage:
                case AttributeType.CooldownReduction:
                case AttributeType.ProjectileSpeed:
                    // 已通过 PlayerAttributeComponent.damageMul / cooldownMul / projectileSpeedMul 在 BuildFinalStats 中参与公式，不修改 weapon runtime
                    break;

                case AttributeType.ProjectileCount:
                {
                    if (world.TryGetComponent(player, out WeaponRuntimeStatsComponent weaponStats))
                    {
                        foreach (var kv in weaponStats.GetAllWeapons())
                        {
                            var runtime = kv.Value;
                            runtime.countAdd += (int)modifier.value;
                        }
                    }
                    break;
                }

                case AttributeType.AreaSize:
                case AttributeType.Duration:
                case AttributeType.AttackSpeed:
                case AttributeType.CriticalChance:
                case AttributeType.CriticalDamage:
                case AttributeType.PierceCount:
                case AttributeType.DodgeChance:
                case AttributeType.DamageReduction:
                default:
                    // 这些属性目前仅记录在 PlayerAttributeComponent 中，
                    // 未来可在对应的 System 中读取并生效。
                    break;
            }
        }
    }
}