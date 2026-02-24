using Battle.Mode;
using Cinemachine;
using Session;
using UI.Core;
using UI.Model;
using UI.Panel;
using UnityEngine;

namespace Battle
{
    public class ECSGameManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera vCam;

        private BattleModeController _controller;

private void Start()
        {
            if (!GameSessionData.HasSelection)
                Debug.LogWarning("[ECSGameManager] GameSessionData 中无角色选择，BattleScene 被加载但未经过角色选择流程。玩家实体将使用默认属性创建。");

            _controller = new BattleModeController();
            _controller.SwitchTo(new ECSBattleMode(vCam));
        }

private void Update()
        {
            _controller?.Update(Time.deltaTime);
        }

private void OnDestroy()
        {
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

    }
}