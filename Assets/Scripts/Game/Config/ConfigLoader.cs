using System;
using UnityEngine;

namespace ConfigHandler
{
    public static class ConfigLoader
    {
        public static void Load<T>(
            Func<T> loadFunc,
            Action<T> initFunc,
            string name
        ) where T : class
        {
            var db = loadFunc();
            if (db != null)
            {
                initFunc(db);
                Debug.Log($"✓ {name} 加载完成");
            }
            else
            {
                Debug.LogError($"✗ {name} 加载失败");
            }
        }
    }

}