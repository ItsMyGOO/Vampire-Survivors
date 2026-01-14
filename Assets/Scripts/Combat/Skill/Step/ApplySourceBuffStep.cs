using Core.Buff;

namespace Combat.Skill
{
    public class ApplySourceBuffStep : ISkillStep
    {
        public void Execute(SkillContext ctx)
        {
            ctx.SourceBuffs?.ApplyDamageModifiers(ctx);
        }
    }
}