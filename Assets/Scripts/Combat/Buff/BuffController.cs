using System.Collections.Generic;
using Core;

namespace Combat.Buff
{
    public class BuffController : IDamageModifier
    {
        private readonly List<IBuff> _buffs = new();

        public void AddBuff(IBuff buff, ISkillTarget owner)
        {
            _buffs.Add(buff);
            buff.OnAdd(owner);
        }

        public void ApplyDamageModifiers(SkillContext ctx)
        {
            foreach (var buff in _buffs)
                buff.Modify(ctx);
        }
    }
}