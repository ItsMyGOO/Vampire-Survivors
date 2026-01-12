using System;
using System.Collections.Generic;
using UnityEngine;

namespace ConfigHandler
{
    // =========================
    // Weapon Config DB
    // =========================
    public sealed class WeaponConfigDB : SingletonConfigDB<WeaponConfigDB, string, WeaponDef>
    {
        public const string ConfigFileName = "weapon_config.json";

        public bool TryGetWeapon(string weaponId, out WeaponDef weapon)
        {
            return TryGet(weaponId, out weapon);
        }

        public static WeaponConfigDB Load(string fileName = ConfigFileName)
        {
            var wrapper = JsonConfigLoader.Load<WeaponConfigRoot>(fileName);
            if (wrapper == null || wrapper.weapons == null)
                return null;

            var db = new WeaponConfigDB();

            foreach (var kvp in wrapper.weapons)
            {
                var weaponId = kvp.Key;
                var weaponDef = kvp.Value;

                // 解析武器类型
                weaponDef.ParsedType = weaponDef.type == "Projectile" 
                    ? WeaponType.Projectile 
                    : WeaponType.Orbit;

                db.Add(weaponId, weaponDef);
                Debug.Log($"加载武器配置: {weaponId} (类型: {weaponDef.ParsedType}, 击退: {weaponDef.knockback})");
            }

            return db;
        }
    }

    // =========================
    // JSON Root
    // =========================
    [Serializable]
    public class WeaponConfigRoot
    {
        public Dictionary<string, WeaponDef> weapons;
    }

    // =========================
    // Weapon Definition
    // =========================
    [Serializable]
    public class WeaponDef
    {
        // JSON 字段
        public string type;
        public float interval;
        public float baseDamage;
        public float baseSpeed;
        public int baseCount;
        public float range;
        public float baseRadius;
        public float orbitSpeed;
        public float knockback;
        public string sheet;
        public string key;

        // 运行时字段
        [NonSerialized]
        public WeaponType ParsedType;

        // 属性访问器
        public WeaponType Type => ParsedType;
        public string Sheet => sheet;
        public string Key => key;
        public float Interval => interval;
        public float BaseDamage => baseDamage;
        public float BaseSpeed => baseSpeed;
        public int BaseCount => baseCount;
        public float Range => range;
        public float BaseRadius => baseRadius;
        public float OrbitSpeed => orbitSpeed;
        public float Knockback => knockback;
    }

    // =========================
    // Enum
    // =========================
    public enum WeaponType
    {
        Projectile,
        Orbit
    }
}
