using ECS.Core;
using Game.Battle;
using UnityEngine;

namespace Battle
{
    public static class UpgradeApplyService
    {
        public static void Apply(UpgradeOption option)
        {
            var world = PlayerContext.Instance.World;
            int player = PlayerContext.Instance.PlayerEntity;

            switch (option.type)
            {
                case UpgradeOptionType.Weapon:
                    ApplyWeapon(world, player, option.id);
                    break;

                case UpgradeOptionType.Passive:
                    ApplyPassive(world, player, option.id);
                    break;
            }
        }

        private static void ApplyWeapon(World world, int player, string weaponId)
        {
            // world.AddComponent(player, new AddWeaponRequest
            // {
            //     weaponId = weaponId
            // });
            Debug.Log($"[UpgradeApplyService] 添加武器 - {weaponId}");
        }

        private static void ApplyPassive(World world, int player, string passiveId)
        {
            // world.AddComponent(player, new AddPassiveRequest
            // {
            //     passiveId = passiveId
            // });
            Debug.Log($"[UpgradeApplyService] 添加被动技能 - {passiveId}");
        }
    }
}