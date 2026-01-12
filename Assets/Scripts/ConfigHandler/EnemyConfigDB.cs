using System;
using System.Collections.Generic;
using Framework.Config;
using UnityEngine;

namespace ConfigHandler
{
    // =========================
    // Enemy Config DB
    // =========================
    public sealed class EnemyConfigDB : SingletonConfigDB<EnemyConfigDB, int, Dictionary<string, EnemyDef>>
    {
        public const string ConfigFileName = "enemy_config.json";

        public Dictionary<string, EnemyDef> GetLevel(int levelId)
        {
            return Get(levelId);
        }

        public EnemyDef GetRandomEnemy(int levelId)
        {
            if (!TryGet(levelId, out var enemies) || enemies.Count == 0)
                return null;

            var keys = new List<string>(enemies.Keys);
            string randomKey = keys[UnityEngine.Random.Range(0, keys.Count)];
            return enemies[randomKey];
        }

        public static EnemyConfigDB Load(string fileName = ConfigFileName)
        {
            var wrapper = JsonConfigLoader.Load<EnemyConfigRoot>(fileName);
            if (wrapper == null || wrapper.levels == null)
                return null;

            var db = new EnemyConfigDB();

            foreach (var levelKvp in wrapper.levels)
            {
                int levelId = int.Parse(levelKvp.Key);
                var enemies = levelKvp.Value;

                Debug.Log($"加载关卡 {levelId} 配置: {enemies.Count} 个敌人");

                foreach (var enemyKvp in enemies)
                {
                    var enemyId = enemyKvp.Key;
                    var enemyDef = enemyKvp.Value;
                    
                    Debug.Log($"  - {enemyId}: HP={enemyDef.hp}, ClipSet={enemyDef.clipSetId}");
                }

                db.Add(levelId, enemies);
            }

            return db;
        }
    }

    // =========================
    // JSON Root
    // =========================
    [Serializable]
    public class EnemyConfigRoot
    {
        public Dictionary<string, Dictionary<string, EnemyDef>> levels;
    }

    // =========================
    // Enemy Definition
    // =========================
    [Serializable]
    public class EnemyDef
    {
        public string clipSetId;
        public float hp;

        // 属性访问器
        public string ClipSetId => clipSetId;
        public float Hp => hp;
    }
}
