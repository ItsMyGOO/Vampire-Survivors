using Cinemachine;
using UI.Core;
using UI.Model;
using UI.Panel;
using UnityEngine;

namespace Battle.Mode
{
    /// <summary>
    /// 将现有 ECS 战斗流程封装为 IBattleMode 实现。
    /// Enter()  — 调用 BattleGameBuilder.Build()，初始化完整 ECS World 和 View 层
    /// Update() — 驱动 World.Update 和 RenderSyncSystem.Update
    /// Exit()   — 调用 BattleWorldContext.Dispose() 并销毁所有 View GameObject
    /// 现有战斗逻辑零改动，仅套一层 IBattleMode 包装。
    /// </summary>
    public class ECSBattleMode : IBattleMode
    {
        private readonly CinemachineVirtualCamera _vCam;
        private BattleWorldContext _context;

        public ECSBattleMode(CinemachineVirtualCamera vCam)
        {
            _vCam = vCam;
        }

        public void Enter()
        {
            _context = BattleGameBuilder.Build(_vCam);

            UIManager.Instance.ShowPanel<BattleHUDPanel>(hideOthers: true);

            if (_context.TryGetUpgradeService(out var upgradeService) &&
                _context.TryGetUpgradeApplyService(out var applyService))
            {
                var upgradePanel = UIManager.Instance.GetPanel<UpgradeSelectPanel>();
                upgradePanel?.BindServices(upgradeService, applyService);
            }
        }

        public void Update(float dt)
        {
            if (_context == null) return;

            _context.World.Update(dt);
            _context.RenderSyncSystem.Update(_context.World);
        }

        public void Exit()
        {
            if (_context == null) return;

            // 先销毁所有 View GameObject（active + pool 缓存）
            _context.RenderSyncSystem.DestroyAll();

            // 再清理 ECS World 数据
            _context.Dispose();

            ViewModelRegistry.Unregister<HUDViewModel>();

            _context = null;
        }
    }
}
