using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    /// <summary>
    /// 磁铁系统
    /// 职责: 吸引道具向玩家移动
    /// </summary>
    public class MagnetSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            // 找到所有有磁铁效果的实体（通常是玩家）
            foreach (var (magnetEntity, magnet) in world.GetComponents<MagnetComponent>())
            {
                if (!magnet.active) continue;
                if (!world.HasComponent<PositionComponent>(magnetEntity)) continue;

                var magnetPos = world.GetComponent<PositionComponent>(magnetEntity);

                // 吸引范围内的可拾取物品
                foreach (var (itemId, pickupable) in world.GetComponents<PickupableComponent>())
                {
                    if (!pickupable.auto_pickup) continue;
                    if (!world.HasComponent<PositionComponent>(itemId)) continue;

                    var itemPos = world.GetComponent<PositionComponent>(itemId);

                    // 计算距离
                    float dx = magnetPos.x - itemPos.x;
                    float dy = magnetPos.y - itemPos.y;
                    float distSqr = dx * dx + dy * dy;
                    float dist = Mathf.Sqrt(distSqr);

                    // 在磁铁范围内
                    if (dist <= magnet.radius && dist > 0.01f)
                    {
                        // 添加或更新速度组件，使道具向玩家移动
                        if (!world.HasComponent<VelocityComponent>(itemId))
                        {
                            world.AddComponent(itemId, new VelocityComponent());
                        }

                        var velocity = world.GetComponent<VelocityComponent>(itemId);

                        // 计算朝向玩家的方向
                        float dirX = dx / dist;
                        float dirY = dy / dist;

                        // 设置速度（距离越近速度越快）
                        float speedMultiplier = 1.0f + (magnet.radius - dist) / magnet.radius;
                        float speed = magnet.strength * speedMultiplier;

                        velocity.x = dirX * speed;
                        velocity.y = dirY * speed;
                        velocity.speed = 1.0f;
                    }
                }
            }
        }
    }
}
