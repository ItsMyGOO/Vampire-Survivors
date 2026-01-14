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
            if (wrapper == null || wrapper.weapons == null)
            {
                UnityEngine.Debug.LogError($"[WeaponUpgradePoolConfigDB] 加载失败: {fileName}");
                return null;
            }

            var db = new WeaponUpgradePoolConfigDB();
            foreach (var kvp in wrapper.weapons)
            {
                var def = kvp.Value;
                def.weaponId = kvp.Key; // 设置 weaponId
                db.Add(kvp.Key, def);
            }
            
            UnityEngine.Debug.Log($"[WeaponUpgradePoolConfigDB] 加载成功: {db.Data.Count} 个武器配置");
            return db;
        }
    }

    [Serializable]
    public class WeaponUpgradePoolConfigRoot
    {
        public Dictionary<string, WeaponUpgradePoolDef> weapons;
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
