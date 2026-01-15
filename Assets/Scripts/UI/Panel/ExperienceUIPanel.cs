using Battle;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace UI.Panel
{
    public class ExperienceUIPanel : MonoBehaviour
    {
        [Header("UI")] public Slider expBar;
        public Text levelText;

        void Start()
        {
            // 等 PlayerContext & Exp 初始化完成
            Observable.EveryUpdate()
                .Where(_ =>
                    PlayerContext.Instance != null &&
                    PlayerContext.Instance.ExpData != null)
                .Take(1)
                .Subscribe(_ => Bind(PlayerContext.Instance.ExpData))
                .AddTo(this);
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
                .Subscribe(v => { expBar.value = Mathf.Clamp01(v); })
                .AddTo(this);
        }
    }
}