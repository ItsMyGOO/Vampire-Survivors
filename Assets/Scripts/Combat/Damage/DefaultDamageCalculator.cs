using Core;

namespace Combat.Damage
{
    public class DefaultDamageCalculator : IDamageCalculator
    {
        public int Calculate(SkillContext ctx)
        {
            var raw = ctx.Source.Attack - ctx.Target.Defense;
            return raw > 0 ? raw : 1;
        }
    }
}