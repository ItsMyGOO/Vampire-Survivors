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
            if (playerId == -1) return;

            if (!world.HasComponent<MoveIntentComponent>(playerId))
            {
                world.AddComponent(playerId, new MoveIntentComponent());
            }

            var moveIntent = world.GetComponent<MoveIntentComponent>(playerId);

            // 读取 WASD 输入
            moveIntent.target_x = Input.GetAxis("Horizontal");
            moveIntent.target_y = Input.GetAxis("Vertical");

            // 从 PlayerStats 获取速度（如果有的话）
            moveIntent.speed = 5.0f; // 默认速度
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