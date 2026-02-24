using System.Collections.Generic;
using ECS.Core;

namespace ECS.Systems
{
    /// <summary>
    /// 命中冷却系统（无敌帧）
    /// 职责：每帧递减 HitCooldownComponent.timer，到期后移除组件。
    ///
    /// HitCooldownComponent 由 AttackHitSystem 在命中瞬间添加，
    /// 存在期间 AttackHitSystem 会跳过该敌人的伤害判定，
    /// 防止同一帧多颗子弹或连续帧重复命中同一敌人造成伤害爆炸。
    ///
    /// 执行顺序：必须在 AttackHitSystem 之前。
    /// </summary>
    public class HitCooldownSystem : SystemBase
    {
        private readonly List<int> _expired = new List<int>();

        public override void Update(World world, float deltaTime)
        {
            _expired.Clear();

            world.IterateComponents(out int[] ids, out HitCooldownComponent[] data, out int count);

            for (int i = 0; i < count; i++)
            {
                data[i].timer += deltaTime;
                if (data[i].timer >= data[i].duration)
                    _expired.Add(ids[i]);
            }

            // 将修改写回存储（struct 必须 SetComponent）
            // IterateComponents 直接持有内部数组引用，data[i] 的修改已写入 ComponentStore，
            // 但为避免依赖内部实现细节，显式 SetComponent 更安全
            for (int i = 0; i < count; i++)
                world.SetComponent(ids[i], data[i]);

            for (int i = 0; i < _expired.Count; i++)
                world.RemoveComponent<HitCooldownComponent>(_expired[i]);
        }
    }
}
