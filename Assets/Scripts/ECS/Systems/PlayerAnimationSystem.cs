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

                if (speed > 0.1f)
                {
                    animation.current = "run";
                }
                else
                {
                    animation.current = "idle";
                }
            }
        }
    }
}