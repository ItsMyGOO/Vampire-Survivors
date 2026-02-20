using System.Collections.Generic;
using ECS.Core;

namespace ECS.Systems
{
    /// <summary>
    /// 动画指令系统
    /// 职责: 执行动画指令
    /// </summary>
    public class AnimationCommandSystem : SystemBase
    {
        // 预分配复用列表，避免每帧 new List 分配
        private readonly List<int> _toRemove = new List<int>();

        public override void Update(World world, float deltaTime)
        {
            _toRemove.Clear();

            foreach (var (entity, command) in world.GetComponents<AnimationCommandComponent>())
            {
                if (!world.HasComponent<AnimationComponent>(entity))
                {
                    _toRemove.Add(entity);
                    continue;
                }

                var animation = world.GetComponent<AnimationComponent>(entity);

                switch (command.command)
                {
                    case "play":
                        animation.AnimName = command.anim_name;
                        animation.Frame = 1;
                        animation.Time = 0f;
                        animation.Playing = true;
                        break;
                    case "stop":
                        animation.Playing = false;
                        break;
                }

                _toRemove.Add(entity);
            }

            for (int i = 0; i < _toRemove.Count; i++)
                world.RemoveComponent<AnimationCommandComponent>(_toRemove[i]);
        }
    }
}