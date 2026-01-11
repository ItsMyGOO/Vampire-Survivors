using ConfigHandler;
using ECS;
using ECS.Core;
using ECS.Systems;
using Lua;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// ECS 游戏管理器 - Unity MonoBehaviour 驱动
    /// 将 ECS World 集成到 Unity 生命周期
    /// </summary>
    public class ECSGameManager : MonoBehaviour
    {
        private World world;
        private int playerId = -1;

        RenderSystem RenderSystem = new RenderSystem(new SpriteProvider(), new RenderObjectPool());

        [Header("调试")] public bool showDebugInfo = true;
        public KeyCode debugKey = KeyCode.F1;

        private void Awake()
        {
            // 创建 World
            world = new World();

            Debug.Log("ECS World 已创建");
        }

        private void Start()
        {
            var animDb = AnimationConfigLoader.LoadAll(LuaMain.Env);
            AnimationConfigDB.Initialize(animDb);

            InitializeSystems();
            InitializeGame();

            Debug.Log($"游戏初始化完成 - 实体数: {world.EntityCount}, 系统数: {world.SystemCount}");
        }

        private void Update()
        {
            // 更新 ECS World
            world.Update(Time.deltaTime);

            var list = Render.Collect(world);
            RenderSystem.Render(list);
            // 调试信息
            if (showDebugInfo && Input.GetKeyDown(debugKey))
            {
                world.DebugPrint();
            }
        }

        private void OnDestroy()
        {
            world?.Clear();
        }

        /// <summary>
        /// 初始化所有系统
        /// </summary>
        private void InitializeSystems()
        {
            // 输入系统
            world.RegisterSystem(new PlayerInputSystem());
            // 敌人生成
            world.RegisterSystem(new EnemySpawnSystem());
            // 移动相关
            world.RegisterSystem(new AIMovementSystem());
            world.RegisterSystem(new MovementSystem());

            // 武器和战斗
            world.RegisterSystem(new WeaponFireSystem());
            world.RegisterSystem(new ProjectileMoveSystem());
            world.RegisterSystem(new OrbitSystem());

            world.RegisterSystem(new AttackHitSystem());
            world.RegisterSystem(new KnockBackSystem());
            world.RegisterSystem(new EnemyDeathSystem());

            // 特效
            // world.RegisterSystem(new ExplosionSystem());
            // world.RegisterSystem(new GravitySystem());

            // 动画
            world.RegisterSystem(new PlayerAnimationSystem());
            world.RegisterSystem(new AnimationCommandSystem());
            world.RegisterSystem(new AnimationSystem());

            // 生命周期
            // world.RegisterSystem(new LifeTimeSystem());

            Debug.Log("所有系统已注册");
        }

        /// <summary>
        /// 初始化游戏（创建玩家等）
        /// </summary>
        private void InitializeGame()
        {
            CreatePlayer();
        }

        /// <summary>
        /// 创建玩家
        /// </summary>
        private void CreatePlayer()
        {
            playerId = world.CreateEntity();

            world.AddComponent(playerId, new PositionComponent());
            world.AddComponent(playerId, new VelocityComponent() { speed = 2 });
            world.AddComponent(playerId, new PlayerTagComponent());
            world.AddComponent(playerId, new HealthComponent(100, 100, 1.0f));
            world.AddComponent(playerId, new ColliderComponent(0.5f));
            world.AddComponent(playerId, new SpriteKeyComponent());
            world.AddComponent(playerId, new AnimationComponent()
            {
                ClipSetId = "Player",
                DefaultState = "Idle"
            });

            // 武器槽
            var weaponSlots = new WeaponSlotsComponent();
            weaponSlots.weapons.Add(new WeaponSlotsComponent.WeaponData("knife", 1, 1.0f));
            world.AddComponent(playerId, weaponSlots);

            Debug.Log($"玩家已创建 - 实体ID: {playerId}");
        }

        /// <summary>
        /// 获取 World（供其他系统访问）
        /// </summary>
        public World GetWorld()
        {
            return world;
        }

        /// <summary>
        /// 获取玩家ID
        /// </summary>
        public int GetPlayerId()
        {
            return playerId;
        }
    }
}