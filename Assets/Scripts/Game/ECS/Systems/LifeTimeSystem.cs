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
        public override void Update(World world, float deltaTime)
        {
            var toDestroy = new List<int>();

            foreach (var (entity, lifetime) in world.GetComponents<LifeTimeComponent>())
            {
                lifetime.elapsed += deltaTime;

                if (lifetime.elapsed >= lifetime.duration)
                {
                    toDestroy.Add(entity);
                }
            }

            foreach (var entity in toDestroy)
            {
                world.DestroyEntity(entity);
            }
        }
    }
}