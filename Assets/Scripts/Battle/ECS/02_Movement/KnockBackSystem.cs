using System.Collections.Generic;
using ECS.Core;

namespace ECS.Systems
{
    /// <summary>
    /// 击退系统 - 直接修改位置版本
    /// </summary>
    public class KnockBackSystem : SystemBase
    {
        private readonly List<int> _toRemove = new List<int>();

        public override void Update(World world, float deltaTime)
        {
            _toRemove.Clear();

            foreach (var (entity, knockback) in world.GetComponents<KnockBackComponent>())
            {
                if (!world.HasComponent<PositionComponent>(entity))
                {
                    _toRemove.Add(entity);
                    continue;
                }

                var position = world.GetComponent<PositionComponent>(entity);
                position.x += knockback.forceX * deltaTime;
                position.y += knockback.forceY * deltaTime;
                world.SetComponent(entity, position);

                var kb = knockback;
                kb.time -= deltaTime;
                world.SetComponent(entity, kb);

                if (kb.time <= 0)
                    _toRemove.Add(entity);
            }

            for (int i = 0; i < _toRemove.Count; i++)
                world.RemoveComponent<KnockBackComponent>(_toRemove[i]);
        }
    }
}