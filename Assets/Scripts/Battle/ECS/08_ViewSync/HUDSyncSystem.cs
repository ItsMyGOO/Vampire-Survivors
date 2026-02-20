using Battle.Upgrade;
using ECS.Core;
using UI.Model;
using UnityEngine;

namespace ECS.SyncSystems
{
    /// <summary>
    /// HUD 同步系统
    /// 职责：每帧将 ECS 状态推送到 HUDViewModel，驱动 UI 响应式更新。
    ///
    /// 数据来源：
    ///   - BattleTime  : 累计 deltaTime
    ///   - Level/Exp   : ExpSystem.ExpData（首次 Update 时从服务注册表缓存）
    ///   - HealthRatio : HealthComponent / HealthComponent.max
    /// </summary>
    public class HUDSyncSystem : ISystem
    {
        private readonly int _playerId;
        private readonly HUDViewModel _vm;

        private float _battleTime;

        // ExpSystem 在 UpgradeWorldInstaller 注册，首次 Update 时懒获取并缓存
        private ExpSystem _expSystem;

        public HUDSyncSystem(int playerId, HUDViewModel vm)
        {
            _playerId = playerId;
            _vm = vm;
        }

        public void Update(World world, float deltaTime)
        {
            // 懒获取 ExpSystem（只查一次字典）
            if (_expSystem == null)
                world.TryGetService<ExpSystem>(out _expSystem);

            SyncBattleTime(deltaTime);
            SyncLevelAndExp();
            SyncHealth(world);
        }

        private void SyncBattleTime(float deltaTime)
        {
            _battleTime += deltaTime;
            _vm.SetBattleTime(_battleTime);
        }

        private void SyncLevelAndExp()
        {
            if (_expSystem == null) return;

            var data = _expSystem.ExpData;
            _vm.SetLevel(data.level.Value);

            float expRatio = data.exp_to_next_level.Value > 0f
                ? data.current_exp.Value / data.exp_to_next_level.Value
                : 0f;
            _vm.SetExpRatio(Mathf.Clamp01(expRatio));
        }

        private void SyncHealth(World world)
        {
            if (!world.TryGetComponent<HealthComponent>(_playerId, out var health)) return;
            if (health.max <= 0f) return;

            _vm.SetHealthRatio(Mathf.Clamp01(health.current / health.max));
        }
    }
}