using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Framework.Config
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
                // 去掉扩展名，用 Resources.Load 读取（路径相对于 Resources/）
                string resourcePath = "Data/" + Path.GetFileNameWithoutExtension(fileName);
                var textAsset = Resources.Load<TextAsset>(resourcePath);

                if (textAsset == null)
                {
                    Debug.LogError($"[JsonConfigLoader] Resources 中找不到配置: {resourcePath}");
                    return null;
                }

                T data = JsonConvert.DeserializeObject<T>(textAsset.text);

                if (data == null)
                {
                    Debug.LogError($"[JsonConfigLoader] 配置解析失败: {resourcePath}");
                    return null;
                }

                Debug.Log($"✓ 配置加载成功: {fileName}");
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"[JsonConfigLoader] 配置加载异常: {fileName}\n{e.Message}\n{e.StackTrace}");
                return null;
            }
        }
    }
}