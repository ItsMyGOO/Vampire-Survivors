using System.Collections.Generic;
using ECS.Core;

namespace ECS.Systems
{
    /// <summary>
    /// 属性计算系统
    /// 职责：
    /// 1. 从基础属性和修改器计算最终属性
    /// 2. 应用公式: final = (base + additive) * (1 + multiplicative)
    /// 3. 将计算结果写入 PlayerAttributeComponent
    ///
    /// 优化：
    /// - 脏标记模式：仅计算标记为 dirty 的实体
    /// - 复用字典和临时列表，零热路径 GC 分配
    /// - CalculateFinalAttributes 直接写入现有对象，不分配新实例
    /// </summary>
    public class AttributeCalculationSystem : SystemBase
    {
        private readonly Dictionary<AttributeType, float> _additiveMap = new Dictionary<AttributeType, float>();
        private readonly Dictionary<AttributeType, float> _multiplicativeMap = new Dictionary<AttributeType, float>();

        // 复用临时列表：从 GetEntitiesWithComponent 返回的缓冲区是共享的，
        // 遍历中调用 RemoveComponent 会修改它，所以先快照一份
        private readonly List<int> _dirtySnapshot = new List<int>();

public override void Update(World world, float deltaTime)
        {
            // 直接填入复用 buffer，零 GC 分配
            world.GetEntitiesWithComponent<AttributeDirtyComponent>(_dirtySnapshot);

            for (int i = 0; i < _dirtySnapshot.Count; i++)
            {
                int entityId = _dirtySnapshot[i];
                CalculateAttributes(world, entityId);
                // Dirty 标记由 AttributeSyncSystem 在同步完成后统一移除，
                // 确保 Sync 能感知到本帧发生了属性变化。
            }
        }

        private void CalculateAttributes(World world, int entityId)
        {
            if (!world.TryGetComponent<BaseAttributeComponent>(entityId, out var baseAttr))
                return;

            // 获取或创建 final 组件（只在首次时分配一次）
            if (!world.TryGetComponent<PlayerAttributeComponent>(entityId, out var final))
            {
                final = new PlayerAttributeComponent();
                world.AddComponent(entityId, final);
            }

            if (!world.TryGetComponent<AttributeModifierCollectionComponent>(entityId, out var modifiers))
            {
                CopyFromBase(baseAttr, final);
                return;
            }

            _additiveMap.Clear();
            _multiplicativeMap.Clear();

            var mods = modifiers.modifiers;
            for (int i = 0; i < mods.Count; i++)
            {
                var modifier = mods[i];
                if (modifier.modifierType == ModifierType.Additive)
                {
                    _additiveMap.TryGetValue(modifier.attributeType, out float cur);
                    _additiveMap[modifier.attributeType] = cur + modifier.value;
                }
                else if (modifier.modifierType == ModifierType.Multiplicative)
                {
                    _multiplicativeMap.TryGetValue(modifier.attributeType, out float cur);
                    _multiplicativeMap[modifier.attributeType] = cur + modifier.value;
                }
            }

            // 直接写入现有 final 对象，零分配
            WriteFinalAttributes(baseAttr, _additiveMap, _multiplicativeMap, final);
        }

        private static void CopyFromBase(BaseAttributeComponent src, PlayerAttributeComponent dst)
        {
            dst.maxHealth        = src.maxHealth;
            dst.healthRegen      = src.healthRegen;
            dst.moveSpeed        = src.moveSpeed;
            dst.armor            = src.armor;
            dst.attackDamage     = src.attackDamage;
            dst.attackSpeed      = src.attackSpeed;
            dst.criticalChance   = src.criticalChance;
            dst.criticalDamage   = src.criticalDamage;
            dst.projectileSpeed  = src.projectileSpeed;
            dst.areaSize         = src.areaSize;
            dst.projectileCount  = src.projectileCount;
            dst.pierceCount      = src.pierceCount;
            dst.pickupRange      = src.pickupRange;
            dst.expGain          = src.expGain;
            dst.cooldownReduction = src.cooldownReduction;
            dst.duration         = src.duration;
            dst.dodgeChance      = src.dodgeChance;
            dst.damageReduction  = src.damageReduction;
            dst.damageMul        = 1f;
            dst.cooldownMul      = 1f;
            dst.projectileSpeedMul = 1f;
        }

        private static void WriteFinalAttributes(
            BaseAttributeComponent baseAttr,
            Dictionary<AttributeType, float> additive,
            Dictionary<AttributeType, float> multiplicative,
            PlayerAttributeComponent dst)
        {
            float Calc(AttributeType type, float baseValue)
            {
                float add = additive.TryGetValue(type, out var a) ? a : 0f;
                float mul = multiplicative.TryGetValue(type, out var m) ? m : 0f;
                return (baseValue + add) * (1f + mul);
            }

            int CalcInt(AttributeType type, int baseValue)
            {
                float add = additive.TryGetValue(type, out var a) ? a : 0f;
                float mul = multiplicative.TryGetValue(type, out var m) ? m : 0f;
                return (int)((baseValue + add) * (1f + mul));
            }

            dst.maxHealth        = Calc(AttributeType.MaxHealth,        baseAttr.maxHealth);
            dst.healthRegen      = Calc(AttributeType.HealthRegen,      baseAttr.healthRegen);
            dst.moveSpeed        = Calc(AttributeType.MoveSpeed,        baseAttr.moveSpeed);
            dst.armor            = Calc(AttributeType.Armor,            baseAttr.armor);
            dst.attackDamage     = Calc(AttributeType.AttackDamage,     baseAttr.attackDamage);
            dst.attackSpeed      = Calc(AttributeType.AttackSpeed,      baseAttr.attackSpeed);
            dst.criticalChance   = Calc(AttributeType.CriticalChance,   baseAttr.criticalChance);
            dst.criticalDamage   = Calc(AttributeType.CriticalDamage,   baseAttr.criticalDamage);
            dst.projectileSpeed  = Calc(AttributeType.ProjectileSpeed,  baseAttr.projectileSpeed);
            dst.areaSize         = Calc(AttributeType.AreaSize,         baseAttr.areaSize);
            dst.projectileCount  = CalcInt(AttributeType.ProjectileCount, baseAttr.projectileCount);
            dst.pierceCount      = CalcInt(AttributeType.PierceCount,   baseAttr.pierceCount);
            dst.pickupRange      = Calc(AttributeType.PickupRange,      baseAttr.pickupRange);
            dst.expGain          = Calc(AttributeType.ExpGain,          baseAttr.expGain);
            dst.cooldownReduction = Calc(AttributeType.CooldownReduction, baseAttr.cooldownReduction);
            dst.duration         = Calc(AttributeType.Duration,         baseAttr.duration);
            dst.dodgeChance      = Calc(AttributeType.DodgeChance,      baseAttr.dodgeChance);
            dst.damageReduction  = Calc(AttributeType.DamageReduction,  baseAttr.damageReduction);

            dst.damageMul          = 1f + (multiplicative.TryGetValue(AttributeType.AttackDamage,    out var dmg) ? dmg : 0f);
            dst.cooldownMul        = 1f - dst.cooldownReduction;
            dst.projectileSpeedMul = 1f + (multiplicative.TryGetValue(AttributeType.ProjectileSpeed, out var ps)  ? ps  : 0f);
        }
    }
}