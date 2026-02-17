using System.Collections.Generic;
using ECS;
using ECS.Core;

namespace Battle.Upgrade
{
    /// <summary>
    /// 被动状态查询辅助类
    /// 提供查询被动等级的静态方法
    /// 将原来 PassiveUpgradeStateComponent 中的方法移到这里
    /// </summary>
    public static class PassiveStateHelper
    {
        /// <summary>
        /// 获取被动等级
        /// </summary>
        public static int GetLevel(World world, int entityId, string passiveId)
        {
            if (string.IsNullOrEmpty(passiveId))
                return 0;

            if (!world.TryGetComponent<PassiveUpgradeStateComponent>(entityId, out var state))
                return 0;

            return state.levels.TryGetValue(passiveId, out var level) ? level : 0;
        }

        /// <summary>
        /// 获取所有被动等级
        /// </summary>
        public static IReadOnlyDictionary<string, int> GetAllLevels(World world, int entityId)
        {
            if (!world.TryGetComponent<PassiveUpgradeStateComponent>(entityId, out var state))
                return new Dictionary<string, int>();

            return state.levels;
        }

        /// <summary>
        /// 检查被动是否已解锁
        /// </summary>
        public static bool HasPassive(World world, int entityId, string passiveId)
        {
            return GetLevel(world, entityId, passiveId) > 0;
        }

        /// <summary>
        /// 获取已解锁的被动数量
        /// </summary>
        public static int GetUnlockedPassiveCount(World world, int entityId)
        {
            if (!world.TryGetComponent<PassiveUpgradeStateComponent>(entityId, out var state))
                return 0;

            return state.levels.Count;
        }
    }
}
