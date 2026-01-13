using System;
using System.Collections.Generic;
using UnityEngine;

namespace ConfigHandler
{
    // =========================
    // JSON Root
    // =========================
    [Serializable]
    public class WeaponBattleConfig
    {
        public Dictionary<string, WeaponBattleDef> weapons;
    }

    // =========================
    // Weapon Battle Definition
    // =========================
    [Serializable]
    public class WeaponBattleDef
    {
        public string type;

        public WeaponBaseStats baseStats;

        public ProjectileConfig projectile;
        public OrbitConfig orbit;

        public List<WeaponLevelStats> levels;
        public int maxLevel;

        [NonSerialized] private WeaponType parsedType;
        public WeaponType Type => parsedType;

        public void ParseAndValidate(string weaponId)
        {
            parsedType = type switch
            {
                "Projectile" => WeaponType.Projectile,
                "Orbit"      => WeaponType.Orbit,
                _ => throw new Exception($"[{weaponId}] Unknown WeaponType: {type}")
            };

            if (baseStats == null)
                throw new Exception($"[{weaponId}] baseStats missing");

            switch (parsedType)
            {
                case WeaponType.Projectile:
                    if (projectile == null)
                        throw new Exception($"[{weaponId}] Projectile weapon missing projectile config");
                    if (orbit != null)
                        Debug.LogWarning($"[{weaponId}] Projectile weapon should not contain orbit config");
                    break;

                case WeaponType.Orbit:
                    if (orbit == null)
                        throw new Exception($"[{weaponId}] Orbit weapon missing orbit config");
                    if (projectile != null)
                        Debug.LogWarning($"[{weaponId}] Orbit weapon should not contain projectile config");
                    break;
            }

            if (levels == null || levels.Count == 0)
                Debug.LogWarning($"[{weaponId}] levels not configured");

            if (maxLevel <= 0)
                maxLevel = levels?.Count ?? 1;
        }
    }

    // =========================
    // Base Stats
    // =========================
    [Serializable]
    public class WeaponBaseStats
    {
        public float damage;
        public int count;
        public float knockback;
    }

    // =========================
    // Level Override Stats
    // =========================
    [Serializable]
    public class WeaponLevelStats
    {
        public float? damage;
        public int? count;
    }

    // =========================
    // Projectile Config
    // =========================
    [Serializable]
    public class ProjectileConfig
    {
        public float interval;
        public float speed;
        public float range;
    }

    // =========================
    // Orbit Config
    // =========================
    [Serializable]
    public class OrbitConfig
    {
        public float radius;
        public float speed;
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
