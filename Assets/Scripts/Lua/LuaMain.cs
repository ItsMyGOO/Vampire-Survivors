using System;
using System.Collections.Generic;
using XLua;
using UnityEngine;

namespace Lua
{
    public class LuaMain : MonoBehaviour
    {
        public static LuaEnv Env { get; private set; }
        private static List<IDisposable> disposables = new List<IDisposable>();

        public static void Register(IDisposable disposable)
        {
            disposables.Add(disposable);
        }

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

        private void OnDestroy()
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
            
            Env.Dispose();
            Env = null;
        }
    }
}