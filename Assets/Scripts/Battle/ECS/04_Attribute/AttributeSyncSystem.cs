using Battle.Upgrade;
using ECS.Core;
using System.Collections.Generic;

namespace ECS.Systems
{
    /// <summary>
    /// 属性同步系统
    /// 职责：将 PlayerAttributeComponent 的计算结果同步到其他 ECS 组件
    ///
    /// 优化：脏标记驱动，只处理本帧发生过属性计算的实体。
    /// AttributeCalculationSystem 计算完成后保留 AttributeDirtyComponent，
    /// 本系统同步完成后统一 Remove，确保每帧属性变化只同步一次。
    /// </summary>
    public class AttributeSyncSystem : SystemBase
    {
        // 快照缓冲，防止遍历时 RemoveComponent 修改共享缓冲区
        private readonly List<int> _dirtySnapshot = new List<int>();

        public override void Update(World world, float deltaTime)
        {
            world.GetEntitiesWithComponent<AttributeDirtyComponent>(_dirtySnapshot);

            for (int i = 0; i < _dirtySnapshot.Count; i++)
            {
                int entityId = _dirtySnapshot[i];

                if (!world.TryGetComponent<PlayerAttributeComponent>(entityId, out var final))
                    continue;

                SyncAttributes(world, entityId, final);

                // Calc 留下的 Dirty 在此统一清理
                world.RemoveComponent<AttributeDirtyComponent>(entityId);
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