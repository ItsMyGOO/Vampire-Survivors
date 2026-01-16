using UnityEngine;
using UnityEngine.UI;
using UI.Core;

namespace UI.Panel
{
    public class PauseMenuPanel : UIPanel
    {
        [Header("UI References")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;

        protected override void OnInit()
        {
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeButtonClicked);
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartButtonClicked);
            if (exitButton != null)
                exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        protected override void OnAfterShow()
        {
            if (Game.GameSceneManager.Instance != null)
                Game.GameSceneManager.Instance.PauseGame();
        }

        private void OnResumeButtonClicked()
        {
            Hide();
            if (Game.GameSceneManager.Instance != null)
                Game.GameSceneManager.Instance.ResumeGame();
        }

        private void OnRestartButtonClicked()
        {
            Hide();
            if (Game.GameSceneManager.Instance != null)
                Game.GameSceneManager.Instance.RestartBattle();
        }

        private void OnExitButtonClicked()
        {
            Time.timeScale = 1f;
            Hide();
            if (Game.GameSceneManager.Instance != null)
                Game.GameSceneManager.Instance.ExitBattle();
        }

        private void OnDestroy()
        {
            if (resumeButton != null)
                resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
            if (restartButton != null)
                restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            if (exitButton != null)
                exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }
    }
}
