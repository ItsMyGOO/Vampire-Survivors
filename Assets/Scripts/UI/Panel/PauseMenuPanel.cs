using Game;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

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
            if (GameSceneManager.Instance != null)
                GameSceneManager.Instance.PauseGame();
        }

        private void OnResumeButtonClicked()
        {
            Hide();
            if (GameSceneManager.Instance != null)
                GameSceneManager.Instance.ResumeGame();
        }

        private void OnRestartButtonClicked()
        {
            Hide();
            if (GameSceneManager.Instance != null)
                GameSceneManager.Instance.RestartBattle();
        }

        private void OnExitButtonClicked()
        {
            Time.timeScale = 1f;
            Hide();
            if (GameSceneManager.Instance != null)
                GameSceneManager.Instance.ExitBattle();
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
