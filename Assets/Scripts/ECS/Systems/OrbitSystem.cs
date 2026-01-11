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
                orbit.currentAngle += orbit.angularSpeed * deltaTime;

                // 获取中心位置
                if (!world.HasComponent<PositionComponent>(orbit.centerEntity))
                {
                    world.DestroyEntity(entity);
                    continue;
                }

                var centerPos = world.GetComponent<PositionComponent>(orbit.centerEntity);
                var position = world.GetComponent<PositionComponent>(entity);

                // 计算轨道位置
                position.x = centerPos.x + Mathf.Cos(orbit.angularSpeed) * orbit.radius;
                position.y = centerPos.y + Mathf.Sin(orbit.angularSpeed) * orbit.radius;
            }
        }
    }
}