using Core;

namespace Combat.Damage
{
    public interface IDamageCalculator
    {
        int Calculate(SkillContext ctx);
    }
}