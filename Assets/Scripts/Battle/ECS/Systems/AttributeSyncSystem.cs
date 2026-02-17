using Battle.Player;
using ECS.Core;

namespace ECS.Systems
{
    /// <summary>
    /// 属性同步系统
    /// 职责：
    /// 1. 将 FinalAttributeComponent 的计算结果同步到其他 ECS 组件
    /// 2. 例如：同步移动速度到 VelocityComponent，同步生命值到 HealthComponent 等
    /// 
    /// 更新时机：
    /// - 在 AttributeCalculationSystem 之后执行
    /// - 确保其他系统使用的是最新的属性值
    /// </summary>
    public class AttributeSyncSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            // 遍历所有拥有最终属性的实体
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

            // 同步移动速度
            SyncMoveSpeed(world, entityId, final);

            // 同步生命值
            SyncHealth(world, entityId, final);

            // 同步拾取范围
            SyncPickupRange(world, entityId, final);

            // 同步经验倍率（如果有经验系统）
            SyncExpGain(world, entityId, final);

            // 其他属性按需同步
        }

        /// <summary>
        /// 同步移动速度到 VelocityComponent
        /// </summary>
        private void SyncMoveSpeed(World world, int entityId, PlayerAttributeComponent player)
        {
            if (world.TryGetComponent<VelocityComponent>(entityId, out var velocity))
            {
                velocity.speed = player.moveSpeed;
            }
        }

        /// <summary>
        /// 同步生命值到 HealthComponent
        /// 注意：只更新最大值，当前值按比例调整
        /// </summary>
        private void SyncHealth(World world, int entityId, PlayerAttributeComponent player)
        {
            if (world.TryGetComponent<HealthComponent>(entityId, out var health))
            {
                // 保持生命值百分比
                float healthPercent = health.current / health.max;
                
                health.max = player.maxHealth;
                health.regen = player.healthRegen;
                
                // 如果是新增最大生命，按比例增加当前生命
                // 避免加血上限后当前血量不变的问题
                health.current = health.max * healthPercent;
            }
        }

        /// <summary>
        /// 同步拾取范围到 PickupRangeComponent
        /// </summary>
        private void SyncPickupRange(World world, int entityId, PlayerAttributeComponent player)
        {
            if (world.TryGetComponent<PickupRangeComponent>(entityId, out var pickup))
            {
                pickup.radius = player.pickupRange;
            }
        }

        /// <summary>
        /// 同步经验增益到经验系统
        /// </summary>
        private void SyncExpGain(World world, int entityId, PlayerAttributeComponent player)
        {
            // 如果使用 PlayerContext 单例方式访问经验系统
            if ( PlayerContext.Instance?.ExpSystem != null)
            {
                 PlayerContext.Instance.ExpSystem.ExpData.exp_multiplier = player.expGain;
            }
        }
    }
}