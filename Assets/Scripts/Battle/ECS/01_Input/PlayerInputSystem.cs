using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    /// <summary>
    /// 玩家输入系统
    /// 职责: 读取玩家输入，更新 VelocityComponent
    /// </summary>
    public class PlayerInputSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            int playerId = FindPlayer(world);
            if (playerId == -1) return;
            if (!world.HasComponent<VelocityComponent>(playerId)) return;

            var velocity = world.GetComponent<VelocityComponent>(playerId);
            velocity.x = Input.GetAxisRaw("Horizontal") * velocity.speed;
            velocity.y = Input.GetAxisRaw("Vertical") * velocity.speed;
            world.SetComponent(playerId, velocity);
        }

        private int FindPlayer(World world)
        {
            foreach (var (entity, _) in world.GetComponents<PlayerTagComponent>())
                return entity;
            return -1;
        }
    }
}