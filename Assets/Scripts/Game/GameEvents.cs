using System;

namespace Game
{
    /// <summary>
    /// 全局静态事件总线，用于解耦跨层通信。
    /// 订阅方在 OnDestroy/Exit 中务必取消订阅，避免内存泄漏。
    /// </summary>
    public static class GameEvents
    {
        /// <summary>
        /// 选角确认后触发，请求切换至 ECSBattleMode。
        /// 由 CharacterSelectPanel 在写入 GameSessionData 后 invoke。
        /// 由 ECSGameManager 订阅并执行模式切换。
        /// </summary>
        public static event Action OnBattleStartRequested;

        public static void RequestBattleStart()
        {
            OnBattleStartRequested?.Invoke();
        }

        /// <summary>清除所有订阅（谨慎使用，仅供场景完全重置时调用）</summary>
        public static void ClearAll()
        {
            OnBattleStartRequested = null;
        }
    }
}