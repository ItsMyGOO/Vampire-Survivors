using ECS.Core;
using ConfigHandler;
using UnityEngine;

namespace ECS.Systems
{
    /// <summary>
    /// 被动升级处理系统
    /// 职责：
    /// 1. 监听 PassiveUpgradeIntentComponent
    /// 2. 更新被动等级状态
    /// 3. 将被动效果转换为属性修改器
    /// 4. 清理已处理的意图组件
    /// </summary>
    public class PassiveUpgradeSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            // 查找所有带有升级意图的实体
            var entities = world.GetEntitiesWithComponent<PassiveUpgradeIntentComponent>();

            foreach (var entityId in entities)
            {
                ProcessUpgradeIntent(world, entityId);
                
                // 移除已处理的意图组件
                world.RemoveComponent<PassiveUpgradeIntentComponent>(entityId);
            }
        }

        private void ProcessUpgradeIntent(World world, int entityId)
        {
            // 获取升级意图
            if (!world.TryGetComponent<PassiveUpgradeIntentComponent>(entityId, out var intent))
                return;

            // 获取或创建被动状态组件
            PassiveUpgradeStateComponent passiveState;
            if (!world.TryGetComponent<PassiveUpgradeStateComponent>(entityId, out passiveState))
            {
                passiveState = PassiveUpgradeStateComponent.Create();
                world.AddComponent(entityId, passiveState);
            }

            // 更新等级
            int currentLevel = 0;
            if (passiveState.levels.TryGetValue(intent.passiveId, out var level))
            {
                currentLevel = level;
            }

            int newLevel = currentLevel + intent.levelDelta;

            // 检查配置
            var db = PassiveUpgradePoolConfigDB.Instance;
            if (db == null)
            {
                Debug.LogError("[PassiveUpgradeSystem] PassiveUpgradePoolConfigDB 未初始化");
                return;
            }

            PassiveUpgradePoolDef def;
            try
            {
                def = db.Get(intent.passiveId);
            }
            catch
            {
                Debug.LogError($"[PassiveUpgradeSystem] 找不到被动配置: {intent.passiveId}");
                return;
            }

            // 检查最大等级
            if (def.excludeIfMax && newLevel > def.maxLevel)
            {
                Debug.LogWarning($"[PassiveUpgradeSystem] 被动 {intent.passiveId} 已达最大等级");
                return;
            }

            // 更新等级
            passiveState.levels[intent.passiveId] = newLevel;
            world.AddComponent(entityId, passiveState);

            Debug.Log($"[PassiveUpgradeSystem] 被动 {intent.passiveId} 等级 {currentLevel} -> {newLevel}");

            // 应用被动效果（添加修改器）
            ApplyPassiveEffect(world, entityId, intent.passiveId, def, currentLevel, newLevel);
        }

        /// <summary>
        /// 将被动效果转换为属性修改器
        /// </summary>
        private void ApplyPassiveEffect(World world, int entityId, string passiveId, PassiveUpgradePoolDef def, int oldLevel, int newLevel)
        {
            // 获取或创建修改器集合
            AttributeModifierCollectionComponent modifierCollection;
            if (!world.TryGetComponent<AttributeModifierCollectionComponent>(entityId, out modifierCollection))
            {
                modifierCollection = AttributeModifierCollectionComponent.Create();
            }

            // 移除该被动的旧修改器
            modifierCollection.modifiers.RemoveAll(m => m.sourceId == passiveId);

            // 添加新修改器（基于新等级）
            if (newLevel > 0)
            {
                var attributeType = ConvertPassiveToAttribute(def.attribute);
                float totalValue = def.valuePerLevel * newLevel;

                var modifier = new  AttributeModifier(
                    sourceId: passiveId,
                    attributeType: attributeType,
                    value: totalValue,
                    modifierType: DetermineModifierType(def.attribute)
                );

                modifierCollection.modifiers.Add(modifier);

                Debug.Log($"[PassiveUpgradeSystem] 添加修改器: {attributeType} += {totalValue} (来源: {passiveId})");
            }

            // 更新组件
            world.AddComponent(entityId, modifierCollection);
            
            // 标记属性需要重新计算（脏标记模式优化）
            world.AddComponent(entityId, new AttributeDirtyComponent());
        }

        /// <summary>
        /// 转换被动属性类型到属性类型
        /// </summary>
        private  AttributeType ConvertPassiveToAttribute(PassiveAttributeType type)
        {
            switch (type)
            {
                case PassiveAttributeType.MaxHealth:         return  AttributeType.MaxHealth;
                case PassiveAttributeType.MoveSpeed:         return  AttributeType.MoveSpeed;
                case PassiveAttributeType.AttackDamage:      return  AttributeType.AttackDamage;
                case PassiveAttributeType.AttackSpeed:       return  AttributeType.AttackSpeed;
                case PassiveAttributeType.CriticalChance:    return  AttributeType.CriticalChance;
                case PassiveAttributeType.AreaSize:          return  AttributeType.AreaSize;
                case PassiveAttributeType.ProjectileCount:   return  AttributeType.ProjectileCount;
                case PassiveAttributeType.PickupRange:       return  AttributeType.PickupRange;
                case PassiveAttributeType.CooldownReduction: return  AttributeType.CooldownReduction;
                case PassiveAttributeType.Duration:          return  AttributeType.Duration;
                case PassiveAttributeType.ExpGain:           return  AttributeType.ExpGain;
                case PassiveAttributeType.ProjectileSpeed:   return  AttributeType.ProjectileSpeed;
                default:
                    Debug.LogWarning($"[PassiveUpgradeSystem] 未知被动属性类型: {type}");
                    return  AttributeType.MaxHealth;
            }
        }

        /// <summary>
        /// 确定修改器类型（加法或乘法）
        /// 根据属性语义决定
        /// </summary>
        private  ModifierType DetermineModifierType(PassiveAttributeType type)
        {
            switch (type)
            {
                // 绝对值增加（加法）
                case PassiveAttributeType.MaxHealth:
                case PassiveAttributeType.MoveSpeed:
                case PassiveAttributeType.PickupRange:
                case PassiveAttributeType.ProjectileCount:
                    return  ModifierType.Additive;

                // 百分比增加（乘法）
                case PassiveAttributeType.AttackDamage:
                case PassiveAttributeType.AttackSpeed:
                case PassiveAttributeType.CriticalChance:
                case PassiveAttributeType.AreaSize:
                case PassiveAttributeType.CooldownReduction:
                case PassiveAttributeType.Duration:
                case PassiveAttributeType.ExpGain:
                case PassiveAttributeType.ProjectileSpeed:
                    return  ModifierType.Multiplicative;

                default:
                    return  ModifierType.Additive;
            }
        }
    }
}
