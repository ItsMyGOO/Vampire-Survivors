using System;
using System.Collections.Generic;
using Framework.Config;
using UnityEngine;

namespace ConfigHandler
{
    public sealed class PassiveUpgradePoolConfigDB
        : SingletonConfigDB<PassiveUpgradePoolConfigDB, string, PassiveUpgradePoolDef>
    {
        public const string ConfigFileName = "PassiveUpgradePoolConfig.json";

        public static PassiveUpgradePoolConfigDB Load()
        {
            return CustomLoad(ConfigFileName);
        }

        public static PassiveUpgradePoolConfigDB CustomLoad(string fileName)
        {
            var root = JsonConfigLoader.Load<PassiveUpgradePoolConfigRoot>(fileName);
            if (root == null || root.passives == null)
            {
                Debug.LogError($"[PassiveUpgradePoolConfigDB] 加载失败: {fileName}");
                return null;
            }

            var db = new PassiveUpgradePoolConfigDB();

            foreach (var kv in root.passives)
            {
                db.Add(kv.Key, kv.Value);
            }

            Debug.Log($"[PassiveUpgradePoolConfigDB] 加载成功: {db.Data.Count} 个被动技能配置");
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
        Duration,
        ExpGain,
        ProjectileSpeed
    }
}
