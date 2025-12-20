namespace Core
{
    public class SkillContext
    {
        public ISkillSource Source;
        public ISkillTarget Target;

        public IDamageModifier SourceBuffs;
        public IDamageModifier TargetBuffs;

        public DamageContext Damage = new();
        public HitContext Hit;

        public class DamageContext
        {
            public int Base;
            public int Value;
            public float Multiplier = 1f;
            public bool IsCritical;
        }

        public class HitContext
        {
        }
    }
}