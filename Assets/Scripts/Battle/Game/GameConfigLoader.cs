using System;
using ConfigHandler;
using UnityEngine;

namespace Battle
{
    public static class GameConfigLoader
    {
        public static void LoadAll()
        {
            ConfigLoader.Load(CharacterConfigDB.Load,          CharacterConfigDB.Initialize,          "Character");
            ConfigLoader.Load(AnimationConfigDB.Load,          AnimationConfigDB.Initialize,          "Animation");
            ConfigLoader.Load(EnemyConfigDB.Load,              EnemyConfigDB.Initialize,              "Enemy");
            ConfigLoader.Load(WeaponConfigDB.Load,             WeaponConfigDB.Initialize,             "Weapon");
            ConfigLoader.Load(DropItemConfigDB.Load,           DropItemConfigDB.Initialize,           "DropItem");
            ConfigLoader.Load(WeaponUpgradeRuleConfigDB.Load,  WeaponUpgradeRuleConfigDB.Initialize,  "WeaponUpgradeRule");
            ConfigLoader.Load(WeaponUpgradePoolConfigDB.Load,  WeaponUpgradePoolConfigDB.Initialize,  "WeaponUpgradePool");
            ConfigLoader.Load(PassiveUpgradePoolConfigDB.Load, PassiveUpgradePoolConfigDB.Initialize, "PassiveUpgradePool");

            Debug.Log("[GameConfigLoader] 所有配置加载完成");
        }
    }
    
    public static class ConfigLoader
    {
        public static void Load<T>(
            Func<T> loadFunc,
            Action<T> initFunc,
            string name
        ) where T : class
        {
            var db = loadFunc();
            if (db != null)
            {
                initFunc(db);
                Debug.Log($"✓ {name} 加载完成");
            }
            else
            {
                Debug.LogError($"✗ {name} 加载失败");
            }
        }
    }
}