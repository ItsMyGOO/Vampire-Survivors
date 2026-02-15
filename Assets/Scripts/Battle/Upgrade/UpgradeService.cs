using System;
using System.Collections.Generic;
using Battle.Player;
using Battle.Weapon;
using ConfigHandler;
using ECS;
using ECS.Core;
using UnityEngine;
using XLua;

namespace Battle.Upgrade
{
    public class UpgradeService : IDisposable
    {
        public Action<UpgradeOption> OnApplyUpgradeOptions;

        // =========================
        // 瀵瑰 API锛圠ua / UI 璋冪敤锛?
        // =========================
        private WeaponUpgradePoolConfigDB WeaponUpgradePoolConfigDB;
        private WeaponUpgradeRuleConfigDB WeaponUpgradeRuleConfigDB;
        private PassiveUpgradePoolConfigDB PassiveUpgradePoolConfigDB;

        // 娴嬭瘯妯″紡寮€鍏?
        [NonSerialized] public bool testMode = true; // 璁句负 true 鍚敤鑷姩闅忔満鍗囩骇娴嬭瘯

        /// <summary>
        /// 鍗囩骇閫夐」鍙敤浜嬩欢锛圲I / Lua 鐩戝惉锛?
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
                Debug.LogWarning("[ExpSystem] 娌℃湁鍙敤鐨勫崌绾ч€夐」");
                return;
            }

            Debug.Log($"[ExpSystem] 鐢熸垚浜?{options.Count} 涓崌绾ч€夐」:");
            for (int i = 0; i < options.Count; i++)
            {
                var opt = options[i];
                Debug.Log($"  [{i}] {opt.type} - {opt.name} (ID: {opt.id})");
            }

            // 瑙﹀彂浜嬩欢
            OnUpgradeOptionsReady?.Invoke(options);

            // 娴嬭瘯妯″紡锛氳嚜鍔ㄩ殢鏈洪€夋嫨涓€涓鍣ㄥ崌绾ч€夐」
            if (testMode)
            {
                AutoSelectUpgradeForTest(options);
            }
        }

        /// <summary>
        /// 娴嬭瘯妯″紡锛氳嚜鍔ㄩ殢鏈洪€夋嫨涓€涓崌绾ч€夐」
        /// </summary>
        private void AutoSelectUpgradeForTest(List<UpgradeOption> options)
        {
            // 浼樺厛閫夋嫨姝﹀櫒绫诲瀷鐨勯€夐」
            var weaponOptions = options.FindAll(opt => opt.type == UpgradeOptionType.Weapon);

            UpgradeOption selected;
            if (weaponOptions.Count > 0)
            {
                // 闅忔満閫夋嫨涓€涓鍣ㄩ€夐」
                int randomIndex = UnityEngine.Random.Range(0, weaponOptions.Count);
                selected = weaponOptions[randomIndex];
                Debug.Log($"[ExpSystem] 娴嬭瘯妯″紡 - 闅忔満閫夋嫨姝﹀櫒: {selected.name} (ID: {selected.id})");
            }
            else if (options.Count > 0)
            {
                // 濡傛灉娌℃湁姝﹀櫒閫夐」锛岄殢鏈洪€夋嫨浠绘剰閫夐」
                int randomIndex = UnityEngine.Random.Range(0, options.Count);
                selected = options[randomIndex];
                Debug.Log($"[ExpSystem] 娴嬭瘯妯″紡 - 闅忔満閫夋嫨閫夐」: {selected.name} (ID: {selected.id})");
            }
            else
            {
                Debug.LogWarning("[ExpSystem] 娴嬭瘯妯″紡 - 娌℃湁鍙€夋嫨鐨勯€夐」");
                return;
            }

            // 搴旂敤閫夋嫨鐨勫崌绾?
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
                // 绛夌骇闄愬埗
                if (playerLevel < def.unlockLevel)
                    continue;

                bool owned = ownedWeapons.TryGetValue(weaponId, out var stats);
                int level = owned ? stats.level : 0;

                if (!WeaponUpgradeRuleConfigDB.Data.TryGetValue(weaponId, out var ruleDef))
                    continue;
                // 宸叉弧绾ф帓闄?
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

                int roll = UnityEngine.Random.Range(0, totalWeight);
                int acc = 0;

                for (int j = 0; j < pool.Count; j++)
                {
                    acc += pool[j].weight;
                    if (roll < acc)
                    {
                        var picked = pool[j];
                        results.Add(BuildOption(picked));
                        pool.RemoveAt(j); // 闃叉閲嶅
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
                            : $"{view.description} (Lv.{c.currentLevel} 鈫?Lv.{nextLevel})",
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

        // 鍙€夛細缁?UI 鐢?
        public int nextLevel;
    }
}