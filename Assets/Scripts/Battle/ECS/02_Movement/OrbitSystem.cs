using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    /// <summary>
    /// 轨道系统
    /// 职责: 处理围绕中心点旋转的武器（如刀）
    /// </summary>
    public class OrbitSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            foreach (var (entity, orbit) in world.GetComponents<OrbitComponent>())
            {
                if (!world.HasComponent<PositionComponent>(entity)) continue;

                var orb = orbit;
                orb.currentAngle -= orb.angularSpeed * deltaTime;

                if (!world.HasComponent<PositionComponent>(orb.centerEntity))
                {
                    world.SetComponent(entity, orb);
                    world.DestroyEntity(entity);
                    continue;
                }

                var centerPos = world.GetComponent<PositionComponent>(orb.centerEntity);
                var position = world.GetComponent<PositionComponent>(entity);
                position.x = centerPos.x + Mathf.Cos(orb.currentAngle) * orb.radius;
                position.y = centerPos.y + Mathf.Sin(orb.currentAngle) * orb.radius;
                world.SetComponent(entity, position);
                world.SetComponent(entity, orb);

                if (world.HasComponent<RotationComponent>(entity))
                {
                    var rotation = world.GetComponent<RotationComponent>(entity);
                    rotation.angle = orb.currentAngle;
                    world.SetComponent(entity, rotation);
                }
            }
        }
    }
}