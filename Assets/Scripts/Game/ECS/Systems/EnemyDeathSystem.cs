using System.Collections.Generic;
using ConfigHandler;
using ECS.Core;

namespace ECS.Systems
{
    /// <summary>
    /// 敌人死亡系统
    /// 职责: 处理敌人死亡逻辑（掉落经验、特效）
    /// </summary>
    public class EnemyDeathSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            var deadEnemies = new List<int>();

            foreach (var (entity, _) in world.GetComponents<EnemyTagComponent>())
            {
                if (!world.HasComponent<HealthComponent>(entity)) continue;

                var health = world.GetComponent<HealthComponent>(entity);

                if (health.current <= 0)
                {
                    deadEnemies.Add(entity);
                }
            }

            // 处理死亡敌人
            foreach (var enemyId in deadEnemies)
            {
                // 掉落经验宝石
                SpawnExpGem(world, enemyId);

                // 播放死亡特效（TODO）

                // 销毁实体
                world.DestroyEntity(enemyId);
            }
        }

        private void SpawnExpGem(World world, int enemyId)
        {
            if (!world.HasComponent<PositionComponent>(enemyId)) return;

            var enemyPos = world.GetComponent<PositionComponent>(enemyId);

            int gemId = world.CreateEntity();

            // 位置
            world.AddComponent(gemId, new PositionComponent(enemyPos.x, enemyPos.y));

            var prop = DropItemConfigDB.Instance.Get("exp_small");
            // 可拾取组件（使用新的系统）
            world.AddComponent(gemId, new PickupableComponent("exp", prop.exp, true));
            // 精灵
            world.AddComponent(gemId, new SpriteKeyComponent()
            {
                sheet = prop.sheet,
                key = prop.key
            });
        }
    }
}