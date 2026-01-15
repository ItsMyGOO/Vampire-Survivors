using System;
using Framework.Config;
using UnityEngine;

namespace ConfigHandler
{
    /// <summary>
    /// 武器配置总 DB（Battle + View 聚合）
    /// </summary>
    public sealed class WeaponConfigDB
        : SingletonConfigDB<WeaponConfigDB, string, WeaponConfig>
    {
        public const string BattleConfigFile = "weapon_battle_config.json";
        public const string ViewConfigFile = "weapon_view_config.json";

        public static WeaponConfigDB Load()
        {
            return CustomLoad(BattleConfigFile, ViewConfigFile);
        }

        public static WeaponConfigDB CustomLoad(
            string battleFile,
            string viewFile)
        {
            var battleRoot = JsonConfigLoader.Load<WeaponBattleConfig>(battleFile);
            var viewRoot = JsonConfigLoader.Load<WeaponViewConfigRoot>(viewFile);

            if (battleRoot?.weapons == null)
                throw new Exception("[WeaponConfigDB] WeaponBattleConfig 加载失败");

            var db = new WeaponConfigDB();

            foreach (var (weaponId, battleDef) in battleRoot.weapons)
            {
                battleDef.ParseAndValidate(weaponId);

                WeaponViewDef viewDef = null;
                if (viewRoot?.weapons != null)
                {
                    viewRoot.weapons.TryGetValue(weaponId, out viewDef);
                    viewDef?.Validate(weaponId);
                }

                db.Add(weaponId, new WeaponConfig
                {
                    battle = battleDef,
                    view = viewDef
                });

                Debug.Log($"[WeaponConfigDB] Loaded weapon: {weaponId}");
            }

            return db;
        }


        // -------- 快捷访问 --------

        public bool TryGetBattle(string weaponId, out WeaponBattleDef battle)
        {
            if (TryGet(weaponId, out var cfg))
            {
                battle = cfg.battle;
                return true;
            }

            battle = null;
            return false;
        }

        public bool TryGetView(string weaponId, out WeaponViewDef view)
        {
            if (TryGet(weaponId, out var cfg))
            {
                view = cfg.view;
                return true;
            }

            view = null;
            return false;
        }
    }
}