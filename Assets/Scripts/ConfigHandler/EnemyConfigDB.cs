// ============================================
// 文件: EnemyConfigHandler.cs
// 敌人配置加载器 - 从 Lua 加载敌人定义
// ============================================

using System.Collections.Generic;
using XLua;
using UnityEngine;

namespace ConfigHandler
{
    /// <summary>
    /// 敌人配置数据库 - 单例
    /// </summary>
    public sealed class EnemyConfigDB
    {
        private static EnemyConfigDB _instance;
        public static EnemyConfigDB Instance => _instance;

        public static void Initialize(EnemyConfigDB db)
        {
            _instance = db;
        }

        // 数据结构: levels[levelId][enemyId] = EnemyDef
        private readonly Dictionary<int, Dictionary<string, EnemyDef>> _levels =
            new Dictionary<int, Dictionary<string, EnemyDef>>();

        internal void AddLevel(int levelId, Dictionary<string, EnemyDef> enemies)
        {
            _levels[levelId] = enemies;
        }

        /// <summary>
        /// 获取指定关卡的所有敌人定义
        /// </summary>
        public bool TryGetLevel(int levelId, out Dictionary<string, EnemyDef> enemies)
        {
            return _levels.TryGetValue(levelId, out enemies);
        }

        /// <summary>
        /// 获取指定关卡的敌人定义（不安全版本）
        /// </summary>
        public Dictionary<string, EnemyDef> GetLevel(int levelId)
        {
            _levels.TryGetValue(levelId, out var enemies);
            return enemies;
        }

        /// <summary>
        /// 获取指定关卡的随机敌人定义
        /// </summary>
        public EnemyDef GetRandomEnemy(int levelId)
        {
            if (!_levels.TryGetValue(levelId, out var enemies) || enemies.Count == 0)
                return null;

            // 随机选择一个敌人
            var keys = new List<string>(enemies.Keys);
            string randomKey = keys[Random.Range(0, keys.Count)];
            return enemies[randomKey];
        }

        /// <summary>
        /// 获取所有关卡ID
        /// </summary>
        public IEnumerable<int> GetAllLevelIds()
        {
            return _levels.Keys;
        }
    }

    /// <summary>
    /// 敌人配置加载器 - 从 Lua 加载
    /// </summary>
    public static class EnemyConfigLoader
    {
        /// <summary>
        /// 从 Lua 加载所有敌人配置
        /// 对应文件: Assets/Lua/Data/enemy_config.lua
        /// </summary>
        public static EnemyConfigDB LoadAll(LuaEnv env)
        {
            var table = env.DoString("return require 'Data.enemy_config'")[0] as LuaTable;
            var db = new EnemyConfigDB();

            foreach (var levelKey in table.GetKeys())
            {
                string levelStr = levelKey.ToString();

                // 解析关卡ID (例如 "level_1" -> 1)
                if (!levelStr.StartsWith("level_"))
                    continue;

                int levelId = int.Parse(levelStr.Substring(6));
                var levelTable = table.Get<LuaTable>(levelKey);

                var enemies = new Dictionary<string, EnemyDef>();

                foreach (var enemyKey in levelTable.GetKeys())
                {
                    string enemyId = enemyKey.ToString();
                    var enemyTable = levelTable.Get<LuaTable>(enemyKey);

                    var enemyDef = BuildEnemyDef(enemyTable);
                    enemies[enemyId] = enemyDef;

                    Debug.Log(
                        $"加载敌人配置: Level {levelId} - {enemyId} (HP: {enemyDef.Hp}, ClipSet: {enemyDef.ClipSetId})");
                }

                db.AddLevel(levelId, enemies);
            }

            return db;
        }

        /// <summary>
        /// 构建敌人定义
        /// </summary>
        private static EnemyDef BuildEnemyDef(LuaTable table)
        {
            var enemyDef = new EnemyDef
            {
                ClipSetId = table.Get<string>("clipSetId"),
                Hp = table.Get<float>("hp")
            };

            return enemyDef;
        }
    }

    /// <summary>
    /// 敌人定义数据结构
    /// </summary>
    public class EnemyDef
    {
        public string ClipSetId; // 动画集ID（对应 animation_db.lua）
        public float Hp; // 生命值
    }
}