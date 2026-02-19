using Battle.Player;
using Battle.Upgrade;
using Battle.View;
using Cinemachine;
using ECS;
using UnityEngine;
using ECS.Core;
using ECS.SyncSystems;
using UI.Core;
using UI.Model;
using UI.Panel;

namespace Battle
{
    public class ECSGameManager : MonoBehaviour
    {
        private World world;
        private RenderSyncSystem syncSystem;

        private HUDViewModel hudViewModel;
        private HUDSyncSystem hudSyncSystem;

        [SerializeField] private CinemachineVirtualCamera vCam;

        private void Start()
        {
            InitializeGame();
            UIManager.Instance.ShowPanel<BattleHUDPanel>();
        }

        private void InitializeGame()
        {
            LoadConfig();
            CreateWorld();
            CreatePlayer();
            InstallSystems();
            InstallUpgrade();
            InstallHUD();
        }

        #region Init Steps

        private void LoadConfig()
        {
            GameConfigLoader.LoadAll();
        }

        private void CreateWorld()
        {
            world = new World();
        }

        private void CreatePlayer()
        {
            int playerId = PlayerFactory.CreatePlayer(world);
            PlayerContext.Initialize(world, playerId);
        }

        private void InstallSystems()
        {
            ECSSystemInstaller.Install(world);

            var renderSystem = new RenderSystem(
                new SpriteProvider(),
                new RenderObjectPool()
            );

            renderSystem.OnCameraTargetChanged +=
                new CameraFollowController(vCam).SetTarget;

            syncSystem = new RenderSyncSystem(renderSystem);
        }

        private void InstallUpgrade()
        {
            UpgradeWorldInstaller.Install(world, PlayerContext.Instance.PlayerEntity);
        }

        private void InstallHUD()
        {
            hudViewModel = new HUDViewModel();
            ViewModelRegistry.Register(hudViewModel);

            hudSyncSystem = new HUDSyncSystem(PlayerContext.Instance.PlayerEntity, hudViewModel);
            world.RegisterSystem(hudSyncSystem);
        }

        #endregion

        private void Update()
        {
            if (world == null) return;

            world.Update(Time.deltaTime);
            syncSystem?.Update(world);
        }

        private void OnDestroy()
        {
            DisposeHUD();
            DisposeWorld();
        }

        #region Dispose

        private void DisposeHUD()
        {
            if (hudSyncSystem != null && world != null)
                world.UnregisterSystem(hudSyncSystem);

            ViewModelRegistry.Unregister<HUDViewModel>();

            hudSyncSystem = null;
            hudViewModel = null;
        }

        private void DisposeWorld()
        {
            world?.Clear();
            world = null;

            PlayerContext.Clear();
        }

        #endregion

        public World World => world;
    }
}
