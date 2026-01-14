using Cinemachine;
using ConfigHandler;
using ECS;
using ECS.Core;
using ECS.Systems;
using Game.Battle;
using Lua;
using UniRx;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// ECS 游戏管理器 - Unity MonoBehaviour 驱动
    /// </summary>
    public class ECSGameManager : MonoBehaviour
    {
        private World world;
        private IntReactiveProperty playerIdProperty = new(-1);
        private RenderSyncSystem renderSystem;

        [Header("调试")] public bool showDebugInfo = true;
        public KeyCode debugKey = KeyCode.F1;

        [Header("渲染")] public Sprite fallbackSprite;

        public CinemachineVirtualCamera cinemachine;

        private void Awake()
        {
            world = new World();
            Debug.Log("ECS World 已创建");

            var spriteProvider = new SpriteProvider();

            if (fallbackSprite != null)
            {
                spriteProvider.SetFallbackSprite(fallbackSprite);
                Debug.Log("已设置渲染占位符精灵");
            }
            else
            {
                Debug.LogWarning("未设置 fallbackSprite，加载失败的精灵将显示为空");
            }

            renderSystem = new RenderSyncSystem(
                new RenderSystem(spriteProvider, new RenderObjectPool(), cinemachine));
        }

        private void Start()
        {
            playerIdProperty
                .DistinctUntilChanged()
                .Subscribe(Bind)
                .AddTo(this);

            LoadConfigurations();
            InitializeSystems();
            InitializeGame();

            Debug.Log($"游戏初始化完成 - 实体数: {world.EntityCount}, 系统数: {world.SystemCount}");
        }

        void Bind(int playerId)
        {
            PlayerContext.Instance.Initialize(world, playerId);

            var upgradeService = new UpgradeService(
                WeaponUpgradePoolConfigDB.Instance,
                WeaponUpgradeRuleConfigDB.Instance,
                PassiveUpgradePoolConfigDB.Instance
            );
            ExpSystem.Instance.Init(LuaMain.Env, PlayerContext.Instance, upgradeService);
            world.RegisterService(ExpSystem.Instance);
        }

        private void Update()
        {
            try
            {
                world.Update(Time.deltaTime);

                renderSystem.Update(world);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"游戏更新出错: {e.Message}\n{e.StackTrace}");
            }

            HandleDebugInput();
        }

        private void OnDestroy()
        {
            ExpSystem.Instance.Dispose();
            world?.Clear();
        }

        /// <summary>
        /// 加载所有配置
        /// </summary>
        private void LoadConfigurations()
        {
            try
            {
                Debug.Log("========== 开始加载配置 ==========");

                ConfigLoader.Load(AnimationConfigDB.Load, AnimationConfigDB.Initialize, "动画配置");
                ConfigLoader.Load(EnemyConfigDB.Load, EnemyConfigDB.Initialize, "敌人配置");
                ConfigLoader.Load(WeaponConfigDB.Load, WeaponConfigDB.Initialize, "武器配置");
                ConfigLoader.Load(DropItemConfigDB.Load, DropItemConfigDB.Initialize, "掉落物配置");

                Debug.Log("========== 配置加载完成 ==========");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"配置加载失败: {e.Message}\n{e.StackTrace}");
            }
        }

        private void InitializeSystems()
        {
            try
            {
                world.RegisterSystem(new PlayerInputSystem());
                world.RegisterSystem(new MagnetSystem());
                world.RegisterSystem(new PickupSystem());

                world.RegisterSystem(new EnemySpawnSystem());

                world.RegisterSystem(new AIMovementSystem());
                world.RegisterSystem(new MovementSystem());

                world.RegisterSystem(new WeaponFireSystem());
                world.RegisterSystem(new OrbitSystem());

                world.RegisterSystem(new AttackHitSystem());
                world.RegisterSystem(new KnockBackSystem());
                world.RegisterSystem(new EnemyDeathSystem());

                world.RegisterSystem(new PlayerAnimationSystem());
                world.RegisterSystem(new AnimationCommandSystem());
                world.RegisterSystem(new AnimationSystem());

                Debug.Log("所有系统已注册");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"系统注册失败: {e.Message}\n{e.StackTrace}");
            }
        }

        private void InitializeGame()
        {
            try
            {
                CreatePlayer();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"游戏初始化失败: {e.Message}\n{e.StackTrace}");
            }
        }

        private void CreatePlayer()
        {
            var playerId = world.CreateEntity();
            world.AddComponent(playerId, new PlayerTagComponent());

            world.AddComponent(playerId, new PositionComponent());
            world.AddComponent(playerId, new VelocityComponent() { speed = 2 });

            world.AddComponent(playerId, new HealthComponent(100, 100, 1.0f));
            world.AddComponent(playerId, new ColliderComponent(0.5f));


            world.AddComponent(playerId, new WeaponSlotsComponent()
            {
                weapons =
                {
                    new WeaponSlotsComponent.WeaponData("ProjectileKnife", 1, 1.0f),
                    new WeaponSlotsComponent.WeaponData("OrbitKnife", 1, 1.0f)
                }
            });

            world.AddComponent(playerId, new PickupRangeComponent(1.0f));
            world.AddComponent(playerId, new MagnetComponent(5.0f, 10.0f));

            world.AddComponent(playerId, new SpriteKeyComponent());
            world.AddComponent(playerId, new AnimationComponent()
            {
                ClipSetName = "Player",
                DefaultAnim = "Idle"
            });
            world.AddComponent(playerId, new CameraFollowComponent());

            playerIdProperty.Value = playerId;
            Debug.Log($"玩家已创建 - 实体ID: {playerId}");
        }

        private void HandleDebugInput()
        {
            if (showDebugInfo && Input.GetKeyDown(debugKey))
            {
                world.DebugPrint();
            }
        }

        public World GetWorld() => world;
        public int GetPlayerId() => playerIdProperty.Value;

        [ContextMenu("重新加载配置")]
        public void ReloadConfigurations()
        {
            Debug.Log("热重载配置...");
            LoadConfigurations();
            Debug.Log("配置已重新加载");
        }
    }
}