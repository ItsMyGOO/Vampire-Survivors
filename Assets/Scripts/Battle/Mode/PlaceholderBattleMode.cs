using UI.Core;
using UI.Panel;
using UnityEngine;

namespace Battle.Mode
{
    /// <summary>
    /// 占位战斗模式：BattleScene 加载后的默认模式。
    /// Enter 时显示 StartMenuPanel，Exit 时隐藏。
    /// 点击"开始游戏"后显示 CharacterSelectPanel，等待选角确认。
    /// 确认后由 ECSGameManager 监听 GameEvents.OnBattleStartRequested 完成模式切换。
    /// </summary>
    public class PlaceholderBattleMode : IBattleMode
    {
        private StartMenuPanel _startMenuPanel;

        public void Enter()
        {
            Debug.Log("[PlaceholderBattleMode] Enter");
            var panel = UIManager.Instance?.ShowPanel<StartMenuPanel>(hideOthers: true, addToStack: false);
            if (panel != null)
            {
                _startMenuPanel = panel;
                _startMenuPanel.OnStartClicked += HandleStartClicked;
            }
        }

        public void Update(float dt)
        {
            // 空实现，预留扩展点
        }

        public void Exit()
        {
            Debug.Log("[PlaceholderBattleMode] Exit");
            if (_startMenuPanel != null)
            {
                _startMenuPanel.OnStartClicked -= HandleStartClicked;
                _startMenuPanel = null;
            }
            UIManager.Instance?.HidePanel<StartMenuPanel>();
        }

private void HandleStartClicked()
        {
            UIManager.Instance?.ShowPanel<CharacterSelectPanel>(hideOthers: true, addToStack: false);
        }
    }
}