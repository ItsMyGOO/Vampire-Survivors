using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    /// <summary>
    /// 投射物移动系统
    /// 职责: 处理投射物的特殊移动（追踪、轨道等）
    /// </summary>
    public class ProjectileMoveSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            // 处理追踪型投射物
            foreach (var (entity, seek) in world.GetComponents<SeekComponent>())
            {
                if (!world.HasComponent<PositionComponent>(entity) ||
                    !world.HasComponent<VelocityComponent>(entity))
                {
                    continue;
                }

                // 检查目标是否还存在
                if (!world.HasComponent<PositionComponent>(seek.target_id))
                {
                    world.RemoveComponent<SeekComponent>(entity);
                    continue;
                }

                var position = world.GetComponent<PositionComponent>(entity);
                var velocity = world.GetComponent<VelocityComponent>(entity);
                var targetPos = world.GetComponent<PositionComponent>(seek.target_id);

                // 计算朝向目标的方向
                float dx = targetPos.x - position.x;
                float dy = targetPos.y - position.y;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                if (dist > 0.1f)
                {
                    float dirX = dx / dist;
                    float dirY = dy / dist;

                    var projectile = world.GetComponent<ProjectileComponent>(entity);
                    float speed = projectile.speed;

                    // 更新速度朝向目标
                    velocity.x = dirX * speed;
                    velocity.y = dirY * speed;
                }
            }
        }
    }
}