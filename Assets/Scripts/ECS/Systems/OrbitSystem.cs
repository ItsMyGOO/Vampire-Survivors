using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    /// <summary>
    /// 轨道系统
    /// 职责: 处理围绕中心点旋转的武器（如刀）
    /// 修复: 同时更新位置和旋转
    /// </summary>
    public class OrbitSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            foreach (var (entity, orbit) in world.GetComponents<OrbitComponent>())
            {
                if (!world.HasComponent<PositionComponent>(entity)) continue;

                // ⭐ 更新旋转角度（弧度）
                orbit.currentAngle -= orbit.angularSpeed * deltaTime;

                // 获取中心位置
                if (!world.HasComponent<PositionComponent>(orbit.centerEntity))
                {
                    world.DestroyEntity(entity);
                    continue;
                }

                var centerPos = world.GetComponent<PositionComponent>(orbit.centerEntity);
                var position = world.GetComponent<PositionComponent>(entity);

                // 计算轨道位置
                position.x = centerPos.x + Mathf.Cos(orbit.currentAngle) * orbit.radius;
                position.y = centerPos.y + Mathf.Sin(orbit.currentAngle) * orbit.radius;

                // ⭐ 修复：更新旋转组件（让刀朝向运动方向）
                if (world.HasComponent<RotationComponent>(entity))
                {
                    var rotation = world.GetComponent<RotationComponent>(entity);
                    // 旋转角度 = 轨道角度（转为度数）
                    // 注意：这里让刀的尖端指向运动方向（切线方向）
                    rotation.angle = orbit.currentAngle ; // +90度让刀尖朝前
                }
            }
        }
    }
}