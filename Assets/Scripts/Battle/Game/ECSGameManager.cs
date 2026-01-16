using Battle.Player;
using Battle.Upgrade;
using Battle.View;
using Battle.Weapon;
using Cinemachine;
using ECS;
using UnityEngine;
using ECS.Core;
using UI.Core;
using UI.Panel;

namespace Battle
{
    public class ECSGameManager : MonoBehaviour
    {
        private World world;
        private RenderSyncSystem syncSystem;

        public CinemachineVirtualCamera vCam;

        private void Start()
        {
            Init();

            UIManager.Instance.ShowPanel<BattleHUDPanel>();
        }

        private void Init()
        {
            // 1. 配置
            GameConfigLoader.LoadAll();

            // 2. World
            world = new World();

            // 3. Player
            int playerId = PlayerFactory.CreatePlayer(world);

            // 4. PlayerContext（只存引用，不干活）
            PlayerContext.Initialize(world, playerId);

            // 5. World 系统
            InstallWorldSystems(world);

            // ===== 升级 / 经验 =====
            UpgradeWorldInstaller.Install(world,playerId);

            // 6. Player 运行时数据
            PlayerWeaponInitializer.Initialize(world, playerId);
        }

        private void InstallWorldSystems(World w)
        {
            ECSSystemInstaller.Install(w);

            // ===== 渲染 =====
            var renderSystem = new RenderSystem(
                new SpriteProvider(),
                new RenderObjectPool()
            );

            renderSystem.OnCameraTargetChanged +=
                new CameraFollowController(vCam).SetTarget;

            syncSystem = new RenderSyncSystem(renderSystem);
        }

        private void Update()
        {
            world?.Update(Time.deltaTime);
            syncSystem.Update(world);
        }

        private void OnDestroy()
        {
            world?.Clear();
            PlayerContext.Clear();
        }

        public World World => world;
    }
}