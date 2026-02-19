using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI.Core;
using UI.Model;
using UniRx;

namespace UI.Panel
{
    public class BattleHUDPanel : UIBindPanel<HUDViewModel>
    {
        [Header("UI References")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI killCountText;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider expSlider;
        [SerializeField] private TextMeshProUGUI levelText;

        private CompositeDisposable _disposables;

        #region Lifecycle

        protected override void OnInit()
        {
            // pauseButton?.onClick.AddListener(OnPauseButtonClicked);
        }

        protected override void OnViewModelReady()
        {
            _disposables = new CompositeDisposable();

            ViewModel.Level
                .Subscribe(lv => levelText.text = $"Lv.{lv}")
                .AddTo(_disposables);

            ViewModel.ExpRatio
                .Subscribe(v => expSlider.value = Mathf.Clamp01(v))
                .AddTo(_disposables);

            // ViewModel.HealthRatio
            //     .Subscribe(v => healthSlider.value = Mathf.Clamp01(v))
            //     .AddTo(_disposables);

            ViewModel.KillCount
                .Subscribe(v => killCountText.text = $"Kills: {v}")
                .AddTo(_disposables);

            ViewModel.BattleTime
                .Subscribe(UpdateTimeDisplay)
                .AddTo(_disposables);
        }

        protected override void OnViewModelDispose()
        {
            _disposables?.Dispose();
            _disposables = null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            // pauseButton?.onClick.RemoveListener(OnPauseButtonClicked);
        }

        #endregion

        #region UI Logic

        private void UpdateTimeDisplay(float battleTime)
        {
            int minutes = Mathf.FloorToInt(battleTime / 60f);
            int seconds = Mathf.FloorToInt(battleTime % 60f);
            timeText.text = $"{minutes:00}:{seconds:00}";
        }

        private void OnPauseButtonClicked()
        {
            UIManager.Instance?.ShowPanel<PauseMenuPanel>();
        }

        #endregion
    }
}
