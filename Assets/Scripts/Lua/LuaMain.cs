using XLua;
using UnityEngine;

namespace Lua
{
    public class LuaMain : MonoBehaviour
    {
        public static LuaEnv Env { get; private set; }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            Env = new LuaEnv();
            Env.AddLoader(CustomLoader);
            Env.DoString("require('bootstrap')");
        }

        private byte[] CustomLoader(ref string filepath)
        {
            var fullPath = Application.dataPath + "/Lua/" +
                           filepath.Replace('.', '/') + ".lua";
            if (System.IO.File.Exists(fullPath))
                return System.Text.Encoding.UTF8.GetBytes(System.IO.File.ReadAllText(fullPath));

            Debug.LogError("Lua not found: " + fullPath);
            return null;
        }

        void Update()
        {
            Env.Tick();
        }

        void OnDestroy()
        {
            Env.Dispose();
            Env = null;
        }
    }
}