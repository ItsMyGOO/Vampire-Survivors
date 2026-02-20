using ECS.Core;

namespace ECS.Systems
{
    /// <summary>
    /// 敌人空间索引系统
    /// 职责：每帧将场景内所有 EnemyTagComponent 实体插入空间哈希，
    ///       并以 IEnemySpatialIndex 服务暴露给 AttackHitSystem 等查询方。
    /// 执行顺序：必须在 AttackHitSystem 之前。
    /// cellSize = 1.0f（敌人碰撞半径上限约 0.5f，格子大小为直径）
    /// </summary>
    public class EnemySpatialIndexSystem : SystemBase, IEnemySpatialIndex
    {
        private readonly SpatialHashGrid _grid = new SpatialHashGrid(1.0f);

        public override void Update(World world, float deltaTime)
        {
            _grid.Clear();

            world.IterateComponents<ECS.EnemyTagComponent>(out int[] ids, out _, out int count);
            for (int i = 0; i < count; i++)
            {
                int enemyId = ids[i];
                if (!world.HasComponent<ECS.PositionComponent>(enemyId)) continue;

                var pos = world.GetComponent<ECS.PositionComponent>(enemyId);
                _grid.Insert(enemyId, pos.x, pos.y);
            }
        }

        public int QueryEnemies(float x, float y, float radius, int[] results)
            => _grid.QueryNeighbors(x, y, radius, results);
    }
}