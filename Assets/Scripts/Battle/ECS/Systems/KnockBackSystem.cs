using System.Collections.Generic;
using ECS.Core;

namespace ECS.Systems
{
    /// <summary>
    /// 击退系统 - 直接修改位置版本
    /// 参考 Lua 实现：pos.x += forceX * dt
    /// 优势：不会影响 AI 的速度计算和朝向
    /// </summary>
    public class KnockBackSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            var toRemove = new List<int>();

            foreach (var (entity, knockback) in world.GetComponents<KnockBackComponent>())
            {
                // 检查是否有位置组件
                if (!world.HasComponent<PositionComponent>(entity))
                {
                    toRemove.Add(entity);
                    continue;
                }

                var position = world.GetComponent<PositionComponent>(entity);

                // 直接修改位置（不修改速度）
                position.x += knockback.forceX * deltaTime;
                position.y += knockback.forceY * deltaTime;

                // 更新剩余时间
                knockback.time -= deltaTime;

                // 检查是否结束
                if (knockback.time <= 0)
                {
                    toRemove.Add(entity);
                }
            }

            // 移除已完成的击退
            foreach (var entity in toRemove)
            {
                world.RemoveComponent<KnockBackComponent>(entity);
            }
        }
    }
}
