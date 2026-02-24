using ConfigHandler;
using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    /// <summary>
    /// 敌人生成系统
    /// 职责：根据时间和关卡生成不同类型和数量的敌人。
    ///
    /// 运行时状态（gameTime、spawnTimer）存储在 SpawnStateComponent 中，
    /// 由 BattleGameBuilder 挂载到全局 SpawnController 实体，符合 ECS 架构规范。
    /// System 本身无可变字段。
    /// </summary>
    public class EnemySpawnSystem : SystemBase
    {
        private const float SPAWN_RANGE  = 10f;
        private const float SPAWN_OFFSET = 2f;

        public override void Update(World world, float deltaTime)
        {
            // 从全局 SpawnController 实体读取状态
            world.IterateComponents(
                out int[] ids, out SpawnStateComponent[] states, out int count);

            if (count == 0) return;

            // SpawnController 只有一个实体
            ref SpawnStateComponent state = ref states[0];
            int controllerId = ids[0];

            state.gameTime   += deltaTime;
            state.spawnTimer += deltaTime;

            if (state.spawnTimer < state.spawnInterval)
            {
                world.SetComponent(controllerId, state);
                return;
            }

            state.spawnTimer = 0f;
            world.SetComponent(controllerId, state);

            int currentLevel = CalculateCurrentLevel(state.gameTime);
            int enemyCount   = CalculateEnemyCount(state.gameTime);

            for (int i = 0; i < enemyCount; i++)
                SpawnRandomEnemy(world, currentLevel);
        }

        private static int CalculateCurrentLevel(float time)
        {
            int level = 1 + Mathf.FloorToInt(time / 10f);
            return Mathf.Min(level, 2); // 当前配置只有 level_1 和 level_2
        }

        private static int CalculateEnemyCount(float time)
        {
            int timeBonus = Mathf.FloorToInt(time / 10f);
            return Mathf.Min(1 + timeBonus, 5);
        }

        private static void SpawnRandomEnemy(World world, int level)
        {
            var enemyDef = EnemyConfigDB.Instance.GetRandomEnemy(level);
            if (enemyDef == null)
            {
                Debug.LogWarning($"[EnemySpawnSystem] 未找到关卡 {level} 的敌人配置");
                return;
            }
            SpawnEnemy(world, enemyDef);
        }

        private static void SpawnEnemy(World world, EnemyDef enemyDef)
        {
            int enemyId = world.CreateEntity();

            Vector2 spawnPos = GetRandomSpawnPosition();

            world.AddComponent(enemyId, new EnemyTagComponent());
            world.AddComponent(enemyId, new PositionComponent(spawnPos.x, spawnPos.y));
            world.AddComponent(enemyId, new VelocityComponent { speed = 0.5f });
            world.AddComponent(enemyId, new HealthComponent
            {
                current = enemyDef.Hp,
                max     = enemyDef.Hp,
                regen   = 0
            });
            world.AddComponent(enemyId, new SeparationComponent(radius: 2.0f, strength: 1.5f));
            world.AddComponent(enemyId, new ColliderComponent(0.5f));
            world.AddComponent(enemyId, new AnimationComponent
            {
                ClipSetName = enemyDef.ClipSetId,
                DefaultAnim = "Run"
            });
            world.AddComponent(enemyId, new SpriteKeyComponent());
        }

        private static Vector2 GetRandomSpawnPosition()
        {
            int edge = Random.Range(0, 4);
            switch (edge)
            {
                case 0: return new Vector2(Random.Range(-SPAWN_RANGE, SPAWN_RANGE),  SPAWN_RANGE + SPAWN_OFFSET);
                case 1: return new Vector2(Random.Range(-SPAWN_RANGE, SPAWN_RANGE), -SPAWN_RANGE - SPAWN_OFFSET);
                case 2: return new Vector2(-SPAWN_RANGE - SPAWN_OFFSET, Random.Range(-SPAWN_RANGE, SPAWN_RANGE));
                default: return new Vector2( SPAWN_RANGE + SPAWN_OFFSET, Random.Range(-SPAWN_RANGE, SPAWN_RANGE));
            }
        }
    }
}