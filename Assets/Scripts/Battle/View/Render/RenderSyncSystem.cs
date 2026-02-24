using ECS.Core;

namespace ECS
{
    /// <summary>
    /// Render 同步系统
    /// 只负责：把 ECS 状态推给 RenderSystem
    /// </summary>
    public class RenderSyncSystem
    {
        private readonly RenderSystem renderSystem;

        public RenderSyncSystem(RenderSystem renderSystem)
        {
            this.renderSystem = renderSystem;
        }

            /// <summary>
    /// 销毁所有由 RenderSystem 管理的 GameObject，清空内部状态。
    /// 切换战斗模式时由 IBattleMode.Exit() 调用。
    /// </summary>
    public void DestroyAll() => renderSystem.DestroyAll();

    
public void Update(World world)
        {
            renderSystem.BeginFrame();

            foreach (var (entity, position) in world.GetComponents<PositionComponent>())
            {
                // TryGetComponent 合并 Has + Get 为单次字典查找
                // 原实现：每实体 4×HasComponent + 4×GetComponent = 8 次查找
                // 现实现：每实体 4×TryGetComponent = 4 次查找
                bool hasVel      = world.TryGetComponent<VelocityComponent>(entity,  out var vel);
                bool hasRot      = world.TryGetComponent<RotationComponent>(entity,  out var rot);
                bool hasSpriteKey = world.TryGetComponent<SpriteKeyComponent>(entity, out var spriteKey);
                bool isCameraFollow = world.HasComponent<CameraFollowComponent>(entity);

                renderSystem.RenderEntity(
                    entity,
                    position,
                    vel,       hasVel,
                    rot,       hasRot,
                    spriteKey,
                    isCameraFollow
                );
            }

            renderSystem.EndFrame();
        }
    }
}