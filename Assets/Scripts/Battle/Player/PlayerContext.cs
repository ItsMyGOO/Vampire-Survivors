using Battle.Upgrade;
using ECS.Core;

namespace Battle.Player
{
    /// <summary>
    /// 玩家战斗上下文 - 轻量数据容器
    /// 仅用于 Debug 工具等需要便捷访问战斗状态的场合
    /// 不应在运行时逻辑中作为全局状态使用
    /// </summary>
    public class PlayerContext
    {
        public World World { get; }
        public int PlayerEntity { get; }
        public ExpSystem ExpSystem { get; }

        public PlayerContext(World world, int playerEntity, ExpSystem expSystem)
        {
            World = world;
            PlayerEntity = playerEntity;
            ExpSystem = expSystem;
        }
    }
}