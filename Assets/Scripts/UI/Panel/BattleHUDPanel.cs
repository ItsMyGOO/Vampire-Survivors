using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI.Core;
using UI.Model;
using UniRx;

namespace UI.Panel
{
    public class BattleHUDPanel : UIPanel
    {
        [Header("UI References")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI killCountText;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider expSlider;
        [SerializeField] private TextMeshProUGUI levelText;

        private CompositeDisposable _disposables;
        private HUDViewModel _viewModel;

        #region Lifecycle

        protected override void OnInit()
        {
            pauseButton?.onClick.AddListener(OnPauseButtonClicked);
        }

        protected override void OnAfterShow()
        {
            base.OnAfterShow();

            _viewModel = ViewModelRegistry.Get<HUDViewModel>();

            if (_viewModel != null)
                SubscribeViewModel();
        }

        protected override void OnBeforeHide()
        {
            DisposeSubscriptions();
        }

        private void OnDestroy()
        {
            DisposeSubscriptions();
            pauseButton?.onClick.RemoveListener(OnPauseButtonClicked);
        }

        #endregion

        #region Binding

        private void SubscribeViewModel()
        {
            DisposeSubscriptions();

            _disposables = new CompositeDisposable();

            _viewModel.Level
                .Subscribe(lv => levelText.text = $"Lv.{lv}")
                .AddTo(_disposables);

            _viewModel.ExpRatio
                .Subscribe(v => expSlider.value = Mathf.Clamp01(v))
                .AddTo(_disposables);

            _viewModel.HealthRatio
                .Subscribe(v => healthSlider.value = Mathf.Clamp01(v))
                .AddTo(_disposables);

            _viewModel.KillCount
                .Subscribe(v => killCountText.text = $"Kills: {v}")
                .AddTo(_disposables);

            _viewModel.BattleTime
                .Subscribe(UpdateTimeDisplay)
                .AddTo(_disposables);
        }

        private void DisposeSubscriptions()
        {
            _disposables?.Dispose();
            _disposables = null;
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
