using UnityEngine;

namespace Battle.Mode
{
    /// <summary>
    /// 战斗模式切换调度器。
    /// 负责生命周期调度，保证切换顺序：旧模式.Exit() 严格在新模式.Enter() 之前。
    /// 本类无任何游戏逻辑，只做生命周期管理。
    /// </summary>
    public class BattleModeController
    {
        private IBattleMode _currentMode;

        /// <summary>
        /// 切换到新模式（同步操作）。
        /// 先完整退出当前模式，再进入新模式，保证不产生残留对象。
        /// </summary>
        public void SwitchTo(IBattleMode newMode)
        {
            if (newMode == null)
            {
                Debug.LogWarning("[BattleModeController] SwitchTo 传入了 null 模式，忽略。");
                return;
            }

            _currentMode?.Exit();
            _currentMode = newMode;
            _currentMode.Enter();
        }

        /// <summary>
        /// 每帧驱动当前模式。
        /// </summary>
        public void Update(float dt)
        {
            _currentMode?.Update(dt);
        }

        /// <summary>
        /// 退出当前模式并清空，通常在 OnDestroy 时调用。
        /// </summary>
        public void Exit()
        {
            _currentMode?.Exit();
            _currentMode = null;
        }
    }
}
