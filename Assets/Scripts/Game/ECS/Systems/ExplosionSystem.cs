using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    /// <summary>
    /// 爆炸系统
    /// 职责: 处理 AOE 爆炸效果
    /// </summary>
    public class ExplosionSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            foreach (var (entity, explosion) in world.GetComponents<AoeExplosionComponent>())
            {
                if (explosion.triggered) continue;

                if (!world.HasComponent<PositionComponent>(entity)) continue;

                var explosionPos = world.GetComponent<PositionComponent>(entity);

                // 对范围内所有敌人造成伤害
                foreach (var (enemyId, _) in world.GetComponents<EnemyTagComponent>())
                {
                    if (!world.HasComponent<PositionComponent>(enemyId)) continue;

                    var enemyPos = world.GetComponent<PositionComponent>(enemyId);
                    float dx = enemyPos.x - explosionPos.x;
                    float dy = enemyPos.y - explosionPos.y;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    if (dist <= explosion.radius)
                    {
                        if (world.HasComponent<HealthComponent>(enemyId))
                        {
                            var health = world.GetComponent<HealthComponent>(enemyId);
                            health.current -= explosion.damage;
                        }
                    }
                }

                explosion.triggered = true;
                world.DestroyEntity(entity);
            }
        }
    }
}