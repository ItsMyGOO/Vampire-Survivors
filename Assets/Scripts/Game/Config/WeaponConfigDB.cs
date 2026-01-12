using System;
using System.Collections.Generic;
using Framework.Config;
using UnityEngine;

namespace ConfigHandler
{
    // =========================
    // Weapon Config DB
    // =========================
    public sealed class WeaponConfigDB
        : SingletonConfigDB<WeaponConfigDB, string, WeaponDef>
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

            foreach (var (weaponId, weaponDef) in wrapper.weapons)
            {
                weaponDef.ParseAndValidate();
                db.Add(weaponId, weaponDef);

                Debug.Log(
                    $"加载武器配置: {weaponId} (类型: {weaponDef.Type}, 击退: {weaponDef.knockback})"
                );
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
    // Weapon Definition（仅公共字段）
    // =========================
    [Serializable]
    public class WeaponDef
    {
        // ---------- JSON 字段 ----------
        public string type;

        public float baseDamage;
        public int baseCount;
        public float knockback;

        public string sheet;
        public string key;

        // 子配置
        public ProjectileConfig projectile;
        public OrbitConfig orbit;

        // ---------- 运行时 ----------
        [NonSerialized]
        private WeaponType parsedType;

        public WeaponType Type => parsedType;

        // ---------- 解析 & 校验 ----------
        public void ParseAndValidate()
        {
            parsedType = type switch
            {
                "Projectile" => WeaponType.Projectile,
                "Orbit" => WeaponType.Orbit,
                _ => throw new Exception($"未知 WeaponType: {type}")
            };

            switch (parsedType)
            {
                case WeaponType.Projectile:
                    if (projectile == null)
                        throw new Exception("Projectile 武器缺少 projectile 配置");

                    if (orbit != null)
                        Debug.LogWarning("Projectile 武器不应包含 orbit 配置");
                    break;

                case WeaponType.Orbit:
                    if (orbit == null)
                        throw new Exception("Orbit 武器缺少 orbit 配置");

                    if (projectile != null)
                        Debug.LogWarning("Orbit 武器不应包含 projectile 配置");
                    break;
            }
        }
    }

    // =========================
    // Projectile 子配置
    // =========================
    [Serializable]
    public class ProjectileConfig
    {
        public float interval;
        public float baseSpeed;
        public float range;
    }

    // =========================
    // Orbit 子配置
    // =========================
    [Serializable]
    public class OrbitConfig
    {
        public float radius;
        public float orbitSpeed;
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
