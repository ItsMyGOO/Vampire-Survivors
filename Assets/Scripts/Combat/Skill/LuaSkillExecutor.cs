using System.Collections.Generic;
using Core;
using XLua;

namespace Combat.Skill
{
    public class LuaSkillExecutor
    {
        private readonly Dictionary<string, LuaTable> _skillCache = new();
        private LuaEnv _lua;

        public LuaSkillExecutor(LuaEnv lua)
        {
            _lua = lua;
        }

        public void CastSkill(string skillName, ISkillSource caster, ISkillTarget target)
        {
            var ctx = new SkillContext
            {
                Caster = caster,
                Target = target,
                Damage = new SkillContext.DamageContext()
                {
                    BaseDamage = caster.Attack,
                    FinalDamage = caster.Attack
                }
            };
            
            // Lua 技能逻辑
            GetSkill(skillName).Get<System.Action<SkillContext>>("Cast")(ctx);

            // 统一结算
            target.TakeDamage(ctx.Damage.FinalDamage);
        }

        private LuaTable GetSkill(string skillName)
        {
            if (_skillCache.TryGetValue(skillName, out var table))
                return table;

            var require = _lua.Global.Get<System.Func<string, LuaTable>>("require");
            table = require($"skill/{skillName}");

            _skillCache.Add(skillName, table);
            return table;
        }
    }
}