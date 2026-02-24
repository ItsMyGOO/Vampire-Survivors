using System;
using System.IO;
using System.Text;
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

            var call = _luaEnv.Global.Get<Action<LuaEntryTest>>("CallCS");
            call(this);
        }

        public void HelloFromLua()
        {
            Debug.Log("Hello from C#");
        }

        private byte[] CustomLoader(ref string filepath)
        {
            string fullPath = Application.dataPath + "/Lua/" + filepath + ".lua";
            if (File.Exists(fullPath))
                return Encoding.UTF8.GetBytes(
                    File.ReadAllText(fullPath)
                );

            return null;
        }

        void OnDestroy()
        {
            _luaEnv.Dispose();
        }
    }
}