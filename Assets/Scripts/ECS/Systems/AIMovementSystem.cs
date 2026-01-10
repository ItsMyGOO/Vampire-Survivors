using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    /// <summary>
    /// AI 移动系统
    /// 职责: 控制敌人追踪玩家
    /// </summary>
    public class AIMovementSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            // 找到玩家
            int playerId = FindPlayer(world);
            if (playerId == -1) return;

            if (!world.HasComponent<PositionComponent>(playerId)) return;

            var playerPos = world.GetComponent<PositionComponent>(playerId);

            // 更新所有敌人的移动意图
            foreach (var (entity, _) in world.GetComponents<EnemyTagComponent>())
            {
                if (!world.HasComponent<PositionComponent>(entity) ||
                    !world.HasComponent<MoveIntentComponent>(entity))
                {
                    continue;
                }

                var enemyPos = world.GetComponent<PositionComponent>(entity);
                var moveIntent = world.GetComponent<MoveIntentComponent>(entity);

                // 计算朝向玩家的方向
                float dx = playerPos.x - enemyPos.x;
                float dy = playerPos.y - enemyPos.y;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                if (dist > 0.1f)
                {
                    moveIntent.target_x = dx / dist;
                    moveIntent.target_y = dy / dist;
                }

                // 更新速度
                if (world.HasComponent<VelocityComponent>(entity))
                {
                    var velocity = world.GetComponent<VelocityComponent>(entity);
                    velocity.x = moveIntent.target_x * moveIntent.speed;
                    velocity.y = moveIntent.target_y * moveIntent.speed;
                }
            }
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