using Combat.Buff.Damage;
using Core.Buff;

namespace Combat.Buff
{
    public static class BuffFactory
    {
        public static IBuff CreateIncreaseDamage(int value)
        {
            return new IncreaseDamageBuff(value);
        }

        public static IBuff CreateReduceDamage(int value)
        {
            return new ReduceDamageBuff(value);
        }
    }
}