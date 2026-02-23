using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Editor
{
    public static class BuildSettingsCleaner
    {
        [MenuItem("Game/Build Settings/Remove CharacterSelectScene")]
        public static void RemoveCharacterSelectScene()
        {
            const string targetPath = "Assets/Scene/CharacterSelectScene.unity";
            var scenes = EditorBuildSettings.scenes;
            var filtered = new List<EditorBuildSettingsScene>();

            foreach (var s in scenes)
            {
                if (s.path != targetPath)
                    filtered.Add(s);
                else
                    Debug.Log($"[BuildSettingsCleaner] 已从 Build Settings 移除: {s.path}");
            }

            EditorBuildSettings.scenes = filtered.ToArray();
            Debug.Log($"[BuildSettingsCleaner] Build Settings 更新完成，共 {filtered.Count} 个场景");
        }
    }
}
