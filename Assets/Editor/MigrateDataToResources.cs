using UnityEditor;
using UnityEngine;
using System.IO;

namespace Editor
{
    /// <summary>
    /// 将 Assets/Data/*.json 迁移到 Assets/Resources/Data/
    /// 执行后自动删除自身
    /// </summary>
    public static class MigrateDataToResources
    {
        [MenuItem("Tools/Migrate Data To Resources (run once)")]
        public static void Run()
        {
            string srcDir = "Assets/Data";
            string dstDir = "Assets/Resources/Data";

            if (!AssetDatabase.IsValidFolder(dstDir))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    AssetDatabase.CreateFolder("Assets", "Resources");
                AssetDatabase.CreateFolder("Assets/Resources", "Data");
            }

            string[] guids = AssetDatabase.FindAssets("", new[] { srcDir });
            int moved = 0;
            foreach (string guid in guids)
            {
                string src = AssetDatabase.GUIDToAssetPath(guid);
                if (!src.EndsWith(".json")) continue;

                string fileName = Path.GetFileName(src);
                string dst = dstDir + "/" + fileName;

                // skip if already there
                if (AssetDatabase.LoadAssetAtPath<Object>(dst) != null)
                {
                    Debug.Log($"[Migrate] 已存在，跳过: {dst}");
                    continue;
                }

                string err = AssetDatabase.MoveAsset(src, dst);
                if (string.IsNullOrEmpty(err))
                {
                    Debug.Log($"[Migrate] ✓ {src} → {dst}");
                    moved++;
                }
                else
                {
                    Debug.LogError($"[Migrate] ✗ {src}: {err}");
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"[Migrate] 完成，共移动 {moved} 个文件");

            // 自删
            AssetDatabase.DeleteAsset("Assets/Editor/MigrateDataToResources.cs");
        }
    }
}
