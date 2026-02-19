using Battle.Player;
using Battle.Upgrade;
using ECS;
using ECS.Core;
using UI.Model;

namespace Battle
{
    public class BattleWorldContext
    {
        public World World { get; }
        public RenderSyncSystem RenderSyncSystem { get; }
        public HUDViewModel HUDViewModel { get; }

        /// <summary>
        /// 仅供 Debug 工具使用，不应在运行时逻辑中传递
        /// </summary>
        public PlayerContext PlayerContext { get; }

        public BattleWorldContext(
            World world,
            RenderSyncSystem renderSyncSystem,
            HUDViewModel hudViewModel,
            PlayerContext playerContext)
        {
            World = world;
            RenderSyncSystem = renderSyncSystem;
            HUDViewModel = hudViewModel;
            PlayerContext = playerContext;
        }

        /// <summary>
        /// 从 World 服务注册表获取升级服务（供 UI 层使用）
        /// </summary>
        public bool TryGetUpgradeService(out UpgradeService upgradeService)
            => World.TryGetService(out upgradeService);

        public bool TryGetUpgradeApplyService(out UpgradeApplyService applyService)
            => World.TryGetService(out applyService);

        public void Dispose()
        {
            World?.Clear();
        }
    }
}