using System;
using System.Collections.Generic;
using Battle.Weapon;
using ConfigHandler;
using ECS;
using ECS.Core;
using UnityEngine;
using XLua;
using Random = UnityEngine.Random;

namespace Battle.Upgrade
{
    public class UpgradeService : IDisposable
    {
        public Action<UpgradeOption> OnApplyUpgradeOptions;

        // =========================
        // 对外 API（Lua / UI 调用）
        // =========================
        private WeaponUpgradePoolConfigDB WeaponUpgradePoolConfigDB;
        private WeaponUpgradeRuleConfigDB WeaponUpgradeRuleConfigDB;
        private PassiveUpgradePoolConfigDB PassiveUpgradePoolConfigDB;

        // 测试模式开关
        [NonSerialized] public bool testMode = true; // 设为 true 启用自动随机升级测试

        /// <summary>
        /// 升级选项可用事件（UI / Lua 监听）
        /// </summary>
        public event Action<List<UpgradeOption>> OnUpgradeOptionsReady;

        private LuaEnv luaEnv;
        private LuaTable luaUpgradeFlow;
        private LuaFunction luaOnUpgradeOptions;

        World world;
        int playerId;

        public UpgradeService(LuaEnv luaEnv, World world, int playerId)
        {
            this.luaEnv = luaEnv;
            this.world = world;
            this.playerId = playerId;

            WeaponUpgradePoolConfigDB = WeaponUpgradePoolConfigDB.Instance;
            WeaponUpgradeRuleConfigDB = WeaponUpgradeRuleConfigDB.Instance;
            PassiveUpgradePoolConfigDB = PassiveUpgradePoolConfigDB.Instance;

            InitLuaUpgradeFlow();
        }

