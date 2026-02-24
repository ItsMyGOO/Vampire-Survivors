using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using XLua;

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
            if (File.Exists(fullPath))
                return Encoding.UTF8.GetBytes(File.ReadAllText(fullPath));

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