using System.Collections.Generic;
using ECS.Core;

namespace ECS.Systems
{
    /// <summary>
    /// 击退系统
    /// 职责: 处理击退效果
    /// </summary>
    public class KnockBackSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            var toRemove = new List<int>();

            foreach (var (entity, knockBack) in world.GetComponents<KnockBackComponent>())
            {
                knockBack.timer += deltaTime;

                if (knockBack.timer >= knockBack.duration)
                {
                    toRemove.Add(entity);
                    continue;
                }

                if (!world.HasComponent<VelocityComponent>(entity)) continue;

                var velocity = world.GetComponent<VelocityComponent>(entity);

                // 应用击退力
                float t = 1.0f - (knockBack.timer / knockBack.duration);
                velocity.x = knockBack.direction_x * knockBack.force * t;
                velocity.y = knockBack.direction_y * knockBack.force * t;
            }

            // 移除已完成的击退
            foreach (var entity in toRemove)
            {
                world.RemoveComponent<KnockBackComponent>(entity);
            }
        }
    }
}