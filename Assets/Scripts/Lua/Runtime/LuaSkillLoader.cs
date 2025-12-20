using XLua;

namespace Lua
{
    public class LuaSkillLoader
    {
        private LuaEnv _env;

        public LuaSkillLoader(LuaEnv env)
        {
            _env = env;
        }

        public LuaTable LoadSkill(string skillName)
        {
            var require = _env.Global.Get<LuaFunction>("require");
            return require.Call($"skill.{skillName}")[0] as LuaTable;
        }
    }
}