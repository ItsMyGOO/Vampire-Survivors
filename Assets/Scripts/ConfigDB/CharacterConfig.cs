using System;
using System.Collections.Generic;

namespace ConfigHandler
{
    // =========================
    // JSON Root
    // =========================
    [Serializable]
    public class CharacterConfigRoot
    {
        public List<CharacterDef> characters;
    }

    // =========================
    // Character Definition
    // =========================
    [Serializable]
    public class CharacterDef
    {
        public string id;
        public string displayName;
        public string description;
        public string clipSetId;
        public string spriteKey;

        // 基础属性覆盖（相对于默认值的覆盖，null 表示使用默认）
        public float? maxHealth;
        public float? healthRegen;
        public float? moveSpeed;
        public float? armor;
        public float? attackDamage;
        public float? attackSpeed;
        public float? criticalChance;
        public float? criticalDamage;
        public float? areaSize;
        public float? pickupRange;
        public float? expGain;
        public float? cooldownReduction;
        public float? damageReduction;
        public float? dodgeChance;

        // 初始武器列表
        public List<CharacterStartingWeapon> startingWeapons;

        // 特性描述（展示用，纯文本）
        public List<string> traits;
    }

    // =========================
    // Starting Weapon
    // =========================
    [Serializable]
    public class CharacterStartingWeapon
    {
        public string weaponId;
        public int level;
    }
}
