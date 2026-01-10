using System.Collections.Generic;
using ECS.Core;

namespace ECS.Systems
{
    /// <summary>
    /// 动画指令系统
    /// 职责: 执行动画指令
    /// </summary>
    public class AnimationCommandSystem : SystemBase {
        public override void Update(World world, float deltaTime) {
            var toRemove = new List<int>();
        
            foreach (var (entity, command) in world.GetComponents<AnimationCommandComponent>()) {
                if (!world.HasComponent<AnimationComponent>(entity)) {
                    toRemove.Add(entity);
                    continue;
                }
            
                var animation = world.GetComponent<AnimationComponent>(entity);
            
                switch (command.command) {
                    case "play":
                        animation.current = command.anim_name;
                        animation.time = 0f;
                        break;
                    case "stop":
                        animation.speed = 0f;
                        break;
                    case "pause":
                        animation.speed = 0f;
                        break;
                }
            
                // 执行回调
                command.callback?.Invoke();
            
                toRemove.Add(entity);
            }
        
            // 移除已执行的指令
            foreach (var entity in toRemove) {
                world.RemoveComponent<AnimationCommandComponent>(entity);
            }
        }
    }
}