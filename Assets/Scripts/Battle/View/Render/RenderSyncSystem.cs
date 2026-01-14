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

        public void Update(World world)
        {
            renderSystem.BeginFrame();

            // 以 Position 作为 render 主键（等价你之前的 Collect）
            foreach (var (entity, position) in world.GetComponents<PositionComponent>())
            {
                VelocityComponent velocity = null;
                RotationComponent rotation = null;
                SpriteKeyComponent spriteKey = null;

                if (world.HasComponent<VelocityComponent>(entity))
                    velocity = world.GetComponent<VelocityComponent>(entity);

                if (world.HasComponent<RotationComponent>(entity))
                    rotation = world.GetComponent<RotationComponent>(entity);

                if (world.HasComponent<SpriteKeyComponent>(entity))
                    spriteKey = world.GetComponent<SpriteKeyComponent>(entity);

                bool isCameraFollow =
                    world.HasComponent<CameraFollowComponent>(entity);
                
                renderSystem.RenderEntity(
                    entity,
                    position,
                    velocity,
                    rotation,
                    spriteKey,
                    isCameraFollow
                );
            }

            renderSystem.EndFrame();
        }
    }
}