using ECS.Core;
using ConfigHandler;
using ECS;
using Battle.Weapon;

namespace Battle.Weapon
{
    /// <summary>
    /// 玩家武器运行时数据初始化
    /// 只负责创建 RuntimeStats，不做数值计算
    /// </summary>
    public static class PlayerWeaponInitializer
    {
        public static void Initialize(World world, int playerId)
        {
            if (!world.TryGetComponent(playerId, out WeaponRuntimeStatsComponent weaponStats))
                return;

            foreach (var weapon in weaponStats.GetAllWeapons())
            {
                if (!WeaponConfigDB.Instance.Data.TryGetValue(
                        weapon.weaponId,
                        out var cfg))
                    continue;

                // 只创建运行时状态 + 设置等级
                if (weapon.level == 0)
                {
                    weapon.level = 1;
                }
            }
        }
    }
}
