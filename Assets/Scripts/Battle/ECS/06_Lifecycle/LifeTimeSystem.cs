using System.Collections.Generic;
using ECS.Core;

namespace ECS.Systems
{
    /// <summary>
    /// 生命周期系统
    /// 职责: 处理有时限的实体
    /// </summary>
    public class LifeTimeSystem : SystemBase
    {
        private readonly List<int> _toDestroy = new List<int>();

        public override void Update(World world, float deltaTime)
        {
            _toDestroy.Clear();

            foreach (var (entity, lifetime) in world.GetComponents<LifeTimeComponent>())
            {
                var lt = lifetime;
                lt.elapsed += deltaTime;
                world.SetComponent(entity, lt);

                if (lt.elapsed >= lt.duration)
                    _toDestroy.Add(entity);
            }

            for (int i = 0; i < _toDestroy.Count; i++)
                world.DestroyEntity(_toDestroy[i]);
        }
    }
}