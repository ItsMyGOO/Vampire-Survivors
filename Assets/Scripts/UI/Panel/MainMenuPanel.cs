using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI.Core;

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
            
            if (Game.GameSceneManager.Instance != null)
                Game.GameSceneManager.Instance.LoadCharacterSelect();
        }

        private void OnQuitButtonClicked()
        {
            if (Game.GameSceneManager.Instance != null)
                Game.GameSceneManager.Instance.QuitGame();
            else
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
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
