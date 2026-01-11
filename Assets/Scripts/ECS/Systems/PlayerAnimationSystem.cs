using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    /// <summary>
    /// 玩家动画系统
    /// 职责: 根据速度切换玩家动画
    /// </summary>
    public class PlayerAnimationSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            foreach (var (entity, _) in world.GetComponents<PlayerTagComponent>())
            {
                if (!world.HasComponent<VelocityComponent>(entity) ||
                    !world.HasComponent<AnimationComponent>(entity))
                {
                    continue;
                }

                var velocity = world.GetComponent<VelocityComponent>(entity);
                var animation = world.GetComponent<AnimationComponent>(entity);

                float speed = Mathf.Sqrt(velocity.x * velocity.x + velocity.y * velocity.y);

                var desireState = speed > 0.1f ? "Run" : "Idle";

                if (!string.Equals(desireState, animation.State))
                    continue;
                animation.State = desireState;
                animation.Playing = true;
                animation.ClipId = animation.State;
                animation.Frame = 1;
                animation.Time = 0f;
            }
        }
    }
}