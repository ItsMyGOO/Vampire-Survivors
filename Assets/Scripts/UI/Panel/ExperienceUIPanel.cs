using ECS;
using ECS.Core;
using Game.Battle;
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
            PlayerContext.Instance.IsInitialized
                .Where(x => x)
                .Take(1)
                .Subscribe(_ => Bind())
                .AddTo(this);
        }

        void Bind()
        {
            var world = PlayerContext.Instance.World;
            var playerId = PlayerContext.Instance.PlayerEntityId;

            Observable.EveryUpdate()
                .Subscribe(_ => Refresh(world, playerId))
                .AddTo(this);
        }

        void Refresh(World world, int playerId)
        {
            if (!world.HasComponent<ExperienceComponent>(playerId))
                return;

            var exp = world.GetComponent<ExperienceComponent>(playerId);

            expBar.value = exp.current_exp / exp.exp_to_next_level;
            levelText.text = $"Lv.{exp.level}";
        }
    }
}