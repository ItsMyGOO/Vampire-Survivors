using System;
using System.Collections.Generic;
using ConfigHandler;

namespace Battle
{
    public class UpgradeService
    {
        // =========================
        // 对外 API（Lua / UI 调用）
        // =========================
        private WeaponUpgradePoolConfigDB WeaponUpgradePoolConfigDB;
        private WeaponUpgradeRuleConfigDB WeaponUpgradeRuleConfigDB;
        private PassiveUpgradePoolConfigDB PassiveUpgradePoolConfigDB;


        public UpgradeService(WeaponUpgradePoolConfigDB weaponUpgradePoolConfigDB,
            WeaponUpgradeRuleConfigDB weaponUpgradeRuleConfigDB,
            PassiveUpgradePoolConfigDB passiveUpgradePoolConfigDB)
        {
            WeaponUpgradePoolConfigDB = weaponUpgradePoolConfigDB;
            WeaponUpgradeRuleConfigDB = weaponUpgradeRuleConfigDB;
            PassiveUpgradePoolConfigDB = passiveUpgradePoolConfigDB;
        }

        public List<UpgradeOption> RollOptions(
            int optionCount,
            int playerLevel,
            IReadOnlyDictionary<string, int> ownedWeapons,
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

        private void CollectWeaponCandidates(
            List<WeightedCandidate> list,
            int playerLevel,
            IReadOnlyDictionary<string, int> ownedWeapons)
        {
            foreach (var (weaponId, def) in WeaponUpgradePoolConfigDB.Data)
            {
                // 等级限制
                if (playerLevel < def.unlockLevel)
                    continue;

                bool owned = ownedWeapons.TryGetValue(weaponId, out var level);

                if (!WeaponUpgradeRuleConfigDB.Data.TryGetValue(weaponId, out var ruleDef))
                    continue;
                // 已满级排除
                if (owned && def.excludeIfMax && level >= ruleDef.maxLevel)
                    continue;

                list.Add(new WeightedCandidate
                {
                    id = weaponId,
                    type = UpgradeOptionType.Weapon,
                    weight = def.weight
                });
            }
        }

        // =========================
        // Passive Pool
        // =========================

        private void CollectPassiveCandidates(
            List<WeightedCandidate> list,
            int playerLevel,
            IReadOnlyDictionary<string, int> passiveLevels)
        {
            foreach (var (passiveId, def) in PassiveUpgradePoolConfigDB.Data)
            {
                if (playerLevel < def.unlockLevel)
                    continue;

                passiveLevels.TryGetValue(passiveId, out var level);

                if (def.excludeIfMax && level >= def.maxLevel)
                    continue;

                list.Add(new WeightedCandidate
                {
                    id = passiveId,
                    type = UpgradeOptionType.Passive,
                    weight = def.weight
                });
            }
        }

        // =========================
        // Weighted Roll
        // =========================

        private List<UpgradeOption> RollFromCandidates(
            List<WeightedCandidate> candidates,
            int count)
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

                    return new UpgradeOption
                    {
                        id = c.id,
                        type = c.type,
                        name = view.name,
                        description = view.description,
                        icon = view.icon
                    };
                }

                case UpgradeOptionType.Passive:
                {
                    var def = PassiveUpgradePoolConfigDB.Instance.Get(c.id);

                    return new UpgradeOption
                    {
                        id = c.id,
                        type = c.type,
                        name = def.attribute.ToString(),
                        description = $"+{def.valuePerLevel} {def.attribute}",
                        icon = $"icon_{def.attribute.ToString().ToLower()}"
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