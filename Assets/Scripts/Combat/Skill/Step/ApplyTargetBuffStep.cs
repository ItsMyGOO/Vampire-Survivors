using Core.Buff;

namespace Combat.Skill
{
    public class ApplyTargetBuffStep : ISkillStep
    {
        public void Execute(SkillContext ctx)
        {
            ctx.TargetBuffs?.ApplyDamageModifiers(ctx);
        }
    }
}