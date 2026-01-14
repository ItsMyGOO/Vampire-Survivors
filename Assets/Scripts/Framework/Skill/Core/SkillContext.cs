namespace Core.Buff
{
    public class SkillContext
    {
        public ISkillSource Caster;
        public ISkillTarget Target;

        public IDamageModifier SourceBuffs;
        public IDamageModifier TargetBuffs;

        public DamageContext Damage;
        public HitContext Hit;

        public class DamageContext
        {
            public int BaseDamage;
            public int FinalDamage;
            public float Multiplier = 1f;
            public bool IsCritical;
        }

        public class HitContext
        {
        }
    }
}