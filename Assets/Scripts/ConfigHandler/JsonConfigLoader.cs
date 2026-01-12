using System;
using System.IO;
using UnityEngine;

namespace ConfigHandler
{
    /// <summary>
    /// JSON 配置加载器
    /// </summary>
    public static class JsonConfigLoader
    {
        private static readonly string ConfigBasePath = "Assets/Data/";

        /// <summary>
        /// 从 JSON 文件加载配置
        /// </summary>
        /// <param name="fileName">文件名（例如: "animation_config.json"）</param>
        public static T Load<T>(string fileName) where T : class
        {
            try
            {
                string path = Path.Combine(ConfigBasePath, fileName);
                
                if (!File.Exists(path))
                {
                    Debug.LogError($"配置文件不存在: {path}");
                    return null;
                }

                string json = File.ReadAllText(path);
                T data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);

                if (data == null)
                {
                    Debug.LogError($"配置文件解析失败: {path}");
                    return null;
                }

                Debug.Log($"✓ 配置加载成功: {fileName}");
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"配置加载异常: {fileName}\n{e.Message}\n{e.StackTrace}");
                return null;
            }
        }
    }
}
