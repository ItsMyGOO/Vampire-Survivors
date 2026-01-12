using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    /// <summary>
    /// 重力系统
    /// 职责: 处理掉落物体重力
    /// </summary>
    public class GravitySystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            foreach (var (entity, fallingBody) in world.GetComponents<FallingBodyComponent>())
            {
                if (!world.HasComponent<PositionComponent>(entity)) continue;

                var position = world.GetComponent<PositionComponent>(entity);

                // 应用重力
                fallingBody.velocity_y += fallingBody.gravity * deltaTime;
                position.y += fallingBody.velocity_y * deltaTime;

                // 地面碰撞
                if (position.y <= 0)
                {
                    position.y = 0;
                    fallingBody.velocity_y = -fallingBody.velocity_y * fallingBody.bounce;

                    // 速度很小时停止
                    if (Mathf.Abs(fallingBody.velocity_y) < 0.5f)
                    {
                        world.RemoveComponent<FallingBodyComponent>(entity);
                    }
                }
            }
        }
    }
}