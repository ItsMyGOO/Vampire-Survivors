namespace Core.Buff
{
    public interface IDamageModifier
    {
        void ApplyDamageModifiers(SkillContext ctx);
    }
}