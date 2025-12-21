using System.Collections.Generic;
using Combat.Skill;
using Core;
using UnityEngine;
using XLua;

namespace Game.Test
{
    public class LuaSkillTest : MonoBehaviour
    {
        private LuaEnv _luaEnv;

        void Start()
        {
            _luaEnv = new LuaEnv();
            _luaEnv.AddLoader(CustomLoader);

            // skill test
            new LuaSkillExecutor(_luaEnv).CastSkill("fireball", new Player(), new Player());
            // lua skill test
            _luaEnv.DoString("require 'test/bootstrap'");
        }

        private byte[] CustomLoader(ref string filepath)
        {
            string fullPath = Application.dataPath + "/Lua/" + filepath + ".lua";
            if (System.IO.File.Exists(fullPath))
                return System.Text.Encoding.UTF8.GetBytes(
                    System.IO.File.ReadAllText(fullPath)
                );

            return null;
        }

        void OnDestroy()
        {
            _luaEnv.Dispose();
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
            table = require($"test/{skillName}");

            _skillCache.Add(skillName, table);
            return table;
        }
    }
}