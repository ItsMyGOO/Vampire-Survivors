using Battle.Upgrade;
using Cinemachine;
using UI.Core;
using UI.Model;
using UI.Panel;
using UnityEngine;

namespace Battle
{
    public class ECSGameManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera vCam;

        private BattleWorldContext _context;

        private void Start()
        {
            _context = BattleGameBuilder.Build(vCam);
            UIManager.Instance.ShowPanel<BattleHUDPanel>();

            // 将升级服务推送给 UI 面板，避免 UI 主动拉取全局状态
            if (_context.TryGetUpgradeService(out var upgradeService) &&
                _context.TryGetUpgradeApplyService(out var applyService))
            {
                var upgradePanel = UIManager.Instance.GetPanel<UpgradeSelectPanel>();
                upgradePanel?.BindServices(upgradeService, applyService);
            }

            // 初始化调试控制器（如果存在）
            var debugController = GetComponent<ECSGameDebugController>();
            debugController?.Init(_context.PlayerContext);
        }

        private void Update()
        {
            if (_context == null) return;

            _context.World.Update(Time.deltaTime);
            _context.RenderSyncSystem.Update(_context.World);
        }

        private void OnDestroy()
        {
            _context?.Dispose();
            ViewModelRegistry.Unregister<HUDViewModel>();
        }
    }
}