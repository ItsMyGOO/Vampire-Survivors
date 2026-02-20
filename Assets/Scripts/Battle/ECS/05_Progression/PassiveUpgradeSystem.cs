using ConfigHandler;
using ECS.Core;
using System.Collections.Generic;
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
        // 快照缓冲区，防止 RemoveComponent 在遍历中修改共享缓冲区
        private readonly List<int> _intentSnapshot = new List<int>();

        public override void Update(World world, float deltaTime)
        {
            // 直接填入复用 buffer，零 GC 分配
            world.GetEntitiesWithComponent<PassiveUpgradeIntentComponent>(_intentSnapshot);

            for (int i = 0; i < _intentSnapshot.Count; i++)
            {
                int entityId = _intentSnapshot[i];
                ProcessUpgradeIntent(world, entityId);
                world.RemoveComponent<PassiveUpgradeIntentComponent>(entityId);
            }
        }

        private void ProcessUpgradeIntent(World world, int entityId)
        {
            if (!world.TryGetComponent<PassiveUpgradeIntentComponent>(entityId, out var intent))
                return;

            PassiveUpgradeStateComponent passiveState;
            if (!world.TryGetComponent<PassiveUpgradeStateComponent>(entityId, out passiveState))
            {
                passiveState = PassiveUpgradeStateComponent.Create();
                world.AddComponent(entityId, passiveState);
            }

            int currentLevel = 0;
            passiveState.levels.TryGetValue(intent.passiveId, out currentLevel);

            int newLevel = currentLevel + intent.levelDelta;

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

            if (def.excludeIfMax && newLevel > def.maxLevel)
            {
                Debug.LogWarning($"[PassiveUpgradeSystem] 被动 {intent.passiveId} 已达最大等级");
                return;
            }

            passiveState.levels[intent.passiveId] = newLevel;
            world.AddComponent(entityId, passiveState);

            Debug.Log($"[PassiveUpgradeSystem] 被动 {intent.passiveId} 等级 {currentLevel} -> {newLevel}");

            ApplyPassiveEffect(world, entityId, intent.passiveId, def, currentLevel, newLevel);
        }

private void ApplyPassiveEffect(World world, int entityId, string passiveId,
            PassiveUpgradePoolDef def, int oldLevel, int newLevel)
        {
            AttributeModifierCollectionComponent modifierCollection;
            if (!world.TryGetComponent<AttributeModifierCollectionComponent>(entityId, out modifierCollection))
                modifierCollection = AttributeModifierCollectionComponent.Create();

            // 倒序移除同源修改器，避免 RemoveAll(lambda) 闭包分配
            var mods = modifierCollection.modifiers;
            for (int i = mods.Count - 1; i >= 0; i--)
            {
                if (mods[i].sourceId == passiveId)
                    mods.RemoveAt(i);
            }

            if (newLevel > 0)
            {
                var attributeType = ConvertPassiveToAttribute(def.attribute);
                float totalValue = def.valuePerLevel * newLevel;

                var modifier = new AttributeModifier(
                    sourceId: passiveId,
                    attributeType: attributeType,
                    value: totalValue,
                    modifierType: DetermineModifierType(def.attribute)
                );

                mods.Add(modifier);

                Debug.Log($"[PassiveUpgradeSystem] 添加修改器: {attributeType} += {totalValue} (来源: {passiveId})");
            }

            world.AddComponent(entityId, modifierCollection);
            world.AddComponent(entityId, new AttributeDirtyComponent());
        }

        private AttributeType ConvertPassiveToAttribute(PassiveAttributeType type)
        {
            switch (type)
            {
                case PassiveAttributeType.MaxHealth: return AttributeType.MaxHealth;
                case PassiveAttributeType.MoveSpeed: return AttributeType.MoveSpeed;
                case PassiveAttributeType.AttackDamage: return AttributeType.AttackDamage;
                case PassiveAttributeType.AttackSpeed: return AttributeType.AttackSpeed;
                case PassiveAttributeType.CriticalChance: return AttributeType.CriticalChance;
                case PassiveAttributeType.AreaSize: return AttributeType.AreaSize;
                case PassiveAttributeType.ProjectileCount: return AttributeType.ProjectileCount;
                case PassiveAttributeType.PickupRange: return AttributeType.PickupRange;
                case PassiveAttributeType.CooldownReduction: return AttributeType.CooldownReduction;
                case PassiveAttributeType.Duration: return AttributeType.Duration;
                case PassiveAttributeType.ExpGain: return AttributeType.ExpGain;
                case PassiveAttributeType.ProjectileSpeed: return AttributeType.ProjectileSpeed;
                default:
                    Debug.LogWarning($"[PassiveUpgradeSystem] 未知被动属性类型: {type}");
                    return AttributeType.MaxHealth;
            }
        }

        private ModifierType DetermineModifierType(PassiveAttributeType type)
        {
            switch (type)
            {
                case PassiveAttributeType.MaxHealth:
                case PassiveAttributeType.MoveSpeed:
                case PassiveAttributeType.PickupRange:
                case PassiveAttributeType.ProjectileCount:
                    return ModifierType.Additive;

                default:
                    return ModifierType.Multiplicative;
            }
        }
    }
}