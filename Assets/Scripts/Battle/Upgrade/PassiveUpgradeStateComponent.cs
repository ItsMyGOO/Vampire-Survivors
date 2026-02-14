using System;
using System.Collections.Generic;

namespace Battle.Upgrade
{
    /// <summary>
    /// 记录玩家各个被动技能的等级。
    /// 作为 ECS 组件挂在玩家实体上，供升级/抽卡等系统查询。
    /// </summary>
    [Serializable]
    public class PassiveUpgradeStateComponent
    {
        private readonly Dictionary<string, int> _levels = new Dictionary<string, int>();

        /// <summary>
        /// 只读视图，用于 UpgradeService 统计当前被动等级。
        /// </summary>
        public IReadOnlyDictionary<string, int> Levels => _levels;

        public int GetLevel(string passiveId)
        {
            if (string.IsNullOrEmpty(passiveId))
                return 0;

            return _levels.TryGetValue(passiveId, out var level) ? level : 0;
        }

        /// <summary>
        /// 将指定被动提升若干级（默认 +1），返回新等级。
        /// </summary>
        public int AddLevel(string passiveId, int delta = 1)
        {
            if (string.IsNullOrEmpty(passiveId))
                return 0;

            var current = GetLevel(passiveId);
            var next = current + delta;
            _levels[passiveId] = next;
            return next;
        }
    }
}

