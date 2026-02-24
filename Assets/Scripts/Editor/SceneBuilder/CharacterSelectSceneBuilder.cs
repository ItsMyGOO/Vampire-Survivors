using System.Reflection;
using Game;
using UI.Core;
using UI.Loader;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Editor.SceneBuilder
{
    /// <summary>
    /// 编辑器工具：构建 CharacterSelectScene
    /// CharacterSelectScene 不含独立 Canvas。
    /// UIManager 和 Canvas 由 MainMenuScene 的 DontDestroyOnLoad 跨场景保留，
    /// CharacterSelectLoader 在运行时通过 UIManager.LoadPanel 动态挂载 CharacterSelectPanel prefab。
    /// </summary>
    public static class CharacterSelectSceneBuilder
    {
        private const string SCENE_PATH = "Assets/Scene/CharacterSelectScene.unity";
        private const string PANEL_PREFAB_PATH = "Assets/Prefabs/UIPanel/CharacterSelectPanel.prefab";

        [MenuItem("Game/Build Scenes/Build CharacterSelectScene")]
        public static void BuildCharacterSelectScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // ── GameSceneManager ──────────────────────────────────────────────
            var gsmGo = new GameObject("GameSceneManager");
            gsmGo.AddComponent<GameSceneManager>();

            // ── EventSystem ───────────────────────────────────────────────────
            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
            esGo.AddComponent<StandaloneInputModule>();

            // ── 背景 Camera ───────────────────────────────────────────────────
            var cameraGo = new GameObject("Main Camera");
            var cam = cameraGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.05f, 0.05f, 0.1f, 1f);
            cam.orthographic = true;
            cameraGo.tag = "MainCamera";

            // ── CharacterSelectLoader ─────────────────────────────────────────
            // 负责在 Start() 时通过 UIManager 加载 CharacterSelectPanel prefab
            var loaderGo = new GameObject("CharacterSelectLoader");
            var loader = loaderGo.AddComponent<CharacterSelectLoader>();

            // 绑定 prefab 引用
            var panelPrefab = AssetDatabase.LoadAssetAtPath<UIPanel>(PANEL_PREFAB_PATH);
            if (panelPrefab != null)
            {
                var loaderType = typeof(CharacterSelectLoader);
                var field = loaderType.GetField("characterSelectPanelPrefab",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                field?.SetValue(loader, panelPrefab);
                Debug.Log($"[CharacterSelectSceneBuilder] 已绑定 prefab: {PANEL_PREFAB_PATH}");
            }
            else
            {
                Debug.LogWarning($"[CharacterSelectSceneBuilder] 未找到 prefab: {PANEL_PREFAB_PATH}，请手动绑定");
            }

            // ── 保存场景 ──────────────────────────────────────────────────────
            EditorSceneManager.SaveScene(scene, SCENE_PATH);
            Debug.Log($"[CharacterSelectSceneBuilder] CharacterSelectScene 已保存到 {SCENE_PATH}");
        }
    }
}
