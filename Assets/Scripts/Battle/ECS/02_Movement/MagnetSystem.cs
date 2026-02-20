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
            foreach (var (magnetEntity, magnet) in world.GetComponents<MagnetComponent>())
            {
                if (!magnet.active) continue;
                if (!world.HasComponent<PositionComponent>(magnetEntity)) continue;

                var magnetPos = world.GetComponent<PositionComponent>(magnetEntity);

                foreach (var (itemId, pickupable) in world.GetComponents<PickupableComponent>())
                {
                    if (!pickupable.auto_pickup) continue;
                    if (!world.HasComponent<PositionComponent>(itemId)) continue;

                    var itemPos = world.GetComponent<PositionComponent>(itemId);

                    float dx = magnetPos.x - itemPos.x;
                    float dy = magnetPos.y - itemPos.y;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    if (dist <= magnet.radius && dist > 0.01f)
                    {
                        if (!world.HasComponent<VelocityComponent>(itemId))
                            world.AddComponent(itemId, new VelocityComponent());

                        var velocity = world.GetComponent<VelocityComponent>(itemId);
                        float dirX = dx / dist;
                        float dirY = dy / dist;
                        float speedMultiplier = 1.0f + (magnet.radius - dist) / magnet.radius;
                        float speed = magnet.strength * speedMultiplier;
                        velocity.x = dirX * speed;
                        velocity.y = dirY * speed;
                        velocity.speed = 1.0f;
                        world.SetComponent(itemId, velocity);
                    }
                }
            }
        }
    }
}