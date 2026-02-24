using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    /// <summary>
    /// 游荡系统
    /// 职责: 驱动携带 WanderComponent 的实体在出生点半径内随机游荡。
    /// 依赖: WanderComponent + PositionComponent + VelocityComponent
    /// </summary>
    public class WanderSystem : SystemBase
    {
        private const float ARRIVE_THRESHOLD = 0.15f;

        public override void Update(World world, float deltaTime)
        {
            world.IterateComponents<WanderComponent>(out int[] ids, out _, out int count);

            for (int i = 0; i < count; i++)
            {
                int entity = ids[i];

                if (!world.HasComponent<PositionComponent>(entity) ||
                    !world.HasComponent<VelocityComponent>(entity))
                    continue;

                var wander = world.GetComponent<WanderComponent>(entity);
                var pos    = world.GetComponent<PositionComponent>(entity);
                var vel    = world.GetComponent<VelocityComponent>(entity);

                float dx   = wander.targetX - pos.x;
                float dy   = wander.targetY - pos.y;
                float dist = dx * dx + dy * dy;

                if (dist < ARRIVE_THRESHOLD * ARRIVE_THRESHOLD)
                {
                    // 到达目标：停止移动，等待后选取新目标
                    vel.x = 0f;
                    vel.y = 0f;

                    wander.waitTimer += deltaTime;
                    if (wander.waitTimer >= wander.waitDuration)
                    {
                        wander.waitTimer = 0f;
                        wander.targetX = wander.originX + Random.Range(-wander.radius, wander.radius);
                        wander.targetY = wander.originY + Random.Range(-wander.radius, wander.radius);
                    }
                }
                else
                {
                    // 向目标点匀速移动
                    float invDist = 1f / Mathf.Sqrt(dist);
                    vel.x = dx * invDist * wander.speed;
                    vel.y = dy * invDist * wander.speed;
                }

                world.SetComponent(entity, wander);
                world.SetComponent(entity, vel);
            }
        }
    }
}
