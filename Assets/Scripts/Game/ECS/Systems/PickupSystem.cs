using ECS.Core;
using UnityEngine;
using System.Collections.Generic;

namespace ECS.Systems
{
    /// <summary>
    /// 拾取系统
    /// 职责: 处理玩家拾取道具逻辑
    /// </summary>
    public class PickupSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            // 找到玩家
            int playerId = FindPlayer(world);
            if (playerId == -1) return;

            if (!world.HasComponent<PositionComponent>(playerId) ||
                !world.HasComponent<PickupRangeComponent>(playerId))
            {
                return;
            }

            var playerPos = world.GetComponent<PositionComponent>(playerId);
            var pickupRange = world.GetComponent<PickupRangeComponent>(playerId);

            var itemsToPickup = new List<int>();

            // 检查所有可拾取物品
            foreach (var (itemId, pickupable) in world.GetComponents<PickupableComponent>())
            {
                if (!world.HasComponent<PositionComponent>(itemId)) continue;

                var itemPos = world.GetComponent<PositionComponent>(itemId);

                // 计算距离
                float dx = itemPos.x - playerPos.x;
                float dy = itemPos.y - playerPos.y;
                float distSqr = dx * dx + dy * dy;

                // 在拾取范围内
                if (distSqr <= pickupRange.radius * pickupRange.radius)
                {
                    itemsToPickup.Add(itemId);
                }
            }

            // 处理拾取
            foreach (var itemId in itemsToPickup)
            {
                ProcessPickup(world, playerId, itemId);
            }
        }

        private void ProcessPickup(World world, int playerId, int itemId)
        {
            var pickupable = world.GetComponent<PickupableComponent>(itemId);

            switch (pickupable.item_type)
            {
                case "exp":
                case "exp_gem":
                    // 添加经验
                    if (world.HasComponent<ExperienceComponent>(playerId))
                    {
                        var exp = world.GetComponent<ExperienceComponent>(playerId);
                        exp.current_exp += pickupable.value * exp.exp_multiplier;
                        
                        Debug.Log($"Picked up {pickupable.value} EXP. Current: {exp.current_exp}/{exp.exp_to_next_level}");
                    }
                    break;

                case "health":
                    // 恢复生命值
                    if (world.HasComponent<HealthComponent>(playerId))
                    {
                        var health = world.GetComponent<HealthComponent>(playerId);
                        health.current = Mathf.Min(health.current + pickupable.value, health.max);
                    }
                    break;

                case "coin":
                    // 添加金币（需要金币系统）
                    Debug.Log($"Picked up {pickupable.value} coins");
                    break;

                case "magnet":
                    // 临时磁铁效果
                    if (!world.HasComponent<MagnetComponent>(playerId))
                    {
                        world.AddComponent(playerId, new MagnetComponent(8.0f, 15.0f));
                    }
                    // 添加持续时间（需要 buff 系统）
                    break;
            }

            // 播放拾取音效（TODO）
            // 播放拾取特效（TODO）

            // 销毁道具
            world.DestroyEntity(itemId);
        }

        private int FindPlayer(World world)
        {
            foreach (var (entity, _) in world.GetComponents<PlayerTagComponent>())
            {
                return entity;
            }
            return -1;
        }
    }
}
