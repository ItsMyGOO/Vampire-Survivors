using System;
using System.Collections.Generic;

namespace ECS
{
    /// <summary>
    /// 被动升级状态组件 - 纯数据
    /// 记录玩家各个被动技能的等级
    /// </summary>
    [Serializable]
    public class PassiveUpgradeStateComponent
    {
        // 被动ID -> 等级
        public Dictionary<string, int> levels;

        public static PassiveUpgradeStateComponent Create()
        {
            return new PassiveUpgradeStateComponent
            {
                levels = new Dictionary<string, int>()
            };
        }
    }

    /// <summary>
    /// 被动升级意图组件
    /// 当需要升级被动时，添加此组件触发系统处理
    /// </summary>
    [Serializable]
    public class PassiveUpgradeIntentComponent
    {
        public string passiveId;
        public int levelDelta; // 通常为 +1

        public PassiveUpgradeIntentComponent()
        {
            
        }

        public PassiveUpgradeIntentComponent(string passiveId, int levelDelta = 1)
        {
            this.passiveId = passiveId;
            this.levelDelta = levelDelta;
        }
    }
}
