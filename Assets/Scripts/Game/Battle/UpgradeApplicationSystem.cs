using ECS;
using ECS.Core;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 升级应用系统 - 负责将属性修改应用到ECS实体
    /// </summary>
    public class UpgradeApplicationSystem
    {
        private World world;
        
        public UpgradeApplicationSystem(World world)
        {
            this.world = world;
        }

        /// <summary>
        /// 应用属性修改到指定实体
        /// </summary>
        public void ApplyAttributeModifier(int entity, AttributeType attributeType, float value)
        {
            if (!world.HasComponent<PlayerAttributeComponent>(entity))
            {
                world.AddComponent(entity, new PlayerAttributeComponent());
            }

            PlayerAttributeComponent attributes = world.GetComponent<PlayerAttributeComponent>(entity);
            AttributeModifier modifier = new AttributeModifier(attributeType, value);
            attributes.ApplyModifier(modifier);

            // 同步到其他相关组件
            SyncToComponents(entity, attributes, attributeType);
            
            Debug.Log($"[UpgradeApplicationSystem] Applied {attributeType}: +{value} to entity {entity}");
        }

        /// <summary>
        /// 同步属性到其他ECS组件
        /// </summary>
        private void SyncToComponents(int entity, PlayerAttributeComponent attributes, AttributeType modifiedType)
        {
            switch (modifiedType)
            {
                case AttributeType.MaxHealth:
                case AttributeType.HealthRegen:
                    SyncHealthComponent(entity, attributes);
                    break;
                    
                case AttributeType.MoveSpeed:
                    SyncVelocityComponent(entity, attributes);
                    break;
                    
                case AttributeType.PickupRange:
                    SyncPickupComponent(entity, attributes);
                    break;
            }
        }

        private void SyncHealthComponent(int entity, PlayerAttributeComponent attributes)
        {
            if (!world.HasComponent<HealthComponent>(entity))
            {
                world.AddComponent(entity, new HealthComponent(attributes.maxHealth, attributes.maxHealth, attributes.healthRegen));
                return;
            }

            HealthComponent health = world.GetComponent<HealthComponent>(entity);
            float healthPercentage = health.current / health.max;
            
            health.max = attributes.maxHealth;
            health.regen = attributes.healthRegen;
            
            // 保持生命值百分比
            health.current = health.max * healthPercentage;
        }

        private void SyncVelocityComponent(int entity, PlayerAttributeComponent attributes)
        {
            if (!world.HasComponent<VelocityComponent>(entity))
            {
                return;
            }

            VelocityComponent velocity = world.GetComponent<VelocityComponent>(entity);
            velocity.speed = attributes.moveSpeed;
        }

        private void SyncPickupComponent(int entity, PlayerAttributeComponent attributes)
        {
            if (!world.HasComponent<PickupRangeComponent>(entity))
            {
                return;
            }

            PickupRangeComponent pickup = world.GetComponent<PickupRangeComponent>(entity);
            pickup.radius = attributes.pickupRange;
        }

        /// <summary>
        /// 获取实体的完整属性信息
        /// </summary>
        public PlayerAttributeComponent GetAttributes(int entity)
        {
            return world.GetComponent<PlayerAttributeComponent>(entity);
        }

        /// <summary>
        /// 重置实体属性为默认值
        /// </summary>
        public void ResetAttributes(int entity)
        {
            if (world.HasComponent<PlayerAttributeComponent>(entity))
            {
                world.RemoveComponent<PlayerAttributeComponent>(entity);
            }
            
            world.AddComponent(entity, new PlayerAttributeComponent());
            Debug.Log($"[UpgradeApplicationSystem] Reset attributes for entity {entity}");
        }
    }
}
