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
            int playerId = FindPlayer(world);
            if (playerId == -1) return;
            if (!world.HasComponent<PositionComponent>(playerId)) return;

            var playerPos = world.GetComponent<PositionComponent>(playerId);

            foreach (var (entity, _) in world.GetComponents<EnemyTagComponent>())
            {
                if (!world.HasComponent<PositionComponent>(entity) ||
                    !world.HasComponent<VelocityComponent>(entity))
                    continue;

                var enemyPos = world.GetComponent<PositionComponent>(entity);
                var vel = world.GetComponent<VelocityComponent>(entity);

                float dx = playerPos.x - enemyPos.x;
                float dy = playerPos.y - enemyPos.y;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                if (dist > 0.1f)
                {
                    vel.x = (dx / dist) * vel.speed;
                    vel.y = (dy / dist) * vel.speed;
                }
                else
                {
                    vel.x = 0f;
                    vel.y = 0f;
                }

                world.SetComponent(entity, vel);
            }
        }

        private int FindPlayer(World world)
        {
            foreach (var (entity, _) in world.GetComponents<PlayerTagComponent>())
                return entity;
            return -1;
        }
    }
}