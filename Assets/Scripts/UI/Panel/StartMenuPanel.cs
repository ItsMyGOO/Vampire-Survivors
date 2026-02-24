using System;
using TMPro;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panel
{
    /// <summary>
    /// 开始菜单面板 — 在 PlaceholderBattleMode 期间显示。
    /// 提供"开始游戏"入口，点击后触发 OnStartClicked 事件。
    /// </summary>
    public class StartMenuPanel : UIPanel
    {
        [Header("UI References")]
        [SerializeField] private Button startButton;
        [SerializeField] private TextMeshProUGUI titleText;

        [Header("Settings")]
        [SerializeField] private string gameTitle = "Vampire Survivors";

        /// <summary>点击"开始游戏"时触发</summary>
        public event Action OnStartClicked;

        protected override void OnInit()
        {
            if (titleText != null)
                titleText.text = gameTitle;

            if (startButton != null)
                startButton.onClick.AddListener(HandleStartClicked);
        }

        protected override void OnAfterShow()
        {
            Time.timeScale = 1f;
            SetButtonInteractable(true);
        }

        private void HandleStartClicked()
        {
            SetButtonInteractable(false);
            OnStartClicked?.Invoke();
        }

        private void SetButtonInteractable(bool interactable)
        {
            if (startButton != null)
                startButton.interactable = interactable;
        }

        private void OnDestroy()
        {
            if (startButton != null)
                startButton.onClick.RemoveListener(HandleStartClicked);
        }
    }
}
