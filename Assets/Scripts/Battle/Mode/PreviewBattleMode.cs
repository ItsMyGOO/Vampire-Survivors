using Battle.Game;
using Battle.View;
using Cinemachine;
using ECS;
using ECS.Core;
using ECS.Systems;
using UI.Core;
using UI.Panel;
using UnityEngine;

namespace Battle.Mode
{
    /// <summary>
    /// 预览战斗模式：战斗场景加载后的默认模式，替代 PlaceholderBattleMode。
    /// Enter  — 创建轻量 World（仅 WanderSystem + MovementSystem），生成预览敌人，显示 StartMenuPanel。
    /// Update — 驱动 World 逻辑与 RenderSyncSystem 同步。
    /// Exit   — 销毁所有预览实体与 View 对象，隐藏 UI，实现幂等。
    /// </summary>
    public class PreviewBattleMode : IBattleMode
    {
        private readonly CinemachineVirtualCamera _vCam;

        private World _world;
        private RenderSyncSystem _renderSyncSystem;
        private StartMenuPanel _startMenuPanel;
        private bool _exited;

        public PreviewBattleMode(CinemachineVirtualCamera vCam)
        {
            _vCam = vCam;
        }

        public void Enter()
        {
            Debug.Log("[PreviewBattleMode] Enter");

            // 确保配置已加载（PreviewMode 可能先于 BattleGameBuilder 运行）
            GameConfigLoader.LoadAll();

            // 创建轻量 World，只注册游荡和移动两个系统，不引入任何战斗逻辑
            _world = new World();
            _world.RegisterSystem(new WanderSystem());
            _world.RegisterSystem(new MovementSystem());
            _world.RegisterSystem(new AnimationSystem());

            // 生成预览敌人
            PreviewEnemySpawner.Spawn(_world);

            // 创建 RenderSyncSystem（不需要相机跟随，不注册 OnCameraTargetChanged）
            var renderSystem = new RenderSystem(
                new SpriteProvider(),
                new RenderObjectPool()
            );
            _renderSyncSystem = new RenderSyncSystem(renderSystem);

            // 显示开始菜单并注册回调
            var panel = UIManager.Instance?.ShowPanel<StartMenuPanel>(hideOthers: true, addToStack: false);
            if (panel != null)
            {
                _startMenuPanel = panel;
                _startMenuPanel.OnStartClicked += HandleStartClicked;
            }
        }

        public void Update(float deltaTime)
        {
            if (_exited) return;

            _world?.Update(deltaTime);
            _renderSyncSystem?.Update(_world);
        }

        public void Exit()
        {
            if (_exited) return;
            _exited = true;

            Debug.Log("[PreviewBattleMode] Exit");

            // 先销毁所有 View GameObject
            _renderSyncSystem?.DestroyAll();
            _renderSyncSystem = null;

            // 清理 ECS World
            _world?.Clear();
            _world = null;

            // 取消 UI 回调并隐藏面板
            if (_startMenuPanel != null)
            {
                _startMenuPanel.OnStartClicked -= HandleStartClicked;
                _startMenuPanel = null;
            }

            UIManager.Instance?.HidePanel<StartMenuPanel>();
        }

        private void HandleStartClicked()
        {
            UIManager.Instance?.ShowPanel<CharacterSelectPanel>(hideOthers: true, addToStack: false);
        }
    }
}