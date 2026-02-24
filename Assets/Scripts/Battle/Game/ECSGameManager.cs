using Battle.Mode;
using Cinemachine;
using Game;
using UnityEngine;

namespace Battle
{
    public class ECSGameManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera vCam;

        private BattleModeController _controller;

        private void Start()
        {
            _controller = new BattleModeController();
            _controller.SwitchTo(new PlaceholderBattleMode());

            GameEvents.OnBattleStartRequested += HandleBattleStartRequested;
        }

        private void Update()
        {
            _controller?.Update(Time.deltaTime);
        }

        private void OnDestroy()
        {
            GameEvents.OnBattleStartRequested -= HandleBattleStartRequested;
            _controller?.Exit();
        }

        /// <summary>
        /// 切换战斗模式，供调试工具或外部逻辑调用。
        /// 内部保证旧模式完整退出后再进入新模式。
        /// </summary>
        public void SwitchMode(IBattleMode mode)
        {
            _controller?.SwitchTo(mode);
        }

        private void HandleBattleStartRequested()
        {
            _controller.SwitchTo(new ECSBattleMode(vCam));
        }
    }
}