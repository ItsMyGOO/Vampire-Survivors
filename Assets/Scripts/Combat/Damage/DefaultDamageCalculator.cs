using Core.Buff;

namespace Combat.Damage
{
    public class DefaultDamageCalculator : IDamageCalculator
    {
        public int Calculate(SkillContext ctx)
        {
            var raw = ctx.Caster.Attack - ctx.Target.Defense;
            return raw > 0 ? raw : 1;
        }
    }
}