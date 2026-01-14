using Cinemachine;
using UnityEngine;
using ECS.Core;
using UniRx;

namespace Battle
{
    public class ECSGameManager : MonoBehaviour
    {
        public static ECSGameManager Instance { get; private set; }

        private World world;
        private IntReactiveProperty playerId = new(-1);

        public CinemachineVirtualCamera camera;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GameConfigLoader.LoadAll();

            world = ECSWorldFactory.Create(camera);

            ECSSystemInstaller.Install(world);

            int pid = PlayerFactory.CreatePlayer(world);
            playerId.Value = pid;

            UpgradeBootstrap.Initialize(world, pid);

            PlayerWeaponInitializer.Initialize(world, pid);

            gameObject.AddComponent<ECSGameDebugController>()
                .Initialize(world, pid);
        }

        private void Update()
        {
            world?.Update(Time.deltaTime);
            ECSWorldFactory.Render(world);
        }

        private void OnDestroy()
        {
            world?.Clear();
        }

        public World World => world;
        public int PlayerId => playerId.Value;
    }
}