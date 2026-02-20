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

            foreach (var (entity, position) in world.GetComponents<PositionComponent>())
            {
                bool hasVel      = world.HasComponent<VelocityComponent>(entity);
                bool hasRot      = world.HasComponent<RotationComponent>(entity);
                bool hasSpriteKey = world.HasComponent<SpriteKeyComponent>(entity);

                var vel      = hasVel      ? world.GetComponent<VelocityComponent>(entity) : default;
                var rot      = hasRot      ? world.GetComponent<RotationComponent>(entity) : default;
                var spriteKey = hasSpriteKey ? world.GetComponent<SpriteKeyComponent>(entity) : null;

                bool isCameraFollow = world.HasComponent<CameraFollowComponent>(entity);

                renderSystem.RenderEntity(
                    entity,
                    position,
                    vel,      hasVel,
                    rot,      hasRot,
                    spriteKey,
                    isCameraFollow
                );
            }

            renderSystem.EndFrame();
        }
    }
}