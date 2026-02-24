using ECS.Core;

namespace ECS.Systems
{
    /// <summary>
    /// 敌人空间索引系统
    /// 职责：每帧将场景内所有敌人实体插入空间哈希，
    ///       并以 IEnemySpatialIndex 服务暴露给 AttackHitSystem 等查询方。
    /// 执行顺序：必须在 AttackHitSystem 之前。
    /// cellSize = 1.0f（敌人碰撞半径上限约 0.5f，格子大小为直径）
    ///
    /// 优化：直接遍历 PositionComponent 裸数组，位置数据连续读取，
    ///       再用 HasComponent<EnemyTagComponent> 过滤，共 1N 次字典查找。
    ///       原实现遍历 EnemyTagComponent 后再 HasComponent + GetComponent<Position>，为 2N 次。
    /// </summary>
    public class EnemySpatialIndexSystem : SystemBase, IEnemySpatialIndex
    {
        private readonly SpatialHashGrid _grid = new SpatialHashGrid(1.0f);

        public override void Update(World world, float deltaTime)
        {
            _grid.Clear();

            world.IterateComponents(out int[] ids, out PositionComponent[] positions, out int count);
            for (int i = 0; i < count; i++)
            {
                if (!world.HasComponent<EnemyTagComponent>(ids[i])) continue;
                _grid.Insert(ids[i], positions[i].x, positions[i].y);
            }
        }

        public int QueryEnemies(float x, float y, float radius, int[] results)
            => _grid.QueryNeighbors(x, y, radius, results);
    }
}