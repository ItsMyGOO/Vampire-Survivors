using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor
{
    /// <summary>
    /// 在已打开的 MainMenuScene 中构建场景内容：
    ///   - GameSceneManager (DontDestroyOnLoad 单例)
    ///   - UICanvas (复用 UICanvas.prefab 结构：Canvas + UIManager)
    ///     └── Panels
    ///         └── MainMenuPanel (nested prefab)
    /// </summary>
    public static class BuildMainMenuScene
    {
        [MenuItem("Tools/Build MainMenuScene")]
        public static void Build()
        {
            // ── 1. 确保 MainMenuScene 已打开 ─────────────────────
            var scene = EditorSceneManager.GetActiveScene();
            if (scene.name != "MainMenuScene")
            {
                Debug.LogError("[BuildMainMenuScene] 请先打开 MainMenuScene 再执行此工具。");
                return;
            }

            // 清理已有根对象（幂等）
            foreach (var go in scene.GetRootGameObjects())
                Object.DestroyImmediate(go);

            // ── 2. GameSceneManager ───────────────────────────────
            var gsmGo = new GameObject("GameSceneManager");
            gsmGo.AddComponent<Game.GameSceneManager>();
            EditorSceneManager.MoveGameObjectToScene(gsmGo, scene);

            // ── 3. UICanvas ───────────────────────────────────────
            // 从 UICanvas.prefab 实例化，保留 prefab 连接
            var uiCanvasPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UICanvas.prefab");
            if (uiCanvasPrefab == null)
            {
                Debug.LogError("[BuildMainMenuScene] UICanvas.prefab 未找到。");
                return;
            }

            var uiCanvasGo = (GameObject)PrefabUtility.InstantiatePrefab(uiCanvasPrefab, scene);
            uiCanvasGo.name = "UICanvas";

            // ── 4. 找到 Panels 容器 ───────────────────────────────
            var panelsTransform = uiCanvasGo.transform.Find("Panels");
            if (panelsTransform == null)
            {
                Debug.LogError("[BuildMainMenuScene] UICanvas/Panels 未找到。");
                return;
            }

            // 移除 UICanvas.prefab 里已有的 Battle 面板（MainMenuScene 不需要）
            for (int i = panelsTransform.childCount - 1; i >= 0; i--)
                Object.DestroyImmediate(panelsTransform.GetChild(i).gameObject);

            // ── 5. 嵌入 MainMenuPanel prefab ─────────────────────
            var mainMenuPanelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Prefabs/UIPanel/MainMenuPanel.prefab");
            if (mainMenuPanelPrefab == null)
            {
                Debug.LogError("[BuildMainMenuScene] MainMenuPanel.prefab 未找到。");
                return;
            }

            var mainMenuPanelGo = (GameObject)PrefabUtility.InstantiatePrefab(
                mainMenuPanelPrefab, panelsTransform);
            mainMenuPanelGo.name = "MainMenuPanel";

            // RectTransform 全拉伸
            var rt = mainMenuPanelGo.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }

            // ── 6. 配置 UIManager → panelContainer 引用 ─────────
            var uiManager = uiCanvasGo.GetComponent<UI.Core.UIManager>();
            if (uiManager != null)
            {
                var so = new SerializedObject(uiManager);
                so.FindProperty("panelContainer").objectReferenceValue = panelsTransform;

                // 同时确保 mainCanvas 引用正确
                var canvas = uiCanvasGo.GetComponent<Canvas>();
                so.FindProperty("mainCanvas").objectReferenceValue = canvas;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            // ── 7. MainMenuPanel 默认显示（hideOnAwake = false）──
            var panelScript = mainMenuPanelGo.GetComponent<UI.Panel.MainMenuPanel>();
            if (panelScript != null)
            {
                var pso = new SerializedObject(panelScript);
                pso.FindProperty("hideOnAwake").boolValue = false;
                pso.ApplyModifiedPropertiesWithoutUndo();
            }

            // ── 8. 保存场景 ───────────────────────────────────────
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            AssetDatabase.Refresh();
            Debug.Log("[BuildMainMenuScene] MainMenuScene 构建完成。");
        }
    }
}
