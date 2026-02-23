using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor
{
    /// <summary>
    /// 在已打开的 CharacterSelectScene 中构建场景内容：
    ///   - GameSceneManager (DontDestroyOnLoad 单例)
    ///   - UICanvas (复用 UICanvas.prefab)
    ///     └── Panels
    ///         └── CharacterSelectPanel (nested prefab)
    /// </summary>
    public static class BuildCharacterSelectScene
    {
        [MenuItem("Tools/Build CharacterSelectScene")]
        public static void Build()
        {
            // ── 1. 确保目标场景已打开 ─────────────────────────────
            var scene = EditorSceneManager.GetActiveScene();
            if (scene.name != "CharacterSelectScene")
            {
                Debug.LogError("[BuildCharacterSelectScene] 请先打开 CharacterSelectScene 再执行此工具。");
                return;
            }

            // 清理已有根对象（幂等）
            foreach (var go in scene.GetRootGameObjects())
                Object.DestroyImmediate(go);

            // ── 2. GameSceneManager ────────────────────────────────
            var gsmGo = new GameObject("GameSceneManager");
            gsmGo.AddComponent<Game.GameSceneManager>();
            EditorSceneManager.MoveGameObjectToScene(gsmGo, scene);

            // ── 3. UICanvas ────────────────────────────────────────
            var uiCanvasPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UICanvas.prefab");
            if (uiCanvasPrefab == null)
            {
                Debug.LogError("[BuildCharacterSelectScene] UICanvas.prefab 未找到。");
                return;
            }

            var uiCanvasGo = (GameObject)PrefabUtility.InstantiatePrefab(uiCanvasPrefab, scene);
            uiCanvasGo.name = "UICanvas";

            // ── 4. 找到 Panels 容器 ───────────────────────────────
            var panelsTransform = uiCanvasGo.transform.Find("Panels");
            if (panelsTransform == null)
            {
                Debug.LogError("[BuildCharacterSelectScene] UICanvas/Panels 未找到。");
                return;
            }

            // 清除 Panels 下的现有子对象
            for (int i = panelsTransform.childCount - 1; i >= 0; i--)
                Object.DestroyImmediate(panelsTransform.GetChild(i).gameObject);

            // ── 5. 嵌入 CharacterSelectPanel prefab ───────────────
            var panelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Prefabs/UIPanel/CharacterSelectPanel.prefab");
            if (panelPrefab == null)
            {
                Debug.LogError("[BuildCharacterSelectScene] CharacterSelectPanel.prefab 未找到。");
                return;
            }

            var panelGo = (GameObject)PrefabUtility.InstantiatePrefab(panelPrefab, panelsTransform);
            panelGo.name = "CharacterSelectPanel";

            // RectTransform 全拉伸
            var rt = panelGo.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }

            // ── 6. 配置 UIManager → panelContainer 引用 ──────────
            var uiManager = uiCanvasGo.GetComponent<UI.Core.UIManager>();
            if (uiManager != null)
            {
                var so = new SerializedObject(uiManager);
                so.FindProperty("panelContainer").objectReferenceValue = panelsTransform;

                var canvas = uiCanvasGo.GetComponent<Canvas>();
                so.FindProperty("mainCanvas").objectReferenceValue = canvas;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            // ── 7. CharacterSelectPanel 默认显示 ──────────────────
            var panelScript = panelGo.GetComponent<UI.Panel.CharacterSelectPanel>();
            if (panelScript != null)
            {
                var pso = new SerializedObject(panelScript);
                var hideOnAwakeProp = pso.FindProperty("hideOnAwake");
                if (hideOnAwakeProp != null)
                {
                    hideOnAwakeProp.boolValue = false;
                    pso.ApplyModifiedPropertiesWithoutUndo();
                }
            }

            // ── 8. 保存场景 ───────────────────────────────────────
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            AssetDatabase.Refresh();
            Debug.Log("[BuildCharacterSelectScene] CharacterSelectScene 构建完成。");
        }
    }
}
