using Combat.Skill;
using Game.Test;
using UnityEngine;
using XLua;

namespace Lua
{
    public class LuaEntryTest : MonoBehaviour
    {
        private LuaEnv _luaEnv;

        void Start()
        {
            _luaEnv = new LuaEnv();
            _luaEnv.AddLoader(CustomLoader);

            BaseTest();
        }

        void BaseTest()
        {
            _luaEnv.DoString("require 'test/entry_test'");

            int sum = _luaEnv.Global.Get<IntBinaryOp>("Add")(2, 3);
            Debug.Log("Lua Add result: " + sum);

            var call = _luaEnv.Global.Get<System.Action<LuaEntryTest>>("CallCS");
            call(this);
        }

        public void HelloFromLua()
        {
            Debug.Log("Hello from C#");
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
}