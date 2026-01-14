using Core.Buff;

namespace Combat.Skill
{
    public class ApplyFinalDamageStep : ISkillStep
    {
        public void Execute(SkillContext ctx)
        {
            ctx.Target.ApplyDamage(ctx.Damage.FinalDamage);
        }
    }
}