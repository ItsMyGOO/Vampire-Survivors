using System;
using System.Collections.Generic;
using Framework.Config;
using UnityEngine;

namespace ConfigHandler
{
    // =========================
    // Character Config DB
    // =========================
    public sealed class CharacterConfigDB : SingletonConfigDB<CharacterConfigDB, string, CharacterDef>
    {
        public const string ConfigFileName = "character_config.json";

        public CharacterDef GetCharacter(string id)
        {
            return Get(id);
        }

        public bool TryGetCharacter(string id, out CharacterDef def)
        {
            return TryGet(id, out def);
        }

        /// <summary>
        /// 返回所有角色定义的有序列表（保留 JSON 中的顺序）
        /// </summary>
        public List<CharacterDef> GetAllCharacters()
        {
            return _orderedList;
        }

        private readonly List<CharacterDef> _orderedList = new List<CharacterDef>();

        public static CharacterConfigDB Load()
        {
            var root = JsonConfigLoader.Load<CharacterConfigRoot>(ConfigFileName);
            if (root == null || root.characters == null)
                return null;

            var db = new CharacterConfigDB();

            foreach (var def in root.characters)
            {
                if (string.IsNullOrEmpty(def.id))
                {
                    Debug.LogWarning("[CharacterConfigDB] 发现 id 为空的角色配置，已跳过");
                    continue;
                }

                db.Add(def.id, def);
                db._orderedList.Add(def);
                Debug.Log($"  角色加载: {def.id} ({def.displayName})");
            }

            return db;
        }
    }
}
