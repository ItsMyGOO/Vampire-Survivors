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
    public class ECSGameManager : MonoBehaviour
    {
        private World world;
        private IntReactiveProperty playerIdProperty = new(-1);
        private RenderSyncSystem renderSystem;
        private WeaponUpgradeManager weaponUpgradeManager;

        [Header("调试")] 
        public bool showDebugInfo = true;
        public KeyCode debugKey = KeyCode.F1;
        
        [Header("测试")]
        public KeyCode addExpKey = KeyCode.E;
        public float testExpAmount = 20f;

        [Header("渲染")] 
        public Sprite fallbackSprite;
        public CinemachineVirtualCamera cinemachine;

        private void Awake()
        {
            world = new World();
            Debug.Log("ECS World created");

            var spriteProvider = new SpriteProvider();
            if (fallbackSprite != null)
            {
                spriteProvider.SetFallbackSprite(fallbackSprite);
            }

            renderSystem = new RenderSyncSystem(
                new RenderSystem(spriteProvider, new RenderObjectPool(), cinemachine));
        }

        private void Start()
        {
            playerIdProperty.DistinctUntilChanged().Subscribe(Bind).AddTo(this);
            LoadConfigurations();
            InitializeSystems();
            InitializeGame();
            Debug.Log("Game initialized");
        }

        void Bind(int playerId)
        {
            PlayerContext.Instance.Initialize(world, playerId);

            weaponUpgradeManager = new WeaponUpgradeManager(
                WeaponUpgradeRuleConfigDB.Instance,
                WeaponConfigDB.Instance
            );
            PlayerContext.Instance.WeaponUpgradeManager = weaponUpgradeManager;

            var upgradeService = new UpgradeService(
                WeaponUpgradePoolConfigDB.Instance,
                WeaponUpgradeRuleConfigDB.Instance,
                PassiveUpgradePoolConfigDB.Instance
            );
            
            UpgradeApplyService.Initialize(weaponUpgradeManager);
            ExpSystem.Instance.Init(LuaMain.Env, PlayerContext.Instance, upgradeService);
            ExpSystem.Instance.testMode = true;
            world.RegisterService(ExpSystem.Instance);
            
            InitializePlayerWeapons(playerId);
            
            Debug.Log("Weapon upgrade system initialized");
            Debug.Log("Press E to add exp, S to print status");
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
                Debug.LogError("Update error: " + e.Message);
            }

            HandleDebugInput();
            HandleTestInput();
        }

        private void OnDestroy()
        {
            ExpSystem.Instance.Dispose();
            world?.Clear();
        }

        private void LoadConfigurations()
        {
            try
            {
                ConfigLoader.Load(AnimationConfigDB.Load, AnimationConfigDB.Initialize, "Animation");
                ConfigLoader.Load(EnemyConfigDB.Load, EnemyConfigDB.Initialize, "Enemy");
                ConfigLoader.Load(WeaponConfigDB.Load, WeaponConfigDB.Initialize, "Weapon");
                ConfigLoader.Load(DropItemConfigDB.Load, DropItemConfigDB.Initialize, "DropItem");
                ConfigLoader.Load(WeaponUpgradeRuleConfigDB.Load, WeaponUpgradeRuleConfigDB.Initialize, "WeaponUpgradeRule");
                ConfigLoader.Load(WeaponUpgradePoolConfigDB.Load, WeaponUpgradePoolConfigDB.Initialize, "WeaponUpgradePool");
                ConfigLoader.Load(PassiveUpgradePoolConfigDB.Load, PassiveUpgradePoolConfigDB.Initialize, "PassiveUpgradePool");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Config load error: " + e.Message);
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
            }
            catch (System.Exception e)
            {
                Debug.LogError("System init error: " + e.Message);
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
                Debug.LogError("Game init error: " + e.Message);
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
            Debug.Log("Player created");
        }

        private void InitializePlayerWeapons(int playerId)
        {
            if (!world.TryGetComponent<WeaponSlotsComponent>(playerId, out var weaponSlots))
                return;

            var runtimeStats = new WeaponRuntimeStatsComponent();
            
            foreach (var weaponData in weaponSlots.weapons)
            {
                var weaponId = weaponData.weapon_type;
                
                if (!WeaponConfigDB.Instance.Data.TryGetValue(weaponId, out var weaponConfig))
                    continue;

                var stats = runtimeStats.GetOrCreateStats(weaponId);
                var battle = weaponConfig.battle;

                stats.level = weaponData.level;
                stats.damage = battle.baseStats.damage;
                stats.projectileCount = battle.baseStats.count;
                stats.knockback = battle.baseStats.knockback;

                if (battle.Type == WeaponType.Projectile)
                {
                    stats.fireRate = battle.projectile.interval;
                    stats.projectileSpeed = battle.projectile.speed;
                    stats.projectileRange = battle.projectile.range;
                }
                else if (battle.Type == WeaponType.Orbit)
                {
                    stats.orbitRadius = battle.orbit.radius;
                    stats.orbitSpeed = battle.orbit.speed;
                    stats.orbitCount = battle.baseStats.count;
                }
                
                PlayerContext.Instance.UpgradeState.weapons[weaponId] = weaponData.level;
            }

            world.AddComponent(playerId, runtimeStats);
            Debug.Log("Player weapons initialized");
        }

        private void HandleDebugInput()
        {
            if (showDebugInfo && Input.GetKeyDown(debugKey))
            {
                world.DebugPrint();
            }
        }

        private void HandleTestInput()
        {
            if (Input.GetKeyDown(addExpKey))
            {
                ExpSystem.Instance.AddExpForTest(testExpAmount);
            }
            
            if (Input.GetKeyDown(KeyCode.S))
            {
                ExpSystem.Instance.PrintStatus();
                PrintWeaponStats();
            }
        }

        public World GetWorld() => world;
        public int GetPlayerId() => playerIdProperty.Value;

        [ContextMenu("Reload Configs")]
        public void ReloadConfigurations()
        {
            LoadConfigurations();
        }
        
        [ContextMenu("Test Weapon Upgrade")]
        public void TestWeaponUpgrade()
        {
            if (weaponUpgradeManager == null) return;
            int playerId = playerIdProperty.Value;
            if (playerId < 0) return;
            weaponUpgradeManager.UpgradeWeapon(world, playerId, "ProjectileKnife");
            PrintWeaponStats();
        }
        
        [ContextMenu("Add 100 Exp")]
        public void AddTestExp()
        {
            ExpSystem.Instance.AddExpForTest(100f);
        }
        
        [ContextMenu("Print Status")]
        public void PrintPlayerStatus()
        {
            ExpSystem.Instance.PrintStatus();
            PrintWeaponStats();
        }
        
        private void PrintWeaponStats()
        {
            int playerId = playerIdProperty.Value;
            if (playerId < 0) return;

            if (!world.TryGetComponent<WeaponRuntimeStatsComponent>(playerId, out var runtimeStats))
                return;

            Debug.Log("===== Weapon Stats =====");
            foreach (var kvp in runtimeStats.weaponStats)
            {
                var stats = kvp.Value;
                Debug.Log(string.Format("Weapon: {0} Lv.{1}", kvp.Key, stats.level));
                Debug.Log(string.Format("  Damage: {0:F1}", stats.GetFinalDamage()));
                Debug.Log(string.Format("  Count: {0}", stats.GetFinalProjectileCount()));
                Debug.Log(string.Format("  FireRate: {0:F2}s", stats.GetFinalFireRate()));
            }
            Debug.Log("========================");
        }
    }
}
