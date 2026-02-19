using ECS.Core;
using Battle.Upgrade;

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
            var entities = world.GetEntitiesWithComponent<PlayerAttributeComponent>();

            foreach (var entityId in entities)
            {
                SyncAttributes(world, entityId);
            }
        }

        private void SyncAttributes(World world, int entityId)
        {
            if (!world.TryGetComponent<PlayerAttributeComponent>(entityId, out var final))
                return;

            SyncMoveSpeed(world, entityId, final);
            SyncHealth(world, entityId, final);
            SyncPickupRange(world, entityId, final);
            SyncExpGain(world, final);
        }

        private void SyncMoveSpeed(World world, int entityId, PlayerAttributeComponent player)
        {
            if (world.TryGetComponent<VelocityComponent>(entityId, out var velocity))
                velocity.speed = player.moveSpeed;
        }

        private void SyncHealth(World world, int entityId, PlayerAttributeComponent player)
        {
            if (world.TryGetComponent<HealthComponent>(entityId, out var health))
            {
                float healthPercent = health.current / health.max;
                health.max = player.maxHealth;
                health.regen = player.healthRegen;
                health.current = health.max * healthPercent;
            }
        }

        private void SyncPickupRange(World world, int entityId, PlayerAttributeComponent player)
        {
            if (world.TryGetComponent<PickupRangeComponent>(entityId, out var pickup))
                pickup.radius = player.pickupRange;
        }

        private void SyncExpGain(World world, PlayerAttributeComponent player)
        {
            // 通过 World 服务注册表获取 ExpSystem，避免全局单例依赖
            if (world.TryGetService<IExpReceiver>(out _))
            {
                // IExpReceiver 不暴露 ExpData，通过已有的 ExpSystem 服务设置倍率
                // ExpSystem 注册为 IExpReceiver，需要转型访问 ExpData
            }

            if (world.TryGetService<ExpSystem>(out var expSystem))
                expSystem.ExpData.exp_multiplier = player.expGain;
        }
    }
}