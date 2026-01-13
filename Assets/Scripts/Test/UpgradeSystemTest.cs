using UnityEngine;
using XLua;
using ECS.Core;
using ECS;
using System.Collections.Generic;

namespace Game.Battle
{
    /// <summary>
    /// 升级系统测试脚本
    /// 用于测试Lua升级系统和ECS属性应用
    /// </summary>
    public class UpgradeSystemTest : MonoBehaviour
    {
        [Header("测试配置")] [SerializeField] private bool autoTest = false;
        [SerializeField] private int expToAdd = 500;

        private LuaEnv luaEnv;
        private World world;
        private int playerEntity;

        void Start()
        {
            if (autoTest)
            {
                InitializeTest();
            }
        }

        [ContextMenu("Initialize Test")]
        public void InitializeTest()
        {
            Debug.Log("=== 升级系统测试开始 ===");

            // 1. 创建Lua环境
            luaEnv = new LuaEnv();
            luaEnv.AddLoader((ref string filepath) =>
            {
                string fullPath = Application.dataPath + "/Lua/" + filepath.Replace('.', '/') + ".lua";
                if (System.IO.File.Exists(fullPath))
                {
                    return System.IO.File.ReadAllBytes(fullPath);
                }

                return null;
            });

            Debug.Log("✓ LuaEnv 创建成功");

            // 2. 创建ECS World
            world = new World();
            Debug.Log("✓ ECS World 创建成功");

            // 3. 创建玩家实体
            playerEntity = world.CreateEntity();
            world.AddComponent(playerEntity, new PositionComponent(0, 0));
            world.AddComponent(playerEntity, new HealthComponent(100, 100, 0));
            world.AddComponent(playerEntity, new VelocityComponent());
            world.AddComponent(playerEntity, new PlayerAttributeComponent());
            Debug.Log($"✓ 玩家实体创建成功 (Entity: {playerEntity})");

            // 4. 初始化PlayerContext
            PlayerContext.Instance.Initialize(world, playerEntity);
            PlayerContext.Instance.SetPlayerEntity(playerEntity);
            Debug.Log("✓ PlayerContext 初始化成功");

            // 5. 初始化ExpSystem
            ExpSystem.Instance.Init(luaEnv, PlayerContext.Instance);
            ExpSystem.Instance.OnUpgradeOptionsAvailable += OnUpgradeOptionsReceived;
            Debug.Log("✓ ExpSystem 初始化成功");

            Debug.Log("=== 初始化完成 ===\n");

            // 打印初始属性
            PrintPlayerAttributes();
        }

        [ContextMenu("Add Experience (100)")]
        public void AddExp100()
        {
            AddExperience(100);
        }

        [ContextMenu("Add Experience (500)")]
        public void AddExp500()
        {
            AddExperience(500);
        }

        [ContextMenu("Add Experience (1000)")]
        public void AddExp1000()
        {
            AddExperience(1000);
        }

        [ContextMenu("Print Player Attributes")]
        public void PrintPlayerAttributes()
        {
            if (world == null || !world.HasComponent<PlayerAttributeComponent>(playerEntity))
            {
                Debug.LogWarning("玩家属性组件不存在");
                return;
            }

            PlayerAttributeComponent attr = world.GetComponent<PlayerAttributeComponent>(playerEntity);
            HealthComponent health = world.GetComponent<HealthComponent>(playerEntity);

            Debug.Log("=== 玩家当前属性 ===");
            Debug.Log($"等级: {PlayerContext.Instance.Exp.level.Value}");
            Debug.Log(
                $"经验: {PlayerContext.Instance.Exp.current_exp.Value:F1}/{PlayerContext.Instance.Exp.exp_to_next_level.Value:F1}");
            Debug.Log("\n--- 基础属性 ---");
            Debug.Log($"最大生命值: {attr.maxHealth} (当前: {health.current:F1})");
            Debug.Log($"生命回复: {attr.healthRegen}/秒");
            Debug.Log($"移动速度: {attr.moveSpeed}");
            Debug.Log($"护甲: {attr.armor}");
            Debug.Log("\n--- 攻击属性 ---");
            Debug.Log($"攻击力: {attr.attackDamage}");
            Debug.Log($"攻击速度: {attr.attackSpeed}x");
            Debug.Log($"暴击率: {attr.criticalChance * 100}%");
            Debug.Log($"暴击伤害: {attr.criticalDamage}x");
            Debug.Log("\n--- 范围和数量 ---");
            Debug.Log($"范围大小: {attr.areaSize}x");
            Debug.Log($"额外弹道: +{attr.projectileCount}");
            Debug.Log($"穿透次数: {attr.pierceCount}");
            Debug.Log("\n--- 特殊属性 ---");
            Debug.Log($"拾取范围: {attr.pickupRange}");
            Debug.Log($"经验倍率: {attr.expGain}x");
            Debug.Log($"冷却缩减: {attr.cooldownReduction * 100}%");
            Debug.Log("==================\n");
        }

        private void AddExperience(int amount)
        {
            if (ExpSystem.Instance == null)
            {
                Debug.LogError("ExpSystem 未初始化！请先运行 Initialize Test");
                return;
            }

            Debug.Log($"\n>>> 添加 {amount} 经验值");
            ExpSystem.Instance.AddExp(amount);
        }

        private void OnUpgradeOptionsReceived(List<UpgradeOption> options)
        {
            Debug.Log("\n=== 升级选项 ===");
            for (int i = 0; i < options.Count; i++)
            {
                UpgradeOption opt = options[i];
                string typeIcon = opt.type == "weapon" ? "⚔️" : "📊";
                Debug.Log($"{i + 1}. {typeIcon} [{opt.type}] {opt.name}");
                Debug.Log($"   {opt.description}");
                if (opt.type == "passive")
                {
                    Debug.Log($"   ({opt.attributeType}: +{opt.value})");
                }
            }

            Debug.Log("================\n");
        }

        void OnDestroy()
        {
            if (ExpSystem.Instance != null)
            {
                ExpSystem.Instance.OnUpgradeOptionsAvailable -= OnUpgradeOptionsReceived;
            }

            luaEnv?.Dispose();
        }

        void Update()
        {
            // 定期Tick Lua
            luaEnv?.Tick();
        }
    }
}