using ConfigHandler;
using ECS;
using ECS.Core;
using ECS.Systems;
using Game.Battle;
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

            renderSystem = new RenderSyncSystem(new RenderSystem(spriteProvider, new RenderObjectPool()));
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

                // 方式1: 使用默认文件名（推荐）
                var animDb = AnimationConfigDB.Load();
                if (animDb != null)
                {
                    AnimationConfigDB.Initialize(animDb);
                }
                else
                {
                    Debug.LogError("✗ 动画配置加载失败");
                }

                // 方式2: 显式指定文件名（更清晰）
                var enemyDb = EnemyConfigDB.Load();
                if (enemyDb != null)
                {
                    EnemyConfigDB.Initialize(enemyDb);
                }
                else
                {
                    Debug.LogError("✗ 敌人配置加载失败");
                }

                // 方式3: 自定义文件名（特殊需求）
                var weaponDb = WeaponConfigDB.Load();
                if (weaponDb != null)
                {
                    WeaponConfigDB.Initialize(weaponDb);
                }
                else
                {
                    Debug.LogError("✗ 武器配置加载失败");
                }

                // 加载掉落物配置
                var dropItemDb = DropItemConfigDB.Load();
                if (dropItemDb != null)
                {
                    DropItemConfigDB.Initialize(dropItemDb);
                }
                else
                {
                    Debug.LogError("✗ 掉落物配置加载失败");
                }

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
                world.RegisterSystem(new ExperienceSystem());

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

            world.AddComponent(playerId, new PickupRangeComponent(1.0f));
            world.AddComponent(playerId, new ExperienceComponent(1, 100f));
            world.AddComponent(playerId, new MagnetComponent(5.0f, 10.0f));

            world.AddComponent(playerId, new SpriteKeyComponent());
            world.AddComponent(playerId, new AnimationComponent()
            {
                ClipSetName = "Player",
                DefaultAnim = "Idle"
            });

            world.AddComponent(playerId, new WeaponSlotsComponent()
            {
                weapons =
                {
                    new WeaponSlotsComponent.WeaponData("ProjectileKnife", 1, 1.0f),
                    new WeaponSlotsComponent.WeaponData("OrbitKnife", 1, 1.0f)
                }
            });

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