using ECS;
using ECS.Core;
using ECS.Systems;
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
            InitializeSystems();
            InitializeGame();

            Debug.Log($"游戏初始化完成 - 实体数: {world.EntityCount}, 系统数: {world.SystemCount}");
        }

        private void Update()
        {
            // 更新 ECS World
            world.Update(Time.deltaTime);

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

            // 移动相关
            world.RegisterSystem(new MovementSystem());
            world.RegisterSystem(new AIMovementSystem());

            // 武器和战斗
            world.RegisterSystem(new WeaponFireSystem());
            world.RegisterSystem(new ProjectileMoveSystem());
            world.RegisterSystem(new OrbitSystem());
            world.RegisterSystem(new AttackHitSystem());

            // 敌人
            world.RegisterSystem(new EnemySpawnSystem());
            world.RegisterSystem(new EnemyDeathSystem());

            // 特效
            world.RegisterSystem(new ExplosionSystem());
            world.RegisterSystem(new KnockBackSystem());
            world.RegisterSystem(new GravitySystem());

            // 动画
            world.RegisterSystem(new PlayerAnimationSystem());
            world.RegisterSystem(new AnimationSystem());
            world.RegisterSystem(new AnimationCommandSystem());

            // 生命周期
            world.RegisterSystem(new LifeTimeSystem());

            Debug.Log("所有系统已注册");
        }

        /// <summary>
        /// 初始化游戏（创建玩家等）
        /// </summary>
        private void InitializeGame()
        {
            CreatePlayer();
            CreateInitialKnife();
        }

        /// <summary>
        /// 创建玩家
        /// </summary>
        private void CreatePlayer()
        {
            playerId = world.CreateEntity();

            world.AddComponent(playerId, new PositionComponent(0, 0));
            world.AddComponent(playerId, new VelocityComponent(0, 0));
            world.AddComponent(playerId, new PlayerTagComponent());
            world.AddComponent(playerId, new HealthComponent(100, 100, 1.0f));
            world.AddComponent(playerId, new ColliderComponent(0.5f));
            world.AddComponent(playerId, new SpriteKeyComponent("player_idle"));
            world.AddComponent(playerId, new AnimationComponent("idle", 1.0f, true));

            // 武器槽
            var weaponSlots = new WeaponSlotsComponent();
            weaponSlots.weapons.Add(new WeaponSlotsComponent.WeaponData("knife", 1, 1.0f));
            world.AddComponent(playerId, weaponSlots);

            Debug.Log($"玩家已创建 - 实体ID: {playerId}");
        }

        /// <summary>
        /// 创建初始刀（轨道武器）
        /// </summary>
        private void CreateInitialKnife()
        {
            int knifeId = world.CreateEntity();

            world.AddComponent(knifeId, new PositionComponent(0, 0));
            world.AddComponent(knifeId, new OrbitComponent(playerId, 2.0f, 3.0f, 0f));
            world.AddComponent(knifeId, new DamageSourceComponent(20f, playerId));
            world.AddComponent(knifeId, new ColliderComponent(0.5f));
            world.AddComponent(knifeId, new SpriteKeyComponent("knife"));

            Debug.Log($"初始刀已创建 - 实体ID: {knifeId}");
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