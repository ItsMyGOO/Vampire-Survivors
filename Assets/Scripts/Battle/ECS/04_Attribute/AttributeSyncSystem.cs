using Battle.Upgrade;
using ECS.Core;
using System.Collections.Generic;

namespace ECS.Systems
{
    /// <summary>
    /// 属性同步系统
    /// 职责：将 PlayerAttributeComponent 的计算结果同步到其他 ECS 组件
    /// 在 AttributeCalculationSystem 之后执行
    /// </summary>
    public class AttributeSyncSystem : SystemBase
    {
public override void Update(World world, float deltaTime)
        {
            foreach (var (entityId, component) in world.GetComponents<PlayerAttributeComponent>())
            {
                SyncAttributes(world, entityId, component);
            }
        }

        private void SyncAttributes(World world, int entityId, PlayerAttributeComponent final)
        {
            SyncMoveSpeed(world, entityId, final);
            SyncHealth(world, entityId, final);
            SyncPickupRange(world, entityId, final);
            SyncExpGain(world, final);
        }

private void SyncMoveSpeed(World world, int entityId, PlayerAttributeComponent player)
        {
            if (world.TryGetComponent<VelocityComponent>(entityId, out var velocity))
            {
                velocity.speed = player.moveSpeed;
                world.SetComponent(entityId, velocity);
            }
        }

private void SyncHealth(World world, int entityId, PlayerAttributeComponent player)
        {
            if (world.TryGetComponent<HealthComponent>(entityId, out var health))
            {
                float healthPercent = health.current / health.max;
                health.max = player.maxHealth;
                health.regen = player.healthRegen;
                health.current = health.max * healthPercent;
                world.SetComponent(entityId, health);
            }
        }

private void SyncPickupRange(World world, int entityId, PlayerAttributeComponent player)
        {
            if (world.TryGetComponent<PickupRangeComponent>(entityId, out var pickup))
            {
                pickup.radius = player.pickupRange;
                world.SetComponent(entityId, pickup);
            }
        }

        private void SyncExpGain(World world, PlayerAttributeComponent player)
        {
            if (world.TryGetService<ExpSystem>(out var expSystem))
                expSystem.ExpData.exp_multiplier = player.expGain;
        }
    }
}