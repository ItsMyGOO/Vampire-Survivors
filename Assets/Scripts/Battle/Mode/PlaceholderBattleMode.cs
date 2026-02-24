using UnityEngine;

namespace Battle.Mode
{
    /// <summary>
    /// 占位战斗模式，作为第二种模式的骨架实现。
    /// 结构完整，后续功能可在此文件上直接扩展，不影响其他模式。
    /// </summary>
    public class PlaceholderBattleMode : IBattleMode
    {
        public void Enter()
        {
            Debug.Log("[PlaceholderBattleMode] Enter — 占位模式已启动，暂无游戏逻辑。");
        }

        public void Update(float dt)
        {
            // 空实现，预留扩展点
        }

        public void Exit()
        {
            Debug.Log("[PlaceholderBattleMode] Exit — 占位模式已退出。");
        }
    }
}
