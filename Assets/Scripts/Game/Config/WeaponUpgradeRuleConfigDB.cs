using System;
using System.Collections.Generic;
using Framework.Config;

namespace ConfigHandler
{
    public sealed class WeaponUpgradeRuleConfigDB
        : SingletonConfigDB<WeaponUpgradeRuleConfigDB, string, WeaponUpgradeRuleDef>
    {
        public const string ConfigFileName = "weapon_upgrade_rule_config.json";

        public static WeaponUpgradeRuleConfigDB Load()
        {
            return CustomLoad(ConfigFileName);
        }

        public static WeaponUpgradeRuleConfigDB CustomLoad(string fileName)
        {
            var wrapper = JsonConfigLoader.Load<WeaponUpgradeRuleConfigRoot>(fileName);
            if (wrapper == null || wrapper.items == null)
                return null;

            var db = new WeaponUpgradeRuleConfigDB();
            foreach (var kvp in wrapper.items)
            {
                db.Add(kvp.Key, kvp.Value);
            }
            return db;
        }
    }

    [Serializable]
    public class WeaponUpgradeRuleConfigRoot
    {
        public Dictionary<string, WeaponUpgradeRuleDef> items;
    }

    [Serializable]
    public class WeaponUpgradeRuleDef
    {
        public string weaponId;
        public int maxLevel;
    }

}