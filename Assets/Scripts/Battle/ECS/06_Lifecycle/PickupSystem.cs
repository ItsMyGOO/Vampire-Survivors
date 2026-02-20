using ECS.Core;
using ECS;
using UnityEngine;
using System.Collections.Generic;

namespace ECS.Systems
{
    /// <summary>
    /// 拾取系统
    /// 职责: 处理玩家拾取道具逻辑
    /// 优化: 通过 IItemSpatialIndex 服务查询玩家附近道具，O(k) 代替 O(N)
    /// </summary>
    public class PickupSystem : SystemBase
    {
        private readonly int[] _neighborBuffer = new int[64];
        private readonly List<int> _itemsToPickup = new List<int>();

        public override void Update(World world, float deltaTime)
        {
            int playerId = FindPlayer(world);
            if (playerId == -1) return;

            if (!world.HasComponent<PositionComponent>(playerId) ||
                !world.HasComponent<PickupRangeComponent>(playerId))
                return;

            if (!world.TryGetService<IItemSpatialIndex>(out var itemIndex))
                return;

            var playerPos = world.GetComponent<PositionComponent>(playerId);
            var pickupRange = world.GetComponent<PickupRangeComponent>(playerId);
            float rangeSq = pickupRange.radius * pickupRange.radius;

            _itemsToPickup.Clear();

            int neighborCount = itemIndex.QueryItems(playerPos.x, playerPos.y, pickupRange.radius, _neighborBuffer);
            for (int i = 0; i < neighborCount; i++)
            {
                int itemId = _neighborBuffer[i];
                if (!world.HasComponent<PositionComponent>(itemId)) continue;

                var itemPos = world.GetComponent<PositionComponent>(itemId);
                float dx = itemPos.x - playerPos.x;
                float dy = itemPos.y - playerPos.y;

                if (dx * dx + dy * dy <= rangeSq)
                    _itemsToPickup.Add(itemId);
            }

            for (int i = 0; i < _itemsToPickup.Count; i++)
                ProcessPickup(world, playerId, _itemsToPickup[i]);
        }

        private void ProcessPickup(World world, int playerId, int itemId)
        {
            if (!world.HasComponent<PickupableComponent>(itemId)) return;
            var pickupable = world.GetComponent<PickupableComponent>(itemId);

            switch (pickupable.item_type)
            {
                case "exp":
                case "exp_gem":
                    if (world.TryGetService<IExpReceiver>(out var exp))
                        exp.AddExp(pickupable.value);
                    break;

                case "health":
                    if (world.HasComponent<HealthComponent>(playerId))
                    {
                        var health = world.GetComponent<HealthComponent>(playerId);
                        health.current = Mathf.Min(health.current + pickupable.value, health.max);
                        world.SetComponent(playerId, health);
                    }
                    break;

                case "coin":
                    Debug.Log($"Picked up {pickupable.value} coins");
                    break;

                case "magnet":
                    if (!world.HasComponent<MagnetComponent>(playerId))
                        world.AddComponent(playerId, new MagnetComponent(8.0f, 15.0f));
                    break;
            }

            world.DestroyEntity(itemId);
        }

        private int FindPlayer(World world)
        {
            foreach (var (entity, _) in world.GetComponents<PlayerTagComponent>())
                return entity;
            return -1;
        }
    }
}