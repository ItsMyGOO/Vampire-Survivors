using System;
using System.Collections.Generic;
using Framework.Config;

namespace ConfigHandler
{
    public sealed class WeaponUpgradePoolConfigDB
        : SingletonConfigDB<WeaponUpgradePoolConfigDB, string, WeaponUpgradePoolDef>
    {
        public const string ConfigFileName = "weapon_upgrade_pool_config.json";

        public static WeaponUpgradePoolConfigDB Load()
        {
            return CustomLoad(ConfigFileName);
        }

        public static WeaponUpgradePoolConfigDB CustomLoad(string fileName)
        {
            var wrapper = JsonConfigLoader.Load<WeaponUpgradePoolConfigRoot>(fileName);
            if (wrapper == null || wrapper.items == null)
                return null;

            var db = new WeaponUpgradePoolConfigDB();
            foreach (var kvp in wrapper.items)
            {
                db.Add(kvp.Key, kvp.Value);
            }
            return db;
        }
    }

    [Serializable]
    public class WeaponUpgradePoolConfigRoot
    {
        public Dictionary<string, WeaponUpgradePoolDef> items;
    }

    [Serializable]
    public class WeaponUpgradePoolDef
    {
        public string weaponId;
        public int unlockLevel;
        public bool excludeIfMax;
        public int weight;
    }

}