        private void InitLuaUpgradeFlow()
        {
            var ret = luaEnv.DoString(@"
                local flow = require('battle.upgrade_flow')
                return flow
            ");

            luaUpgradeFlow = ret[0] as LuaTable;
            luaOnUpgradeOptions = luaUpgradeFlow.Get<LuaFunction>("OnUpgradeOptions");
        }

        public void Dispose()
        {
            luaOnUpgradeOptions?.Dispose();

            luaUpgradeFlow?.Dispose();
            luaUpgradeFlow = null;

            luaEnv = null;
        }

        // =========================
        // Upgrade
        // =========================

        public void RollUpgradeOptions(int level)
        {
            var options = RollOptions(
                optionCount: 3,
                playerLevel: level
            );

            if (options == null || options.Count == 0)
            {
                Debug.LogWarning("[ExpSystem] 没有可用的升级选项");
                return;
            }

            Debug.Log($"[ExpSystem] 生成了 {options.Count} 个升级选项:");
            for (int i = 0; i < options.Count; i++)
            {
                var opt = options[i];
                Debug.Log($"  [{i}] {opt.type} - {opt.name} (ID: {opt.id})");
            }

            // 触发事件
            OnUpgradeOptionsReady?.Invoke(options);

            // 测试模式：自动随机选择一个武器升级选项
            if (testMode)
            {
                AutoSelectUpgradeForTest(options);
            }
        }

        /// <summary>
        /// 测试模式：自动随机选择一个升级选项
        /// </summary>
        private void AutoSelectUpgradeForTest(List<UpgradeOption> options)
        {
            // 优先选择武器类型的选项
            var weaponOptions = options.FindAll(opt => opt.type == UpgradeOptionType.Weapon);

            UpgradeOption selected;
            if (weaponOptions.Count > 0)
            {
                // 随机选择一个武器选项
                int randomIndex = Random.Range(0, weaponOptions.Count);
                selected = weaponOptions[randomIndex];
                Debug.Log($"[ExpSystem] 测试模式 - 随机选择武器: {selected.name} (ID: {selected.id})");
            }
            else if (options.Count > 0)
            {
                // 如果没有武器选项，随机选择任意选项
                int randomIndex = Random.Range(0, options.Count);
                selected = options[randomIndex];
                Debug.Log($"[ExpSystem] 测试模式 - 随机选择选项: {selected.name} (ID: {selected.id})");
            }
            else
            {
                Debug.LogWarning("[ExpSystem] 测试模式 - 没有可选择的选项");
                return;
            }

            // 应用选择的升级
            OnApplyUpgradeOptions.Invoke(selected);
        }

        public List<UpgradeOption> RollOptions(int optionCount, int playerLevel)
        {
            var weapons = world.GetComponent<WeaponRuntimeStatsComponent>(playerId).GetAllWeapons();

            IReadOnlyDictionary<string, int> passiveLevels = null;
            var passiveState = world.GetComponent<PassiveUpgradeStateComponent>(playerId);
            if (passiveState != null)
            {
                passiveLevels = passiveState.levels;
            }

            return RollOptions(optionCount, playerLevel, weapons, passiveLevels);
        }

        public List<UpgradeOption> RollOptions(int optionCount, int playerLevel,
            IReadOnlyDictionary<string, WeaponRuntimeStats> ownedWeapons,
            IReadOnlyDictionary<string, int> passiveLevels)
        {
            var candidates = new List<WeightedCandidate>();

            CollectWeaponCandidates(
                candidates,
                playerLevel,
                ownedWeapons
            );

            CollectPassiveCandidates(
                candidates,
                playerLevel,
                passiveLevels
            );

            return RollFromCandidates(candidates, optionCount);
        }

        // =========================
        // Weapon Pool
        // =========================

        private void CollectWeaponCandidates(List<WeightedCandidate> list, int playerLevel,
            IReadOnlyDictionary<string, WeaponRuntimeStats> ownedWeapons)
        {
            foreach (var (weaponId, def) in WeaponUpgradePoolConfigDB.Data)
            {
                // 等级限制
                if (playerLevel < def.unlockLevel)
                    continue;

                bool owned = ownedWeapons.TryGetValue(weaponId, out var stats);
                int level = owned ? stats.level : 0;

                if (!WeaponUpgradeRuleConfigDB.Data.TryGetValue(weaponId, out var ruleDef))
                    continue;
                // 已满级排除
                if (owned && def.excludeIfMax && level >= ruleDef.maxLevel)
                    continue;

                list.Add(new WeightedCandidate
                {
                    id = weaponId,
                    type = UpgradeOptionType.Weapon,
                    weight = def.weight,
                    currentLevel = owned ? level : 0
                });
            }
        }

        // =========================
        // Passive Pool
        // =========================

        private void CollectPassiveCandidates(List<WeightedCandidate> list, int playerLevel,
            IReadOnlyDictionary<string, int> passiveLevels)
        {
            foreach (var (passiveId, def) in PassiveUpgradePoolConfigDB.Data)
            {
                if (playerLevel < def.unlockLevel)
                    continue;

                int level = 0;
                passiveLevels?.TryGetValue(passiveId, out level);

                if (def.excludeIfMax && level >= def.maxLevel)
                    continue;

                list.Add(new WeightedCandidate
                {
                    id = passiveId,
                    type = UpgradeOptionType.Passive,
                    weight = def.weight,
                    currentLevel = level
                });
            }
        }

        // =========================
        // Weighted Roll
        // =========================

        private List<UpgradeOption> RollFromCandidates(List<WeightedCandidate> candidates, int count)
        {
            var results = new List<UpgradeOption>();
            var pool = new List<WeightedCandidate>(candidates);

            for (int i = 0; i < count && pool.Count > 0; i++)
            {
                int totalWeight = 0;
                foreach (var c in pool)
                    totalWeight += c.weight;

                int roll = Random.Range(0, totalWeight);
                int acc = 0;

                for (int j = 0; j < pool.Count; j++)
                {
                    acc += pool[j].weight;
                    if (roll < acc)
                    {
                        var picked = pool[j];
                        results.Add(BuildOption(picked));
                        pool.RemoveAt(j); // 防止重复
                        break;
                    }
                }
            }

            return results;
        }

        // =========================
        // Build UI Option
        // =========================

        private UpgradeOption BuildOption(WeightedCandidate c)
        {
            switch (c.type)
            {
                case UpgradeOptionType.Weapon:
                {
                    var view = WeaponConfigDB.Instance.Get(c.id).view;
                    int nextLevel = c.currentLevel + 1;

                    return new UpgradeOption
                    {
                        id = c.id,
                        type = c.type,
                        name = view.name,
                        description = c.currentLevel == 0
                            ? view.description
                            : $"{view.description} (Lv.{c.currentLevel} → Lv.{nextLevel})",
                        icon = view.icon,
                        nextLevel = nextLevel
                    };
                }

                case UpgradeOptionType.Passive:
                {
                    var def = PassiveUpgradePoolConfigDB.Instance.Get(c.id);
                    int nextLevel = c.currentLevel + 1;

                    return new UpgradeOption
                    {
                        id = c.id,
                        type = c.type,
                        name = def.attribute.ToString(),
                        description = $"+{def.valuePerLevel} {def.attribute} (Lv.{nextLevel})",
                        icon = $"icon_{def.attribute.ToString().ToLower()}",
                        nextLevel = nextLevel
                    };
                }

                default:
                    throw new Exception("Unknown UpgradeOptionType");
            }
        }

        // =========================
        // Internal
        // =========================

        private class WeightedCandidate
        {
            public string id;
            public UpgradeOptionType type;
            public int weight;
            public int currentLevel;
        }
    }

    public enum UpgradeOptionType
    {
        Weapon,
        Passive
    }

    public class UpgradeOption
    {
        public string id; // weaponId / passiveId
        public UpgradeOptionType type;

        public string name;
        public string description;
        public string icon;

        // 可选：给 UI 用
        public int nextLevel;
    }
}