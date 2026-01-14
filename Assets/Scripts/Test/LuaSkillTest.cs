using System;
using System.Collections.Generic;
using Lua;
using UnityEngine;
using XLua;

namespace Game.Test
{
    public class LuaSkillTest : MonoBehaviour
    {
        void Start()
        {
            var buffSystem = LuaMain.Env.DoString(@"
            local bs = BuffSystem.new()
            bs:add(FireBoost.new())
            return bs
        ")[0] as LuaTable;

            var skillExecutor = new LuaSkillExecutor(LuaMain.Env);

            var ctx = LuaMain.Env.NewTable();
            ctx.Set("caster", new { id = 1 });
            ctx.Set("target", new { id = 2 });
            ctx.Set("base_damage", 100);
            ctx.Set("buff_system", buffSystem);

            skillExecutor.CastSkill("fireball", ctx);
        }
    }

    public class LuaSkillExecutor
    {
        private readonly Dictionary<string, LuaTable> _skillCache = new();
        private LuaEnv _lua;

        public LuaSkillExecutor(LuaEnv lua)
        {
            _lua = lua;
        }

        public void CastSkill<T>(string skillName, T ctx)
        {
            // Lua 技能逻辑
            var skill = GetSkill(skillName);
            var cast = skill.Get<Action<T>>("cast");
            cast(ctx);
        }

        private LuaTable GetSkill(string skillName)
        {
            if (_skillCache.TryGetValue(skillName, out var table))
                return table;

            var require = _lua.Global.Get<Func<string, LuaTable>>("require");
            table = require($"Game/Skill/{skillName}");

            _skillCache.Add(skillName, table);
            return table;
        }
    }
}