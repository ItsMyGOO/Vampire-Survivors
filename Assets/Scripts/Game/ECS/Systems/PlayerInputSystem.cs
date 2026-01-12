using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    /// <summary>
    /// 玩家输入系统
    /// 职责: 读取玩家输入，设置 MoveIntent 组件
    /// </summary>
    public class PlayerInputSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            // 找到玩家实体
            int playerId = FindPlayer(world);
            var velocity = world.GetComponent<VelocityComponent>(playerId);

            // 读取 WASD 输入
            velocity.x = Input.GetAxisRaw("Horizontal") * velocity.speed;
            velocity.y = Input.GetAxisRaw("Vertical") * velocity.speed;
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