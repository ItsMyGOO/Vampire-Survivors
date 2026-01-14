// ============================================
// 文件: EnemySpawnSystem.cs - 完整版本
// 使用 EnemyConfigDB 加载敌人配置
// ============================================

using ECS.Core;
using UnityEngine;
using ConfigHandler;

namespace ECS.Systems
{
    /// <summary>
    /// 敌人生成系统 - 完整实现
    /// 根据时间和关卡生成不同类型和数量的敌人
    /// </summary>
    public class EnemySpawnSystem : SystemBase
    {
        private float spawnTimer = 0f;
        private float spawnInterval = 1.0f; // 生成间隔（秒）
        private float gameTime = 0f; // 游戏时间（用于难度递增）

        // 生成位置范围
        private const float SPAWN_RANGE = 10f;
        private const float SPAWN_OFFSET = 2f;

        public override void Update(World world, float deltaTime)
        {
            gameTime += deltaTime;
            spawnTimer += deltaTime;

            if (spawnTimer < spawnInterval)
                return;

            spawnTimer = 0f;

            // 根据游戏时间决定难度
            int currentLevel = CalculateCurrentLevel(gameTime);
            int enemyCount = CalculateEnemyCount(gameTime);

            // 生成敌人
            for (int i = 0; i < enemyCount; i++)
            {
                SpawnRandomEnemy(world, currentLevel);
            }
        }

        /// <summary>
        /// 计算当前关卡（难度等级）
        /// </summary>
        private int CalculateCurrentLevel(float time)
        {
            // 每10秒提升一个难度等级
            int level = 1 + Mathf.FloorToInt(time / 10f);

            // 限制最大等级（根据配置文件中的实际关卡数）
            return Mathf.Min(level, 2); // 当前配置只有 level_1 和 level_2
        }

        /// <summary>
        /// 计算每波生成的敌人数量
        /// </summary>
        private int CalculateEnemyCount(float time)
        {
            // 基础数量 + 随时间递增
            int baseCount = 1;
            int timeBonus = Mathf.FloorToInt(time / 10f);

            return Mathf.Min(baseCount + timeBonus, 5); // 最多同时生成5个
        }

        /// <summary>
        /// 生成随机敌人
        /// </summary>
        private void SpawnRandomEnemy(World world, int level)
        {
            // 从配置数据库获取随机敌人定义
            var enemyDef = EnemyConfigDB.Instance.GetRandomEnemy(level);

            if (enemyDef == null)
            {
                Debug.LogWarning($"未找到关卡 {level} 的敌人配置");
                return;
            }

            SpawnEnemy(world, enemyDef);
        }

        /// <summary>
        /// 生成敌人实体
        /// </summary>
        private void SpawnEnemy(World world, EnemyDef enemyDef)
        {
            int enemyId = world.CreateEntity();

            // 敌人标签
            world.AddComponent(enemyId, new EnemyTagComponent());

            // 位置组件（在屏幕边缘随机位置）
            Vector2 spawnPos = GetRandomSpawnPosition();
            world.AddComponent(enemyId, new PositionComponent(spawnPos.x, spawnPos.y));
            // 移动意图组件（AI用）
            world.AddComponent(enemyId, new VelocityComponent { speed = 0.5f });

            // 生命值组件
            world.AddComponent(enemyId, new HealthComponent
            {
                current = enemyDef.Hp,
                max = enemyDef.Hp,
                regen = 0
            });
            // 碰撞体组件
            world.AddComponent(enemyId, new ColliderComponent(0.5f));

            // 动画组件
            world.AddComponent(enemyId, new AnimationComponent
            {
                ClipSetName = enemyDef.ClipSetId,
                DefaultAnim = "Run"
            });
            // 精灵键组件（使用动画系统）
            world.AddComponent(enemyId, new SpriteKeyComponent());

            // Debug.Log($"生成敌人: {enemyDef.ClipSetId} (HP: {enemyDef.Hp}) at ({spawnPos.x:F2}, {spawnPos.y:F2})");
        }

        /// <summary>
        /// 获取随机生成位置（在屏幕边缘）
        /// </summary>
        private Vector2 GetRandomSpawnPosition()
        {
            int edge = Random.Range(0, 4);
            float offset = SPAWN_OFFSET;

            switch (edge)
            {
                case 0: // 上边缘
                    return new Vector2(
                        Random.Range(-SPAWN_RANGE, SPAWN_RANGE),
                        SPAWN_RANGE + offset
                    );

                case 1: // 下边缘
                    return new Vector2(
                        Random.Range(-SPAWN_RANGE, SPAWN_RANGE),
                        -SPAWN_RANGE - offset
                    );

                case 2: // 左边缘
                    return new Vector2(
                        -SPAWN_RANGE - offset,
                        Random.Range(-SPAWN_RANGE, SPAWN_RANGE)
                    );

                default: // 右边缘
                    return new Vector2(
                        SPAWN_RANGE + offset,
                        Random.Range(-SPAWN_RANGE, SPAWN_RANGE)
                    );
            }
        }
    }
}