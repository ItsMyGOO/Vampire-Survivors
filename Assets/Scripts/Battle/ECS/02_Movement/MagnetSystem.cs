using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    /// <summary>
    /// 磁铁系统
    /// 职责: 吸引道具向玩家移动
    /// 优化: 通过 IItemSpatialIndex 服务查询范围内道具，O(M×k) 代替 O(M×N)
    /// </summary>
    public class MagnetSystem : SystemBase
    {
        // 邻居缓冲区：预分配，QueryItems 零 GC
        private readonly int[] _neighborBuffer = new int[256];

        public override void Update(World world, float deltaTime)
        {
            if (!world.TryGetService<IItemSpatialIndex>(out var itemIndex))
                return;

            foreach (var (magnetEntity, magnet) in world.GetComponents<ECS.MagnetComponent>())
            {
                if (!magnet.active) continue;
                if (!world.HasComponent<ECS.PositionComponent>(magnetEntity)) continue;

                var magnetPos = world.GetComponent<ECS.PositionComponent>(magnetEntity);
                int neighborCount = itemIndex.QueryItems(magnetPos.x, magnetPos.y, magnet.radius, _neighborBuffer);

                for (int i = 0; i < neighborCount; i++)
                {
                    int itemId = _neighborBuffer[i];

                    if (!world.HasComponent<ECS.PickupableComponent>(itemId)) continue;
                    var pickupable = world.GetComponent<ECS.PickupableComponent>(itemId);
                    if (!pickupable.auto_pickup) continue;

                    if (!world.HasComponent<ECS.PositionComponent>(itemId)) continue;
                    var itemPos = world.GetComponent<ECS.PositionComponent>(itemId);

                    float dx = magnetPos.x - itemPos.x;
                    float dy = magnetPos.y - itemPos.y;
                    float distSq = dx * dx + dy * dy;
                    if (distSq <= 0.0001f) continue;

                    float dist = Mathf.Sqrt(distSq);
                    if (!world.HasComponent<ECS.VelocityComponent>(itemId))
                        world.AddComponent(itemId, new ECS.VelocityComponent());

                    var velocity = world.GetComponent<ECS.VelocityComponent>(itemId);
                    float speedMultiplier = 1.0f + (magnet.radius - dist) / magnet.radius;
                    float speed = magnet.strength * speedMultiplier;
                    velocity.x = (dx / dist) * speed;
                    velocity.y = (dy / dist) * speed;
                    velocity.speed = 1.0f;
                    world.SetComponent(itemId, velocity);
                }
            }
        }
    }
}