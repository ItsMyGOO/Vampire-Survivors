using System.Collections.Generic;

namespace Battle.Upgrade
{
    /// <summary>
    /// 玩家升级状态（Weapon + Passive）
    /// 仅作为 UpgradeService 的查询数据
    /// </summary>
    public sealed class PlayerUpgradeState
    {
        /// <summary>
        /// weaponId -> level
        /// </summary>
        public readonly Dictionary<string, int> weapons =
            new Dictionary<string, int>();

        /// <summary>
        /// passiveId -> level
        /// </summary>
        public readonly Dictionary<string, int> passives =
            new Dictionary<string, int>();

        // =========================
        // Query
        // =========================

        public bool HasWeapon(string weaponId)
            => weapons.ContainsKey(weaponId);

        public bool HasPassive(string passiveId)
            => passives.ContainsKey(passiveId);

        public int GetWeaponLevel(string weaponId)
            => weapons.TryGetValue(weaponId, out var lv) ? lv : 0;

        public int GetPassiveLevel(string passiveId)
            => passives.TryGetValue(passiveId, out var lv) ? lv : 0;

        // =========================
        // Mutate（只给 ApplySystem 用）
        // =========================

        public void AddOrUpgradeWeapon(string weaponId)
        {
            if (weapons.TryGetValue(weaponId, out var lv))
                weapons[weaponId] = lv + 1;
            else
                weapons[weaponId] = 1;
        }

        public void AddOrUpgradePassive(string passiveId)
        {
            if (passives.TryGetValue(passiveId, out var lv))
                passives[passiveId] = lv + 1;
            else
                passives[passiveId] = 1;
        }

        public static IReadOnlyDictionary<string, int> GetWeapons(int playerEntity)
        {
            throw new System.NotImplementedException();
        }

        public static IReadOnlyDictionary<string, int> GetPassives(int playerEntity)
        {
            throw new System.NotImplementedException();
        }
    }
}