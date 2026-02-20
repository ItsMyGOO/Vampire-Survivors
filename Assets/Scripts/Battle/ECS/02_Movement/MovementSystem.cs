using ECS.Core;

namespace ECS.Systems
{
    /// <summary>
    /// 移动系统
    /// 职责: 根据 Velocity 更新 Position
    /// </summary>
    public class MovementSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            foreach (var (entity, velocity) in world.GetComponents<VelocityComponent>())
            {
                if (!world.HasComponent<PositionComponent>(entity))
                    continue;

                var position = world.GetComponent<PositionComponent>(entity);
                position.x += velocity.x * deltaTime;
                position.y += velocity.y * deltaTime;
                world.SetComponent(entity, position);
            }
        }
    }
}