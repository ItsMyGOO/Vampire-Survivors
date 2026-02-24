using ConfigHandler;
using ECS;
using ECS.Core;
using UnityEngine;

namespace Battle.Game
{
    /// <summary>
    /// 预览敌人生成工具（静态工具类）
    /// 职责：在给定 World 中批量生成携带 WanderComponent 的敌人实体，供 PreviewBattleMode 使用。
    /// 不触发 EnemySpawnSystem，不添加 SpawnStateComponent，与正式战斗逻辑完全解耦。
    /// </summary>
    public static class PreviewEnemySpawner
    {
        private const int   DEFAULT_COUNT         = 8;
        private const float DEFAULT_SPAWN_RANGE   = 6f;
        private const float DEFAULT_WANDER_RADIUS = 2f;
        private const float DEFAULT_WANDER_SPEED  = 0.8f;
        private const float DEFAULT_WAIT_DURATION = 1.0f;
        private const float DEFAULT_HP            = 10f;

        /// <summary>
        /// 在给定 World 中生成预览敌人实体。
        /// </summary>
        public static void Spawn(
            World world,
            int   count        = DEFAULT_COUNT,
            float spawnRange   = DEFAULT_SPAWN_RANGE,
            float wanderRadius = DEFAULT_WANDER_RADIUS,
            float wanderSpeed  = DEFAULT_WANDER_SPEED)
        {
            // 尝试从配置读取一个敌人定义，获取 ClipSetId；失败则 fallback
            EnemyDef enemyDef = TryGetFallbackEnemyDef();

            for (int i = 0; i < count; i++)
            {
                float spawnX = Random.Range(-spawnRange, spawnRange);
                float spawnY = Random.Range(-spawnRange, spawnRange);

                int entityId = world.CreateEntity();

                world.AddComponent(entityId, new EnemyTagComponent());
                world.AddComponent(entityId, new PositionComponent(spawnX, spawnY));
                world.AddComponent(entityId, new VelocityComponent { speed = wanderSpeed });
                world.AddComponent(entityId, new HealthComponent
                {
                    current = DEFAULT_HP,
                    max     = DEFAULT_HP,
                    regen   = 0f
                });
                world.AddComponent(entityId, new WanderComponent
                {
                    originX      = spawnX,
                    originY      = spawnY,
                    radius       = wanderRadius,
                    speed        = wanderSpeed,
                    targetX      = spawnX,
                    targetY      = spawnY,
                    waitTimer    = 0f,
                    waitDuration = DEFAULT_WAIT_DURATION
                });
                world.AddComponent(entityId, new AnimationComponent
                {
                    ClipSetName  = enemyDef != null ? enemyDef.ClipSetId : string.Empty,
                    DefaultAnim  = "Run"
                });
                world.AddComponent(entityId, new SpriteKeyComponent());
            }

            Debug.Log($"[PreviewEnemySpawner] 已生成 {count} 个预览敌人");
        }

        /// <summary>
        /// 尝试从 EnemyConfigDB 读取 level_1 的第一个敌人定义作为预览用途。
        /// 若配置未加载或不存在则返回 null。
        /// </summary>
        private static EnemyDef TryGetFallbackEnemyDef()
        {
            if (EnemyConfigDB.Instance == null)
                return null;

            return EnemyConfigDB.Instance.GetRandomEnemy(1);
        }
    }
}
