using ECS.Core;

namespace ECS.Systems
{
    /// <summary>
    /// 动画系统
    /// 职责: 更新动画状态
    /// </summary>
    public class AnimationSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            foreach (var (entity, animation) in world.GetComponents<AnimationComponent>())
            {
                animation.time += deltaTime * animation.speed;

                // TODO: 实际的帧更新逻辑
                // 这里需要根据 animation_db.lua 来更新帧
            }
        }
    }
}