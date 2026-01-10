using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    /// <summary>
    /// 轨道系统
    /// 职责: 处理围绕玩家旋转的武器（如刀）
    /// </summary>
    public class OrbitSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            foreach (var (entity, orbit) in world.GetComponents<OrbitComponent>())
            {
                if (!world.HasComponent<PositionComponent>(entity)) continue;

                // 更新旋转角度
                orbit.current_angle += orbit.angular_speed * deltaTime;

                // 获取中心位置
                if (!world.HasComponent<PositionComponent>(orbit.center_entity))
                {
                    world.DestroyEntity(entity);
                    continue;
                }

                var centerPos = world.GetComponent<PositionComponent>(orbit.center_entity);
                var position = world.GetComponent<PositionComponent>(entity);

                // 计算轨道位置
                position.x = centerPos.x + Mathf.Cos(orbit.current_angle) * orbit.radius;
                position.y = centerPos.y + Mathf.Sin(orbit.current_angle) * orbit.radius;
            }
        }
    }
}