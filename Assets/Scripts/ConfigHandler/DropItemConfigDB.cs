using System;
using System.Collections.Generic;

namespace ConfigHandler
{
    // =========================
    // Drop Item Config DB
    // =========================
    public sealed class DropItemConfigDB : SingletonConfigDB<DropItemConfigDB, string, DropItemDef>
    {
        public const string ConfigFileName = "prop_config.json";

        public static DropItemConfigDB Load(string fileName = ConfigFileName)
        {
            var wrapper = JsonConfigLoader.Load<DropItemConfigRoot>(fileName);
            if (wrapper == null || wrapper.items == null)
                return null;

            var db = new DropItemConfigDB();

            foreach (var kvp in wrapper.items)
            {
                db.Add(kvp.Key, kvp.Value);
            }

            return db;
        }
    }

    // =========================
    // JSON Root
    // =========================
    [Serializable]
    public class DropItemConfigRoot
    {
        public Dictionary<string, DropItemDef> items;
    }

    // =========================
    // Drop Item Definition
    // =========================
    [Serializable]
    public class DropItemDef
    {
        public string sheet;
        public string key;
        public int exp;
        public float pickupRadius;

        // 属性访问器
        public string Sheet => sheet;
        public string Key => key;
        public int Exp => exp;
        public float PickupRadius => pickupRadius;
    }
}
