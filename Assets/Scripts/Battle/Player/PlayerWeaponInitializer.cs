using ECS.Core;
using ConfigHandler;
using ECS;

namespace Battle
{
    /// <summary>
    /// 玩家武器运行时数据初始化
    /// 只负责创建 RuntimeStats，不做数值计算
    /// </summary>
    public static class PlayerWeaponInitializer
    {
        public static void Initialize(World world, int playerId)
        {
            if (!world.TryGetComponent(playerId, out WeaponSlotsComponent slots))
                return;

            // 如果已经存在，避免重复初始化
            if (world.HasComponent<WeaponRuntimeStatsComponent>(playerId))
                return;

            var statsComponent = new WeaponRuntimeStatsComponent();

            foreach (var weapon in slots.weapons)
            {
                if (!WeaponConfigDB.Instance.Data.TryGetValue(
                        weapon.weapon_type,
                        out var cfg))
                    continue;

                // 只创建运行时状态 + 设置等级
                var stats = statsComponent.GetOrCreate(weapon.weapon_type);
                stats.level = weapon.level;
            }

            world.AddComponent(playerId, statsComponent);
        }
    }
}