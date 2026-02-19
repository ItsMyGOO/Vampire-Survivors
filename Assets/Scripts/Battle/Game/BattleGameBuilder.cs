using Battle.Player;
using Battle.Upgrade;
using Battle.View;
using Cinemachine;
using ECS;
using ECS.Core;
using ECS.SyncSystems;
using UI.Model;

namespace Battle
{
    public static class BattleGameBuilder
    {
        public static BattleWorldContext Build(CinemachineVirtualCamera vCam)
        {
            GameConfigLoader.LoadAll();

            var world = new World();

            // Player
            int playerId = PlayerFactory.CreatePlayer(world);

            // Core Systems
            ECSSystemInstaller.Install(world);

            // Upgrade Module
            UpgradeWorldInstaller.Install(world, playerId);

            // Render
            var renderSystem = new RenderSystem(
                new SpriteProvider(),
                new RenderObjectPool()
            );

            renderSystem.OnCameraTargetChanged +=
                new CameraFollowController(vCam).SetTarget;

            var renderSync = new RenderSyncSystem(renderSystem);

            // HUD
            var hudViewModel = new HUDViewModel();
            ViewModelRegistry.Register(hudViewModel);

            world.RegisterSystem(new HUDSyncSystem(playerId, hudViewModel));

            // 构建 PlayerContext 供 Debug 工具使用
            world.TryGetService<ExpSystem>(out var expSystem);
            var playerContext = new PlayerContext(world, playerId, expSystem);

            return new BattleWorldContext(world, renderSync, hudViewModel, playerContext);
        }
    }
}