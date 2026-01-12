// ============================================
// 文件: WeaponConfigHandler.cs
// 武器配置加载器 - 从 Lua 加载武器定义
// ============================================

using System.Collections.Generic;
using XLua;
using UnityEngine;

namespace ConfigHandler
{
    /// <summary>
    /// 武器配置数据库 - 单例
    /// </summary>
    public sealed class WeaponConfigDB
    {
        private static WeaponConfigDB _instance;
        public static WeaponConfigDB Instance => _instance;

        public static void Initialize(WeaponConfigDB db)
        {
            _instance = db;
        }

        private readonly Dictionary<string, WeaponDef> _weapons = new Dictionary<string, WeaponDef>();

        internal void AddWeapon(string weaponId, WeaponDef weaponDef)
        {
            _weapons[weaponId] = weaponDef;
        }

        /// <summary>
        /// 获取武器定义
        /// </summary>
        public bool TryGetWeapon(string weaponId, out WeaponDef weaponDef)
        {
            return _weapons.TryGetValue(weaponId, out weaponDef);
        }

        /// <summary>
        /// 获取武器定义（不安全版本，找不到返回null）
        /// </summary>
        public WeaponDef GetWeapon(string weaponId)
        {
            _weapons.TryGetValue(weaponId, out var weaponDef);
            return weaponDef;
        }

        /// <summary>
        /// 获取所有武器ID
        /// </summary>
        public IEnumerable<string> GetAllWeaponIds()
        {
            return _weapons.Keys;
        }
    }

    /// <summary>
    /// 武器配置加载器 - 从 Lua 加载
    /// </summary>
    public static class WeaponConfigLoader
    {
        /// <summary>
        /// 从 Lua 加载所有武器配置
        /// 对应文件: Assets/Lua/Data/weapon_defs.lua
        /// </summary>
        public static WeaponConfigDB LoadAll(LuaEnv env)
        {
            var table = env.DoString("return require 'Data.weapon_defs'")[0] as LuaTable;
            var db = new WeaponConfigDB();

            foreach (var weaponKey in table.GetKeys())
            {
                string weaponId = weaponKey.ToString();
                var weaponTable = table.Get<LuaTable>(weaponKey);

                var weaponDef = BuildWeaponDef(weaponTable);
                db.AddWeapon(weaponId, weaponDef);

                Debug.Log($"加载武器配置: {weaponId} (类型: {weaponDef.Type}, 击退: {weaponDef.Knockback})");
            }

            return db;
        }

        /// <summary>
        /// 构建武器定义
        /// </summary>
        private static WeaponDef BuildWeaponDef(LuaTable table)
        {
            var weaponDef = new WeaponDef();

            // 通用属性
            string typeStr = table.Get<string>("type");
            weaponDef.Type = typeStr == "Projectile" ? WeaponType.Projectile : WeaponType.Orbit;
            weaponDef.Sheet = table.Get<string>("sheet");
            weaponDef.Key = table.Get<string>("key");

            // 投射型武器属性
            if (weaponDef.Type == WeaponType.Projectile)
            {
                weaponDef.Interval = table.Get<float>("interval");
                weaponDef.BaseDamage = table.Get<float>("base_damage");
                weaponDef.BaseSpeed = table.Get<float>("base_speed");
                weaponDef.BaseCount = table.Get<int>("base_count");
                weaponDef.Range = table.Get<float>("range");
            }
            // 轨道型武器属性
            else if (weaponDef.Type == WeaponType.Orbit)
            {
                weaponDef.BaseCount = table.Get<int>("base_count");
                weaponDef.BaseRadius = table.Get<float>("base_radius");
                weaponDef.BaseDamage = table.Get<float>("base_damage");
                weaponDef.OrbitSpeed = table.Get<float>("orbit_speed");
            }

            // ⭐ 读取击退力度（所有武器类型通用）
            weaponDef.Knockback = table.Get<float>("knockback");

            return weaponDef;
        }
    }

    /// <summary>
    /// 武器类型枚举
    /// </summary>
    public enum WeaponType
    {
        Projectile, // 投射型（发射子弹）
        Orbit       // 轨道型（围绕旋转）
    }

    /// <summary>
    /// 武器定义数据结构
    /// </summary>
    public class WeaponDef
    {
        // 通用属性
        public WeaponType Type;
        public string Sheet;        // 精灵图集路径
        public string Key;          // 精灵键名

        // 投射型武器属性
        public float Interval;      // 发射间隔
        public float BaseDamage;    // 基础伤害
        public float BaseSpeed;     // 投射物速度
        public int BaseCount;       // 基础数量
        public float Range;         // 射程

        // 轨道型武器属性
        public float BaseRadius;    // 轨道半径
        public float OrbitSpeed;    // 旋转速度（角速度）

        // ⭐ 击退属性（所有武器类型通用）
        public float Knockback;     // 击退力度
    }
}
