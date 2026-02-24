using System;

namespace Battle.Mode
{
    /// <summary>
    /// 战斗模式接口。
    ///
    /// 生命周期：
    ///   Enter() → Update() × N → Exit()
    ///
    /// 实现者职责：
    ///   Enter  — 创建本模式所需的所有数据（ECS World、组件、View 对象等）
    ///   Update — 每帧驱动本模式的逻辑与渲染同步
    ///   Exit   — 销毁本模式创建的一切：
    ///              · ECS World / 所有实体组件
    ///              · RenderSystem 中的所有 View 对象（GameObject）
    ///              · RenderObjectPool 中的缓存对象
    ///              · 任何注册到外部系统的回调或服务
    ///            Exit 返回后，场景中不得残留任何由本模式创建的 GameObject。
    ///
    /// 切换顺序（由 BattleModeController 保证）：
    ///   旧模式.Exit() 完成后，才会调用新模式.Enter()。
    /// </summary>
    public interface IBattleMode
    {
        /// <summary>进入本模式，完成全部初始化。</summary>
        void Enter();

        /// <summary>每帧驱动，由 BattleModeController 调用。</summary>
        void Update(float deltaTime);

        /// <summary>
        /// 退出本模式，完整清理所有数据与 View 对象。
        /// 实现必须幂等：多次调用不得抛出异常。
        /// </summary>
        void Exit();
    }
}