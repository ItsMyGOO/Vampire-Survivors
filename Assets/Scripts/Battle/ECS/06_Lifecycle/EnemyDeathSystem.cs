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
        private readonly List<int> _deadEnemies = new List<int>();

        public override void Update(World world, float deltaTime)
        {
            _deadEnemies.Clear();

            foreach (var (entity, _) in world.GetComponents<EnemyTagComponent>())
            {
                if (!world.HasComponent<HealthComponent>(entity)) continue;

                var health = world.GetComponent<HealthComponent>(entity);
                if (health.current <= 0)
                    _deadEnemies.Add(entity);
            }

            for (int i = 0; i < _deadEnemies.Count; i++)
            {
                int enemyId = _deadEnemies[i];
                SpawnExpGem(world, enemyId);
                world.DestroyEntity(enemyId);
            }
        }

        private void SpawnExpGem(World world, int enemyId)
        {
            if (!world.HasComponent<PositionComponent>(enemyId)) return;

            var enemyPos = world.GetComponent<PositionComponent>(enemyId);

            int gemId = world.CreateEntity();

            world.AddComponent(gemId, new PositionComponent(enemyPos.x, enemyPos.y));

            var prop = DropItemConfigDB.Instance.Get("exp_small");
            world.AddComponent(gemId, new PickupableComponent("exp", prop.exp));
            world.AddComponent(gemId, new SpriteKeyComponent
            {
                sheet = prop.sheet,
                key = prop.key
            });
        }
    }
}