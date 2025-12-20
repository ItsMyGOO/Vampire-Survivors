using Core;

namespace Combat.Buff.Damage
{
    public class ReduceDamageBuff : IBuff
    {
        private readonly int _value;

        public ReduceDamageBuff(int value)
        {
            _value = value;
        }

        public void ApplyDamageModifiers(SkillContext ctx)
        {
            ctx.Damage.FinalDamage -= _value;
        }

        public void OnAdd(ISkillTarget owner)
        {
            throw new System.NotImplementedException();
        }

        public void OnRemove()
        {
            throw new System.NotImplementedException();
        }

        public void Modify(SkillContext ctx)
        {
            throw new System.NotImplementedException();
        }
    }
}