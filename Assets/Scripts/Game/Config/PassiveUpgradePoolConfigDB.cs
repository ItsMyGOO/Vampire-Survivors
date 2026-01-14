using System;
using System.Collections.Generic;
using Framework.Config;

namespace ConfigHandler
{
    public sealed class PassiveUpgradePoolConfigDB
        : SingletonConfigDB<PassiveUpgradePoolConfigDB, string, PassiveUpgradePoolDef>
    {
        public const string ConfigFileName = "passive_upgrade_pool_config.json";

        public static PassiveUpgradePoolConfigDB Load()
        {
            return CustomLoad(ConfigFileName);
        }

        public static PassiveUpgradePoolConfigDB CustomLoad(string fileName)
        {
            var root = JsonConfigLoader.Load<PassiveUpgradePoolConfigRoot>(fileName);
            if (root == null || root.passives == null)
                return null;

            var db = new PassiveUpgradePoolConfigDB();

            foreach (var kv in root.passives)
            {
                db.Add(kv.Key, kv.Value);
            }

            return db;
        }
    }

    [Serializable]
    public class PassiveUpgradePoolConfigRoot
    {
        public Dictionary<string, PassiveUpgradePoolDef> passives;
    }

    [Serializable]
    public class PassiveUpgradePoolDef
    {
        public PassiveAttributeType attribute;
        public float valuePerLevel;
        public int maxLevel;

        public int weight;
        public int unlockLevel;
        public bool excludeIfMax;
    }

    public enum PassiveAttributeType
    {
        MaxHealth,
        MoveSpeed,
        AttackDamage,
        AttackSpeed,
        CriticalChance,
        AreaSize,
        ProjectileCount,
        PickupRange,
        CooldownReduction,
        Duration
    }
}