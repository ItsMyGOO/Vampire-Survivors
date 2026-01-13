using System;
using ECS.Core;
using UnityEngine;
using XLua;
using System.Collections.Generic;
using ECS;

namespace Game.Battle
{
    /// <summary>
    /// 经验和升级系统
    /// 职责: 
    /// 1. 处理经验累积
    /// 2. 检测升级条件
    /// 3. 触发升级并调用 Lua 处理升级逻辑
    /// </summary>
    public class ExpSystem : IExpReceiver, IDisposable
    {
        private static ExpSystem instance;

        public static ExpSystem Instance => instance ??= new ExpSystem();

        // 升级经验曲线配置
        private const float BASE_EXP = 10;
        private const float EXP_GROWTH_RATE = 1.15f;

        // Lua 升级系统
        private LuaEnv luaEnv;
        private LuaTable luaUpgradeSystem;
        private Func<LuaTable, LuaTable> rollOptionsFunc;
        private Action<string, LuaTable> applyUpgradeFunc;

        private Exp exp;
        private World world;
        private int playerEntity;

        // 升级回调事件
        public event Action<List<UpgradeOption>> OnUpgradeOptionsAvailable;

        public void Init(LuaEnv luaEnv, PlayerContext playerContext)
        {
            this.luaEnv = luaEnv;
            exp = playerContext.Exp;
            world = playerContext.World;
            playerEntity = playerContext.PlayerEntity;

            InitializeLuaUpgradeSystem();
        }

