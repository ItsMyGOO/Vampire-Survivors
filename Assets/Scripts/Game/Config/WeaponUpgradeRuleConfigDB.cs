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
            var root = JsonConfigLoader.Load<WeaponUpgradeRuleConfigRoot>(fileName);
            if (root == null || root.weapons == null)
                return null;

            var db = new WeaponUpgradeRuleConfigDB();
            foreach (var kvp in root.weapons)
            {
                var def = kvp.Value;
                def.weaponId = kvp.Key;
                db.Add(kvp.Key, def);
            }
            return db;
        }
    }

    [Serializable]
    public class WeaponUpgradeRuleConfigRoot
    {
        public Dictionary<string, WeaponUpgradeRuleDef> weapons;
    }

    [Serializable]
    public class WeaponUpgradeRuleDef
    {
        public string weaponId;
        public int maxLevel;
        public List<StatUpgradeRule> rules;
    }

    [Serializable]
    public class StatUpgradeRule
    {
        public string stat;    // "Damage", "Count", "Cooldown", "Radius"
        public string op;      // "Add", "Mul"
        public float value;    // 修改值
        public int every;      // 每隔多少级触发（0或不设置表示每级都触发）
    }
}
