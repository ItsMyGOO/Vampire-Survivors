using System;
using System.Collections.Generic;
using Battle.View.Battle.View;
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

        private List<IDisposable> disposables = new List<IDisposable>();
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GameConfigLoader.LoadAll();

            var renderSystem = new RenderSystem(new SpriteProvider(), new RenderObjectPool());
            renderSystem.OnCameraTargetChanged += new CameraFollowController(vCam).SetTarget;

            world = new World();
            syncSystem = new RenderSyncSystem(renderSystem);

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
            syncSystem.Update(world);
        }

        private void OnDestroy()
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
            
            world?.Clear();
        }

        public World World => world;
        public int PlayerId => playerId.Value;
    }
}