using XLua;
using UnityEngine;

namespace Lua
{
    public class LuaMain : MonoBehaviour
    {
        private LuaEnv _luaEnv;

        void Awake()
        {
            _luaEnv = new LuaEnv();

            // 指定 Lua 根目录
            _luaEnv.AddLoader(CustomLoader);

            _luaEnv.DoString("require('bootstrap')");
        }

        private byte[] CustomLoader(ref string filepath)
        {
            var fullPath = Application.dataPath + "/Lua/" +
                           filepath.Replace('.', '/') + ".lua";
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