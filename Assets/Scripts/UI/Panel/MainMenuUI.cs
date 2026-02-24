using Game;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panel
{
    /// <summary>
    /// 主菜单UI控制器
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TextMeshProUGUI titleText;
        
        [Header("Settings")]
        [SerializeField] private string gameTitle = "Vampire Survivors";

        private void Awake()
        {
            // 设置标题
            if (titleText != null)
            {
                titleText.text = gameTitle;
            }

            // 绑定按钮事件
            if (startButton != null)
            {
                startButton.onClick.AddListener(OnStartButtonClicked);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuitButtonClicked);
            }
        }

        private void Start()
        {
            // 确保时间缩放正常
            Time.timeScale = 1f;
        }

        /// <summary>
        /// 开始游戏按钮点击
        /// </summary>
        private void OnStartButtonClicked()
        {
            Debug.Log("[MainMenuUI] 开始游戏");
            
            // 禁用按钮防止重复点击
            if (startButton != null)
            {
                startButton.interactable = false;
            }

            // 调用场景管理器开始战斗
            if (GameSceneManager.Instance != null)
            {
                GameSceneManager.Instance.StartBattle();
            }
            else
            {
                Debug.LogError("[MainMenuUI] GameSceneManager 不存在!");
            }
        }

        /// <summary>
        /// 退出游戏按钮点击
        /// </summary>
        private void OnQuitButtonClicked()
        {
            Debug.Log("[MainMenuUI] 退出游戏");

            if (GameSceneManager.Instance != null)
            {
                GameSceneManager.Instance.QuitGame();
            }
            else
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }

        private void OnDestroy()
        {
            // 清理按钮事件
            if (startButton != null)
            {
                startButton.onClick.RemoveListener(OnStartButtonClicked);
            }

            if (quitButton != null)
            {
                quitButton.onClick.RemoveListener(OnQuitButtonClicked);
            }
        }
    }
}
