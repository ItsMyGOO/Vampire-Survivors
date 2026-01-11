using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    /// <summary>
    /// 敌人生成系统
    /// 职责: 根据波次配置生成敌人
    /// </summary>
    public class EnemySpawnSystem : SystemBase
    {
        private float spawnTimer = 0f;
        private float spawnInterval = 1.0f;
        private int currentWave = 1;

        public override void Update(World world, float deltaTime)
        {
            spawnTimer += deltaTime;

            if (spawnTimer >= spawnInterval)
            {
                int spawnCount = GetSpawnCountForWave(currentWave);

                for (int i = 0; i < spawnCount; i++)
                {
                    SpawnEnemy(world, "Zombie");
                }

                spawnTimer = 0f;
            }
        }

        private int GetSpawnCountForWave(int wave)
        {
            return Mathf.Min(1 + wave / 2, 5); // 最多同时生成5个
        }

        private void SpawnEnemy(World world, string enemyType)
        {
            int enemyId = world.CreateEntity();

            // 在屏幕边缘随机位置生成
            Vector2 spawnPos = GetRandomSpawnPosition();

            world.AddComponent(enemyId, new EnemyTagComponent());

            world.AddComponent(enemyId, new PositionComponent(spawnPos.x, spawnPos.y));
            world.AddComponent(enemyId, new VelocityComponent(){speed = 0.5f});
            world.AddComponent(enemyId, new MoveIntentComponent());

            world.AddComponent(enemyId, new HealthComponent(50, 50));
            world.AddComponent(enemyId, new ColliderComponent(0.5f));

            world.AddComponent(enemyId, new SpriteKeyComponent());
            world.AddComponent(enemyId, new AnimationComponent()
            {
                ClipSetId = "Zombie1",
                DefaultState = "Run"
            });
        }

        private Vector2 GetRandomSpawnPosition()
        {
            int edge = UnityEngine.Random.Range(0, 4);
            float offset = 2.0f;

            switch (edge)
            {
                case 0: // 上
                    return new Vector2(UnityEngine.Random.Range(-10f, 10f), 10f + offset);
                case 1: // 下
                    return new Vector2(UnityEngine.Random.Range(-10f, 10f), -10f - offset);
                case 2: // 左
                    return new Vector2(-10f - offset, UnityEngine.Random.Range(-10f, 10f));
                default: // 右
                    return new Vector2(10f + offset, UnityEngine.Random.Range(-10f, 10f));
            }
        }
    }
}