        private void InitializeLuaUpgradeSystem()
        {
            try
            {
                // 加载Lua升级系统
                object[] ret = luaEnv.DoString(@"
                    local UpgradeSystem = require('system.upgrade_system')
                    UpgradeSystem:Init()
                    return UpgradeSystem
                ", "ExpSystem_InitUpgrade");

                luaUpgradeSystem = ret[0] as LuaTable;

                if (luaUpgradeSystem == null)
                {
                    Debug.LogError("[ExpSystem] Failed to load Lua UpgradeSystem");
                    return;
                }

                // 获取Lua函数引用
                rollOptionsFunc = luaUpgradeSystem.Get<Func<LuaTable, LuaTable>>("RollOptions");
                applyUpgradeFunc = luaUpgradeSystem.Get<Action<string, LuaTable>>("ApplyUpgrade");

                Debug.Log("[ExpSystem] Lua UpgradeSystem initialized successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"[ExpSystem] Error initializing Lua UpgradeSystem: {e.Message}\n{e.StackTrace}");
            }
        }

        public void AddExp(int value)
        {
            exp.current_exp.Value += value * exp.exp_multiplier;
            while (exp.current_exp.Value >= exp.exp_to_next_level.Value)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            // 减去升级所需经验
            exp.current_exp.Value -= exp.exp_to_next_level.Value;

            // 升级
            exp.level.Value++;

            // 计算下一级所需经验
            exp.exp_to_next_level.Value = CalculateExpForLevel(exp.level.Value + 1);

            Debug.Log($"[ExpSystem] Level Up! New Level: {exp.level.Value}");

            // 处理升级选项
            ProcessUpgrade();
        }

        private void ProcessUpgrade()
        {
            if (luaUpgradeSystem == null)
            {
                Debug.LogError("[ExpSystem] Lua UpgradeSystem not initialized");
                return;
            }

            try
            {
                // 调用Lua生成升级选项
                LuaTable optionsTable = rollOptionsFunc(luaUpgradeSystem);

                if (optionsTable == null)
                {
                    Debug.LogError("[ExpSystem] Failed to generate upgrade options from Lua");
                    return;
                }

                // 解析Lua返回的选项
                List<UpgradeOption> options = ParseUpgradeOptions(optionsTable);

                if (options == null || options.Count == 0)
                {
                    Debug.LogError("[ExpSystem] No upgrade options available");
                    return;
                }

                // 随机选择一个选项（这里可以改为UI选择）
                int selectedIndex = UnityEngine.Random.Range(0, options.Count);
                UpgradeOption selectedOption = options[selectedIndex];

                Debug.Log($"[ExpSystem] Auto-selected upgrade: {selectedOption.name}");

                // 触发升级选项可用事件（供UI使用）
                OnUpgradeOptionsAvailable?.Invoke(options);

                // 应用选中的升级
                ApplyUpgrade(selectedOption.id);
            }
            catch (Exception e)
            {
                Debug.LogError($"[ExpSystem] Error processing upgrade: {e.Message}\n{e.StackTrace}");
            }
        }

        private List<UpgradeOption> ParseUpgradeOptions(LuaTable optionsTable)
        {
            if (optionsTable == null) return null;

            List<UpgradeOption> options = new List<UpgradeOption>();

            try
            {
                // Lua数组索引从1开始
                for (int i = 1; i <= 3; i++)
                {
                    LuaTable optionTable = optionsTable.Get<int, LuaTable>(i);
                    if (optionTable == null) continue;

                    string id = optionTable.Get<string>("id");
                    string name = optionTable.Get<string>("name");
                    string description = optionTable.Get<string>("description");
                    string iconKey = optionTable.Get<string>("icon");
                    string type = optionTable.Get<string>("type");
                    string attributeType = optionTable.Get<string>("attributeType");
                    float value = optionTable.Get<float>("value");

                    UpgradeOption option = new UpgradeOption
                    {
                        id = id,
                        name = name,
                        description = description,
                        iconKey = iconKey,
                        type = type,
                        attributeType = attributeType,
                        value = value
                    };

                    options.Add(option);

                    Debug.Log($"[ExpSystem] Option {i}: [{option.type}] {option.name} - {option.description}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[ExpSystem] Error parsing upgrade options: {e.Message}");
            }

            return options;
        }

        /// <summary>
        /// 应用选中的升级
        /// </summary>
        public void ApplyUpgrade(string upgradeId)
        {
            if (luaUpgradeSystem == null)
            {
                Debug.LogError("[ExpSystem] Lua UpgradeSystem not initialized");
                return;
            }

            try
            {
                // 调用Lua应用升级
                var applyFunc = luaUpgradeSystem.Get<Func<LuaTable, string, LuaTable>>("ApplyUpgrade");
                LuaTable result = applyFunc(luaUpgradeSystem, upgradeId);

                if (result == null)
                {
                    Debug.LogError($"[ExpSystem] Failed to apply upgrade: {upgradeId}");
                    return;
                }

                bool success = result.Get<bool>("success");
                if (!success)
                {
                    Debug.LogError($"[ExpSystem] Upgrade application failed: {upgradeId}");
                    return;
                }

                string upgradeType = result.Get<string>("type");

                // 根据类型应用到ECS系统
                if (upgradeType == "passive")
                {
                    ApplyPassiveUpgrade(result);
                }
                else if (upgradeType == "weapon")
                {
                    ApplyWeaponUpgrade(result);
                }

                Debug.Log($"[ExpSystem] Successfully applied upgrade: {upgradeId} ({upgradeType})");
            }
            catch (Exception e)
            {
                Debug.LogError($"[ExpSystem] Error applying upgrade: {e.Message}\n{e.StackTrace}");
            }
        }

        private void ApplyPassiveUpgrade(LuaTable result)
        {
            string attributeType = result.Get<string>("attributeType");
            float value = result.Get<float>("value");

            // 获取玩家属性组件
            if (!world.HasComponent<PlayerAttributeComponent>(playerEntity))
            {
                world.AddComponent(playerEntity, new PlayerAttributeComponent());
            }

            PlayerAttributeComponent attributes = world.GetComponent<PlayerAttributeComponent>(playerEntity);

            // 解析属性类型并应用
            if (Enum.TryParse<AttributeType>(attributeType, out AttributeType attrType))
            {
                AttributeModifier modifier = new AttributeModifier(attrType, value);
                attributes.ApplyModifier(modifier);

                Debug.Log($"[ExpSystem] Applied passive: {attributeType} +{value}");

                // 如果是生命值相关，更新HealthComponent
                if (attrType == AttributeType.MaxHealth || attrType == AttributeType.HealthRegen)
                {
                    UpdateHealthComponent(attributes);
                }
            }
        }

        private void ApplyWeaponUpgrade(LuaTable result)
        {
            LuaTable weaponData = result.Get<LuaTable>("data");
            if (weaponData == null)
            {
                Debug.LogError("[ExpSystem] Weapon data is null");
                return;
            }

            string weaponId = weaponData.Get<string>("id");
            string weaponName = weaponData.Get<string>("name");

            // TODO: 在这里添加武器到玩家
            Debug.Log($"[ExpSystem] Added weapon: {weaponName} ({weaponId})");

            // 这里可以触发武器生成逻辑
            // 例如：WeaponSystem.Instance.AddWeapon(playerEntity, weaponId);
        }

        private void UpdateHealthComponent(PlayerAttributeComponent attributes)
        {
            if (!world.HasComponent<HealthComponent>(playerEntity))
            {
                return;
            }

            HealthComponent health = world.GetComponent<HealthComponent>(playerEntity);
            health.max = attributes.maxHealth;
            health.regen = attributes.healthRegen;

            // 确保当前生命值不超过最大值
            if (health.current > health.max)
            {
                health.current = health.max;
            }
        }

        private float CalculateExpForLevel(int level)
        {
            return BASE_EXP * Mathf.Pow(EXP_GROWTH_RATE, level - 1);
        }

        public static float GetTotalExpForLevel(int targetLevel)
        {
            float total = 0f;
            for (int i = 1; i <= targetLevel; i++)
            {
                total += BASE_EXP * Mathf.Pow(EXP_GROWTH_RATE, i - 1);
            }

            return total;
        }

        public void Dispose()
        {
            applyUpgradeFunc = null;
            rollOptionsFunc = null;
            luaUpgradeSystem?.Dispose();
            luaUpgradeSystem = null;
        }
    }

    /// <summary>
    /// 升级选项数据
    /// </summary>
    [Serializable]
    public class UpgradeOption
    {
        public string id;
        public string name;
        public string description;
        public string iconKey;
        public string type; // "weapon" or "passive"
        public string attributeType; // 属性类型（仅被动属性使用）
        public float value; // 属性值（仅被动属性使用）
    }
}