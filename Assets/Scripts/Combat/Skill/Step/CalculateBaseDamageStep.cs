using Combat.Damage;
using Core.Buff;

namespace Combat.Skill
{
    public class CalculateBaseDamageStep : ISkillStep
    {
        private readonly IDamageCalculator _calculator;

        public CalculateBaseDamageStep(IDamageCalculator calculator)
        {
            _calculator = calculator;
        }

        public void Execute(SkillContext ctx)
        {
            ctx.Damage.FinalDamage = _calculator.Calculate(ctx);
        }
    }
}