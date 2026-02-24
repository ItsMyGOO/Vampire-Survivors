using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panel
{
    /// <summary>
    /// 战斗UI控制器
    /// 包含暂停菜单、退出按钮等
    /// </summary>
    public class BattleUIController : MonoBehaviour
    {
        [Header("Pause Menu")]
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;

        [Header("Battle Info")]
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI killCountText;

        private float battleTime;
        private int killCount;
        private bool isPaused;

        private void Awake()
        {
            // 绑定按钮事件
            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(OnPauseButtonClicked);
            }

            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(OnResumeButtonClicked);
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartButtonClicked);
            }

            if (exitButton != null)
            {
                exitButton.onClick.AddListener(OnExitButtonClicked);
            }

            // 初始状态
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }
        }

        private void Start()
        {
            // 监听敌人死亡事件 (如果有的话)
            // GameEvents.OnEnemyKilled += OnEnemyKilled;
        }

        private void Update()
        {
            if (isPaused)
                return;

            // 更新战斗时间
            battleTime += Time.deltaTime;
            UpdateTimeDisplay();

            // 监听ESC键暂停
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnPauseButtonClicked();
            }
        }

        /// <summary>
        /// 暂停按钮点击
        /// </summary>
        private void OnPauseButtonClicked()
        {
            if (GameSceneManager.Instance != null)
            {
                GameSceneManager.Instance.PauseGame();
            }

            isPaused = true;

            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
            }

            Debug.Log("[BattleUI] 游戏暂停");
        }

        /// <summary>
        /// 继续按钮点击
        /// </summary>
        private void OnResumeButtonClicked()
        {
            if (GameSceneManager.Instance != null)
            {
                GameSceneManager.Instance.ResumeGame();
            }

            isPaused = false;

            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }

            Debug.Log("[BattleUI] 游戏继续");
        }

        /// <summary>
        /// 重新开始按钮点击
        /// </summary>
        private void OnRestartButtonClicked()
        {
            Debug.Log("[BattleUI] 重新开始");

            // 恢复时间缩放
            if (isPaused)
            {
                OnResumeButtonClicked();
            }

            if (GameSceneManager.Instance != null)
            {
                GameSceneManager.Instance.RestartBattle();
            }
        }

        /// <summary>
        /// 退出战斗按钮点击
        /// </summary>
        private void OnExitButtonClicked()
        {
            Debug.Log("[BattleUI] 退出战斗");

            // 恢复时间缩放
            if (isPaused)
            {
                Time.timeScale = 1f;
            }

            if (GameSceneManager.Instance != null)
            {
                GameSceneManager.Instance.ExitBattle();
            }
        }

        /// <summary>
        /// 更新时间显示
        /// </summary>
        private void UpdateTimeDisplay()
        {
            if (timeText != null)
            {
                int minutes = Mathf.FloorToInt(battleTime / 60f);
                int seconds = Mathf.FloorToInt(battleTime % 60f);
                timeText.text = $"{minutes:00}:{seconds:00}";
            }
        }

        /// <summary>
        /// 更新击杀数显示
        /// </summary>
        private void UpdateKillCountDisplay()
        {
            if (killCountText != null)
            {
                killCountText.text = $"Kills: {killCount}";
            }
        }

        /// <summary>
        /// 敌人被击杀回调
        /// </summary>
        private void OnEnemyKilled(int enemyId)
        {
            killCount++;
            UpdateKillCountDisplay();
        }

        private void OnDestroy()
        {
            // 清理按钮事件
            if (pauseButton != null)
            {
                pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
            }

            if (resumeButton != null)
            {
                resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
            }

            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            }

            if (exitButton != null)
            {
                exitButton.onClick.RemoveListener(OnExitButtonClicked);
            }

            // 取消事件监听
            // GameEvents.OnEnemyKilled -= OnEnemyKilled;
        }
    }
}
