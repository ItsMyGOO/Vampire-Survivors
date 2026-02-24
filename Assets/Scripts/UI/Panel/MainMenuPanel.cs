using Game;
using TMPro;
using UI.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panel
{
    public class MainMenuPanel : UIPanel
    {
        [Header("UI References")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TextMeshProUGUI titleText;
        
        [Header("Settings")]
        [SerializeField] private string gameTitle = "Vampire Survivors";

        protected override void OnInit()
        {
            if (titleText != null)
                titleText.text = gameTitle;

            if (startButton != null)
                startButton.onClick.AddListener(OnStartButtonClicked);

            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        protected override void OnAfterShow()
        {
            Time.timeScale = 1f;
            SetButtonsInteractable(true);
        }

private void OnStartButtonClicked()
        {
            SetButtonsInteractable(false);

            // 直接在同一 Canvas 内显示 CharacterSelectPanel，不再跳场景
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPanel<CharacterSelectPanel>(hideOthers: true, addToStack: true);
            }
            else
            {
                Debug.LogError("[MainMenuPanel] UIManager 不存在");
                SetButtonsInteractable(true);
            }
        }

        private void OnQuitButtonClicked()
        {
            if (GameSceneManager.Instance != null)
                GameSceneManager.Instance.QuitGame();
            else
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }

        private void SetButtonsInteractable(bool interactable)
        {
            if (startButton != null)
                startButton.interactable = interactable;
            if (quitButton != null)
                quitButton.interactable = interactable;
        }

        private void OnDestroy()
        {
            if (startButton != null)
                startButton.onClick.RemoveListener(OnStartButtonClicked);
            if (quitButton != null)
                quitButton.onClick.RemoveListener(OnQuitButtonClicked);
        }
    }
}
