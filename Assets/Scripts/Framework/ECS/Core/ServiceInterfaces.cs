namespace ECS.Core
{
    public interface IExpReceiver
    {
        void AddExp(int value);
    }

    /// <summary>
    /// 道具空间索引服务接口。
    /// 由 ItemSpatialIndexSystem 每帧构建，MagnetSystem / PickupSystem 查询使用。
    /// </summary>
    public interface IItemSpatialIndex
    {
        /// <summary>
        /// 查询 (x, y) 半径 radius 范围内的道具实体 ID，写入 results 缓冲区。
        /// 返回实际写入数量（零 GC）。
        /// </summary>
        int QueryItems(float x, float y, float radius, int[] results);
    }

    /// <summary>
    /// 敌人空间索引服务接口。
    /// 由 EnemySpatialIndexSystem 每帧构建，AttackHitSystem 等查询使用。
    /// </summary>
    public interface IEnemySpatialIndex
    {
        /// <summary>
        /// 查询 (x, y) 半径 radius 范围内的敌人实体 ID，写入 results 缓冲区。
        /// 返回实际写入数量（零 GC）。
        /// </summary>
        int QueryEnemies(float x, float y, float radius, int[] results);
    }
}