using Battle.Player;
using Battle.Upgrade;
using Battle.View;
using Battle.Weapon;
using Cinemachine;
using ECS;
using UnityEngine;
using ECS.Core;
using UniRx;

namespace Battle
{
    public class ECSGameManager : MonoBehaviour
    {
        public static ECSGameManager Instance { get; private set; }

        private World world;
        private RenderSyncSystem syncSystem;
        private IntReactiveProperty playerId = new(-1);

        public CinemachineVirtualCamera vCam;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GameConfigLoader.LoadAll();

            world = new World();
            int pid = PlayerFactory.CreatePlayer(world);

            var renderSystem = new RenderSystem(new SpriteProvider(), new RenderObjectPool());
            renderSystem.OnCameraTargetChanged += new CameraFollowController(vCam).SetTarget;
            
            syncSystem = new RenderSyncSystem(renderSystem);

            ECSSystemInstaller.Install(world);

            
            playerId.Value = pid;

            UpgradeBootstrap.Initialize(world, pid);

            PlayerWeaponInitializer.Initialize(world, pid);

            gameObject.AddComponent<ECSGameDebugController>()
                .Initialize(world, pid);
        }

        private void Update()
        {
            world?.Update(Time.deltaTime);
            syncSystem.Update(world);
        }

        private void OnDestroy()
        {
            world?.Clear();
        }

        public World World => world;
        public int PlayerId => playerId.Value;
    }
}