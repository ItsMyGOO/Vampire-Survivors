using System.Collections.Generic;
using Core;

namespace Combat.Skill
{
    public class SkillExecutor
    {
        private readonly List<ISkillStep> _steps;

        public SkillExecutor(IEnumerable<ISkillStep> steps)
        {
            _steps = new List<ISkillStep>(steps);
        }

        public void Execute(SkillContext ctx)
        {
            foreach (var step in _steps)
                step.Execute(ctx);
        }
    }
}