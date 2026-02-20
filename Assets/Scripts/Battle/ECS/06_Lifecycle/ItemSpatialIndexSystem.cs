using ECS.Core;

namespace ECS.Systems
{
    /// <summary>
    /// 道具空间索引系统
    /// 职责：每帧将场景内所有 PickupableComponent 实体插入空间哈希，
    ///       并以 IItemSpatialIndex 服务暴露给 MagnetSystem / PickupSystem。
    /// 执行顺序：必须在 MagnetSystem 和 PickupSystem 之前。
    /// </summary>
    public class ItemSpatialIndexSystem : SystemBase, IItemSpatialIndex
    {
        // cellSize = 0.5f（道具体积小，密度可能较高，小格子减少冗余候选）
        private readonly SpatialHashGrid _grid = new SpatialHashGrid(0.5f);

        public override void Update(World world, float deltaTime)
        {
            _grid.Clear();

            world.IterateComponents<ECS.PickupableComponent>(out int[] ids, out _, out int count);
            for (int i = 0; i < count; i++)
            {
                int itemId = ids[i];
                if (!world.HasComponent<ECS.PositionComponent>(itemId)) continue;

                var pos = world.GetComponent<ECS.PositionComponent>(itemId);
                _grid.Insert(itemId, pos.x, pos.y);
            }
        }

        public int QueryItems(float x, float y, float radius, int[] results)
            => _grid.QueryNeighbors(x, y, radius, results);
    }
}