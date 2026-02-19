using UniRx;

namespace UI.Model
{
    using UniRx;

    public class HUDViewModel
    {
        // ===== 内部可写 =====
        private readonly ReactiveProperty<int> level = new();
        private readonly ReactiveProperty<float> expRatio = new();
        private readonly ReactiveProperty<float> healthRatio = new();
        private readonly ReactiveProperty<int> killCount = new();
        private readonly ReactiveProperty<float> battleTime = new();

        // ===== 对外只读 =====
        public IReadOnlyReactiveProperty<int> Level => level;
        public IReadOnlyReactiveProperty<float> ExpRatio => expRatio;
        public IReadOnlyReactiveProperty<float> HealthRatio => healthRatio;
        public IReadOnlyReactiveProperty<int> KillCount => killCount;
        public IReadOnlyReactiveProperty<float> BattleTime => battleTime;

        // ===== 供 SyncSystem 调用 =====
        public void SetLevel(int value) => level.Value = value;
        public void SetExpRatio(float value) => expRatio.Value = value;
        public void SetHealthRatio(float value) => healthRatio.Value = value;
        public void SetKillCount(int value) => killCount.Value = value;
        public void SetBattleTime(float value) => battleTime.Value = value;
    }

}