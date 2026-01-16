using Battle.Upgrade;
using ECS.Core;

namespace Battle.Player
{
    /// <summary>
    /// 玩家上下文 - 单例
    /// 存储玩家相关的全局数据
    /// </summary>
    public class PlayerContext
    {
        public static PlayerContext Instance { get; private set; }

        public World World { get; private set; }
        public int PlayerEntity { get; private set; }
        public ExpSystem ExpSystem { get; set; }

        public static void Initialize(World world, int playerEntity)
        {
            Instance = new PlayerContext
            {
                World = world,
                PlayerEntity = playerEntity,
            };
        }

        /// <summary>
        /// 清理所有数据
        /// 在退出战斗时调用
        /// </summary>
        public static void Clear()
        {
            UnityEngine.Debug.Log("[PlayerContext] 清理数据");
            
            // 清空单例
            Instance = null;
        }
    }
}