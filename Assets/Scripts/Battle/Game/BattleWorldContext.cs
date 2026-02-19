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

        public BattleWorldContext(
            World world,
            RenderSyncSystem renderSyncSystem,
            HUDViewModel hudViewModel)
        {
            World = world;
            RenderSyncSystem = renderSyncSystem;
            HUDViewModel = hudViewModel;
        }

        public void Dispose()
        {
            World?.Clear();
        }
    }
}