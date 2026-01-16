using Battle.Player;
using Battle.Upgrade;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI.Core;
using UniRx;

namespace UI.Panel
{
    public class BattleHUDPanel : UIPanel
    {
        [Header("UI References")] [SerializeField]
        private Button pauseButton;

        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI killCountText;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider expSlider;
        [SerializeField] private Text levelText;

        private float battleTime = 0f;
        private int killCount = 0;

        protected override void OnInit()
        {
            if (pauseButton != null)
                pauseButton.onClick.AddListener(OnPauseButtonClicked);

            UpdateTimeDisplay();
            UpdateKillCountDisplay();
        }

        protected override void OnStart()
        {
           Bind(PlayerContext.Instance.ExpSystem.ExpData);
        }

        void Bind(ExpData expData)
        {
            // 等级显示
            expData.level
                .Subscribe(lv => { levelText.text = $"Lv.{lv}"; })
                .AddTo(this);

            // 经验条：current / nextLevel
            Observable.CombineLatest(
                    expData.current_exp,
                    expData.exp_to_next_level,
                    (cur, max) => max > 0f ? cur / max : 0f)
                .Subscribe(v => { expSlider.value = Mathf.Clamp01(v); })
                .AddTo(this);
        }

        private void Update()
        {
            if (!IsVisible) return;

            battleTime += Time.deltaTime;
            UpdateTimeDisplay();
        }

        private void OnPauseButtonClicked()
        {
            if (UIManager.Instance != null)
                UIManager.Instance.ShowPanel<PauseMenuPanel>();
        }

        private void UpdateTimeDisplay()
        {
            if (timeText != null)
            {
                int minutes = Mathf.FloorToInt(battleTime / 60f);
                int seconds = Mathf.FloorToInt(battleTime % 60f);
                timeText.text = $"{minutes:00}:{seconds:00}";
            }
        }

        private void UpdateKillCountDisplay()
        {
            if (killCountText != null)
                killCountText.text = $"Kills: {killCount}";
        }

        public void UpdateHealth(float current, float max)
        {
            if (healthSlider != null)
                healthSlider.value = current / max;
        }

        public void UpdateExp(float current, float max)
        {
            if (expSlider != null)
                expSlider.value = current / max;
        }

        public void AddKill()
        {
            killCount++;
            UpdateKillCountDisplay();
        }

        protected override void OnAfterShow()
        {
            battleTime = 0f;
            killCount = 0;
            UpdateTimeDisplay();
            UpdateKillCountDisplay();
        }

        private void OnDestroy()
        {
            if (pauseButton != null)
                pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
        }
    }
}