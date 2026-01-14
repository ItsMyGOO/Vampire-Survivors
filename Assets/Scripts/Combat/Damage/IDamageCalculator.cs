using Core.Buff;

namespace Combat.Damage
{
    public interface IDamageCalculator
    {
        int Calculate(SkillContext ctx);
    }
}