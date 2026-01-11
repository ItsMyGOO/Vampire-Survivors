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
    /// 改进版本：完整的错误处理和调试功能
    /// </summary>
    public class ECSGameManager : MonoBehaviour
    {
        private World world;
        private int playerId = -1;
        private RenderSystem renderSystem;

        [Header("调试")] public bool showDebugInfo = true;
        public KeyCode debugKey = KeyCode.F1;
        public KeyCode renderStatsKey = KeyCode.F2; // ⭐ 新增：查看渲染统计

        [Header("渲染")] public Sprite fallbackSprite; // ⭐ 占位符精灵（可在 Inspector 中设置）

        private void Awake()
        {
            // 创建 World
            world = new World();
            Debug.Log("ECS World 已创建");

            // 创建渲染系统
            var spriteProvider = new SpriteProvider();

            // ⭐ 设置占位符精灵
            if (fallbackSprite != null)
            {
                spriteProvider.SetFallbackSprite(fallbackSprite);
                Debug.Log("已设置渲染占位符精灵");
            }
            else
            {
                Debug.LogWarning("未设置 fallbackSprite，加载失败的精灵将显示为空");
            }

            renderSystem = new RenderSystem(spriteProvider, new RenderObjectPool());
        }

        private void Start()
        {
            // ⭐ 带错误处理的配置加载
            LoadConfigurations();

            // 初始化系统和游戏
            InitializeSystems();
            InitializeGame();

            Debug.Log($"游戏初始化完成 - 实体数: {world.EntityCount}, 系统数: {world.SystemCount}");
        }

        private void Update()
        {
            try
            {
                // 更新 ECS World
                world.Update(Time.deltaTime);

                // 渲染
                var renderItems = Render.Collect(world);
                renderSystem.Render(renderItems);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"游戏更新出错: {e.Message}\n{e.StackTrace}");
            }

            // 调试功能
            HandleDebugInput();
        }

        private void OnDestroy()
        {
            world?.Clear();
            renderSystem?.Clear();
        }

        /// <summary>
        /// ⭐ 加载所有配置 - 带错误处理
        /// </summary>
        private void LoadConfigurations()
        {
            try
            {
                Debug.Log("开始加载配置...");

                // 加载动画配置
                var animDb = AnimationConfigLoader.LoadAll(LuaMain.Env);
                AnimationConfigDB.Initialize(animDb);
                Debug.Log("✓ 动画配置加载完成");

                // 加载敌人配置
                var enemyDb = EnemyConfigLoader.LoadAll(LuaMain.Env);
                EnemyConfigDB.Initialize(enemyDb);
                Debug.Log("✓ 敌人配置加载完成");

                // 加载武器配置
                var weaponDb = WeaponConfigLoader.LoadAll(LuaMain.Env);
                WeaponConfigDB.Initialize(weaponDb);
                Debug.Log("✓ 武器配置加载完成");

                Debug.Log("所有配置加载成功");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"配置加载失败: {e.Message}\n{e.StackTrace}");
                Debug.LogError("游戏可能无法正常运行，请检查 Lua 配置文件");
            }
        }

        /// <summary>
        /// 初始化所有系统
        /// </summary>
        private void InitializeSystems()
        {
            try
            {
                // 输入系统
                world.RegisterSystem(new PlayerInputSystem());

                // 敌人生成
                world.RegisterSystem(new EnemySpawnSystem());
                world.RegisterSystem(new WeaponFireSystem());
                // 移动相关
                world.RegisterSystem(new AIMovementSystem());
                world.RegisterSystem(new MovementSystem());
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
            catch (System.Exception e)
            {
                Debug.LogError($"系统注册失败: {e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// 初始化游戏（创建玩家等）
        /// </summary>
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
            weaponSlots.weapons.Add(new WeaponSlotsComponent.WeaponData(
                "ProjectileKnife", 1, 1.0f));
            weaponSlots.weapons.Add(new WeaponSlotsComponent.WeaponData(
                "OrbitKnife", 1, 1.0f));
            world.AddComponent(playerId, weaponSlots);

            Debug.Log($"玩家已创建 - 实体ID: {playerId}");
        }

        /// <summary>
        /// ⭐ 处理调试输入
        /// </summary>
        private void HandleDebugInput()
        {
            // F1: ECS World 调试信息
            if (showDebugInfo && Input.GetKeyDown(debugKey))
            {
                world.DebugPrint();
            }

            // F2: 渲染系统统计
            if (showDebugInfo && Input.GetKeyDown(renderStatsKey))
            {
                renderSystem.PrintStats();
            }
        }

        // ============================================
        // 公共接口
        // ============================================

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

        /// <summary>
        /// ⭐ 热重载配置（用于开发调试）
        /// </summary>
        [ContextMenu("重新加载配置")]
        public void ReloadConfigurations()
        {
            Debug.Log("热重载配置...");
            LoadConfigurations();
            Debug.Log("配置已重新加载");
        }
    }
}