using System;
using System.Collections.Generic;

namespace ECS
{
    /// <summary>
    /// 属性类型枚举
    /// </summary>
    public enum AttributeType
    {
        MaxHealth,
        HealthRegen,
        MoveSpeed,
        Armor,
        AttackDamage,
        AttackSpeed,
        CriticalChance,
        CriticalDamage,
        AreaSize,
        ProjectileCount,
        PierceCount,
        PickupRange,
        ExpGain,
        CooldownReduction,
        ProjectileSpeed,
        Duration,
        DodgeChance,
        DamageReduction
    }

    /// <summary>
    /// 修改器类型
    /// </summary>
    public enum ModifierType
    {
        Additive,      // 加法: final = base + modifier
        Multiplicative // 乘法: final = base * (1 + modifier)
    }

    /// <summary>
    /// 单个属性修改器
    /// 纯数据结构，可追踪来源
    /// </summary>
    [Serializable]
    public class AttributeModifier
    {
        public string sourceId;           // 来源ID（被动ID、武器ID等）
        public AttributeType attributeType;
        public ModifierType modifierType;
        public float value;

        public AttributeModifier(string sourceId, AttributeType attributeType, float value, ModifierType modifierType = ModifierType.Additive)
        {
            this.sourceId = sourceId;
            this.attributeType = attributeType;
            this.modifierType = modifierType;
            this.value = value;
        }
    }

    /// <summary>
    /// 属性修改器集合组件
    /// 存储所有应用于该实体的修改器
    /// </summary>
    [Serializable]
    public class AttributeModifierCollectionComponent
    {
        // 所有修改器列表
        public List<AttributeModifier> modifiers;

        public static AttributeModifierCollectionComponent Create()
        {
            return new AttributeModifierCollectionComponent
            {
                modifiers = new List<AttributeModifier>()
            };
        }
    }

    /// <summary>
    /// 基础属性组件 - 纯数据
    /// 存储未经修改的基础值
    /// </summary>
    [Serializable]
    public class BaseAttributeComponent
    {
        // 基础属性
        public float maxHealth;
        public float healthRegen;
        public float moveSpeed;
        public float armor;

        // 攻击属性
        public float attackDamage;
        public float attackSpeed;
        public float criticalChance;
        public float criticalDamage;
        public float projectileSpeed;

        // 范围和数量
        public float areaSize;
        public int projectileCount;
        public int pierceCount;

        // 特殊属性
        public float pickupRange;
        public float expGain;
        public float cooldownReduction;
        public float duration;

        // 生存属性
        public float dodgeChance;
        public float damageReduction;

        public static BaseAttributeComponent CreateDefault()
        {
            return new BaseAttributeComponent
            {
                maxHealth = 100f,
                healthRegen = 0f,
                moveSpeed = 2f,
                armor = 0f,
                attackDamage = 0f,
                attackSpeed = 1f,
                criticalChance = 0f,
                criticalDamage = 1.5f,
                projectileSpeed = 1f,
                areaSize = 1f,
                projectileCount = 0,
                pierceCount = 0,
                pickupRange = 2f,
                expGain = 1f,
                cooldownReduction = 0f,
                duration = 1f,
                dodgeChance = 0f,
                damageReduction = 0f
            };
        }
    }

    /// <summary>
    /// 最终属性组件 - 纯数据
    /// 存储计算后的最终值
    /// 由 AttributeCalculationSystem 计算并更新
    /// </summary>
    [Serializable]
    public class PlayerAttributeComponent
    {
        // 与 BaseAttributeComponent 相同的字段结构
        public float maxHealth;
        public float healthRegen;
        public float moveSpeed;
        public float armor;
        public float attackDamage;
        public float attackSpeed;
        public float criticalChance;
        public float criticalDamage;
        public float projectileSpeed;
        public float areaSize;
        public int projectileCount;
        public int pierceCount;
        public float pickupRange;
        public float expGain;
        public float cooldownReduction;
        public float duration;
        public float dodgeChance;
        public float damageReduction;

        // 计算辅助字段
        public float damageMul;           // 伤害倍率
        public float cooldownMul;         // 冷却倍率
        public float projectileSpeedMul;  // 弹道速度倍率
    }
}
