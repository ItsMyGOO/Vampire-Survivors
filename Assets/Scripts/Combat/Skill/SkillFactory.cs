using Combat.Damage;
using Core.Buff;

namespace Combat.Skill
{
    public static class SkillFactory
    {
        public static SkillExecutor CreateDefault()
        {
            return new SkillExecutor(new ISkillStep[]
            {
                new CalculateBaseDamageStep(new DefaultDamageCalculator()),
                new ApplySourceBuffStep(),
                new ApplyTargetBuffStep(),
                new ApplyFinalDamageStep()
            });
        }
    }
}