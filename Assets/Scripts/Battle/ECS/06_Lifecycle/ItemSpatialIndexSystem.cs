using ECS.Core;

namespace ECS.Systems
{
    /// <summary>
    /// 道具空间索引系统
    /// 职责：每帧将场景内所有可拾取道具插入空间哈希，
    ///       并以 IItemSpatialIndex 服务暴露给 MagnetSystem / PickupSystem。
    /// 执行顺序：必须在 MagnetSystem 和 PickupSystem 之前。
    /// cellSize = 0.5f（道具体积小，密度可能较高，小格子减少冗余候选）
    ///
    /// 优化：直接遍历 PositionComponent 裸数组，位置数据连续读取，
    ///       再用 HasComponent<PickupableComponent> 过滤，共 1N 次字典查找。
    ///       原实现遍历 PickupableComponent 后再 HasComponent + GetComponent<Position>，为 2N 次。
    /// </summary>
    public class ItemSpatialIndexSystem : SystemBase, IItemSpatialIndex
    {
        private readonly SpatialHashGrid _grid = new SpatialHashGrid(0.5f);

        public override void Update(World world, float deltaTime)
        {
            _grid.Clear();

            world.IterateComponents<PositionComponent>(out int[] ids, out PositionComponent[] positions, out int count);
            for (int i = 0; i < count; i++)
            {
                if (!world.HasComponent<PickupableComponent>(ids[i])) continue;
                _grid.Insert(ids[i], positions[i].x, positions[i].y);
            }
        }

        public int QueryItems(float x, float y, float radius, int[] results)
            => _grid.QueryNeighbors(x, y, radius, results);
    }
}