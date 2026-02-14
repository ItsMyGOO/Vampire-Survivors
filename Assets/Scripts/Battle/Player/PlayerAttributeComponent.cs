using System;

namespace Battle.Player
{
    /// <summary>
    /// 玩家属性组件 - 用于升级和属性提升
    /// </summary>
    [Serializable]
    public class PlayerAttributeComponent
    {
        // 基础属性
        public float maxHealth = 100f;           // 最大生命值
        public float healthRegen = 0f;           // 生命回复/秒
        public float moveSpeed = 2f;             // 移动速度（与 VelocityComponent.speed 一致，用赋值同步避免累乘爆炸）
        public float armor = 0f;                 // 护甲值
        
        // 攻击属性（展示用增量）
        public float attackDamage = 0f;          // 伤害倍率增量（0.1 = +10%）
        public float attackSpeed = 1f;           // 攻击速度倍率，预留
        public float criticalChance = 0f;        // 暴击率 (0-1)
        public float criticalDamage = 1.5f;     // 暴击伤害倍率
        public float projectileSpeed = 0f;       // 弹道速度倍率增量，预留展示
        
        // 最终参与公式的倍率（仅由被动累乘，不在 ApplyAttributeToECS 里改 weapon runtime）
        // finalDamage = baseDamage * levelMul * damageMul * weapon.damageMul
        public float damageMul = 1f;             // 玩家全局伤害倍率
        public float cooldownMul = 1f;           // 冷却间隔倍率（0.95 = 间隔缩短 5%）
        public float projectileSpeedMul = 1f;    // 玩家全局弹道速度倍率
        
        // 范围和数量
        public float areaSize = 1f;              // 范围大小倍率，预留
        public int projectileCount = 0;          // 额外弹道数量
        public int pierceCount = 0;              // 穿透次数
        
        // 特殊属性
        public float pickupRange = 2f;            // 拾取范围
        public float expGain = 1f;               // 经验获取倍率
        public float cooldownReduction = 0f;      // 冷却缩减 (0-0.9)，展示用
        public float duration = 1f;               // 持续时间倍率，预留
        
        // 生存属性
        public float dodgeChance = 0f;           // 闪避率 (0-1)
        public float damageReduction = 0f;       // 伤害减免 (0-1)
   
        /// <summary>
        /// 应用属性修改
        /// </summary>
        public void ApplyModifier(AttributeModifier modifier)
        {
            switch (modifier.attributeType)
            {
                case AttributeType.MaxHealth:
                    maxHealth += modifier.value;
                    break;
                case AttributeType.HealthRegen:
                    healthRegen += modifier.value;
                    break;
                case AttributeType.MoveSpeed:
                    moveSpeed += modifier.value;
                    break;
                case AttributeType.Armor:
                    armor += modifier.value;
                    break;
                case AttributeType.AttackDamage:
                    attackDamage += modifier.value;
                    damageMul *= (1f + modifier.value);
                    break;
                case AttributeType.AttackSpeed:
                    attackSpeed += modifier.value;
                    break;
                case AttributeType.CriticalChance:
                    criticalChance += modifier.value;
                    break;
                case AttributeType.CriticalDamage:
                    criticalDamage += modifier.value;
                    break;
                case AttributeType.AreaSize:
                    areaSize += modifier.value;
                    break;
                case AttributeType.ProjectileCount:
                    projectileCount += (int)modifier.value;
                    break;
                case AttributeType.PierceCount:
                    pierceCount += (int)modifier.value;
                    break;
                case AttributeType.PickupRange:
                    pickupRange += modifier.value;
                    break;
                case AttributeType.ExpGain:
                    expGain += modifier.value;
                    break;
                case AttributeType.CooldownReduction:
                    cooldownReduction += modifier.value;
                    cooldownMul *= (1f - modifier.value);
                    break;
                case AttributeType.ProjectileSpeed:
                    projectileSpeed += modifier.value;
                    projectileSpeedMul *= (1f + modifier.value);
                    break;
            }
        }
    }

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
    /// 属性修改器
    /// </summary>
    [Serializable]
    public class AttributeModifier
    {
        public AttributeType attributeType;
        public float value;

        public AttributeModifier(AttributeType type, float value)
        {
            this.attributeType = type;
            this.value = value;
        }
    }
}
