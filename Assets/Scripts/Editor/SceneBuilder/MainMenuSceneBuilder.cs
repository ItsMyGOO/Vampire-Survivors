using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace Editor.SceneBuilder
{
    /// <summary>
    /// 编辑器工具：构建 MainMenuScene
    /// 执行一次即可，场景持久化后无需再次运行
    /// </summary>
    public static class MainMenuSceneBuilder
    {
        private const string SCENE_PATH = "Assets/Scene/MainMenuScene.unity";

        [MenuItem("Game/Build Scenes/Build MainMenuScene")]
public static void BuildMainMenuScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // ── GameSceneManager ──────────────────────────────────────────────
            var gsmGo = new GameObject("GameSceneManager");
            gsmGo.AddComponent<Game.GameSceneManager>();

            // ── EventSystem ───────────────────────────────────────────────────
            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
            esGo.AddComponent<StandaloneInputModule>();

            // ── UICanvas（DontDestroyOnLoad 根）──────────────────────────────
            // UICanvas 是全局唯一 Canvas，UIManager 挂在此处。
            // 跨场景切换时 Canvas 保留，各 Scene 通过 UIManager.LoadPanel 挂载 Panel prefab。
            var canvasGo = new GameObject("UICanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            var uiManager = canvasGo.AddComponent<UI.Core.UIManager>();

            // ── Panels 容器 ───────────────────────────────────────────────────
            var panelsGo = new GameObject("Panels");
            panelsGo.transform.SetParent(canvasGo.transform, false);
            var panelsRect = panelsGo.GetComponent<RectTransform>();
            panelsRect.anchorMin = Vector2.zero;
            panelsRect.anchorMax = Vector2.one;
            panelsRect.offsetMin = Vector2.zero;
            panelsRect.offsetMax = Vector2.zero;

            // 通过反射绑定 UIManager 字段
            var uiManagerType = typeof(UI.Core.UIManager);
            uiManagerType.GetField("panelContainer",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(uiManager, panelsGo.transform);
            uiManagerType.GetField("mainCanvas",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(uiManager, canvas);

            // ── MainMenuLoader ────────────────────────────────────────────────
            // 在运行时从 Prefab 加载 MainMenuPanel，不直接内嵌 Panel 到场景。
            var loaderGo = new GameObject("MainMenuLoader");
            var loader = loaderGo.AddComponent<UI.Loader.MainMenuLoader>();

            var panelPrefab = AssetDatabase.LoadAssetAtPath<UI.Core.UIPanel>(
                "Assets/Prefabs/UIPanel/MainMenuPanel.prefab");
            if (panelPrefab != null)
            {
                typeof(UI.Loader.MainMenuLoader)
                    .GetField("mainMenuPanelPrefab",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(loader, panelPrefab);
                Debug.Log("[MainMenuSceneBuilder] 已绑定 MainMenuPanel prefab");
            }
            else
            {
                Debug.LogWarning("[MainMenuSceneBuilder] 未找到 MainMenuPanel prefab，请手动绑定");
            }

            // ── 背景 Camera ───────────────────────────────────────────────────
            var cameraGo = new GameObject("Main Camera");
            var cam = cameraGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.05f, 0.05f, 0.1f, 1f);
            cam.orthographic = true;
            cameraGo.tag = "MainCamera";

            // ── 保存场景 ──────────────────────────────────────────────────────
            EditorSceneManager.SaveScene(scene, SCENE_PATH);
            Debug.Log($"[MainMenuSceneBuilder] MainMenuScene 已保存到 {SCENE_PATH}");

            AddSceneToBuildSettings(SCENE_PATH);
            // CharacterSelectScene 已废弃，选人功能改为 Panel 内嵌切换cene.unity");
            AddSceneToBuildSettings("Assets/Scene/BattleScene.unity");
        }

        private static void BuildMainMenuPanel(Transform parent)
        {
            // ── Panel 根节点 ───────────────────────────────────────────────────
            var panelGo = new GameObject("MainMenuPanel");
            panelGo.transform.SetParent(parent, false);

            var panelRect = panelGo.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            panelGo.AddComponent<CanvasGroup>();

            var panel = panelGo.AddComponent<UI.Panel.MainMenuPanel>();

            // ── 背景图（深色半透明遮罩）────────────────────────────────────────
            var bgGo = new GameObject("Background");
            bgGo.transform.SetParent(panelGo.transform, false);
            var bgRect = bgGo.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            var bgImg = bgGo.AddComponent<Image>();
            bgImg.color = new Color(0.04f, 0.04f, 0.08f, 0.95f);

            // ── 标题文字 ──────────────────────────────────────────────────────
            var titleGo = new GameObject("TitleText");
            titleGo.transform.SetParent(panelGo.transform, false);
            var titleRect = titleGo.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0, 160);
            titleRect.sizeDelta = new Vector2(800, 120);
            var titleTmp = titleGo.AddComponent<TextMeshProUGUI>();
            titleTmp.text = "Vampire Survivors";
            titleTmp.fontSize = 72;
            titleTmp.fontStyle = FontStyles.Bold;
            titleTmp.alignment = TextAlignmentOptions.Center;
            titleTmp.color = new Color(0.95f, 0.85f, 0.3f, 1f);

            // ── 副标题 ────────────────────────────────────────────────────────
            var subTitleGo = new GameObject("SubTitleText");
            subTitleGo.transform.SetParent(panelGo.transform, false);
            var subTitleRect = subTitleGo.AddComponent<RectTransform>();
            subTitleRect.anchorMin = new Vector2(0.5f, 0.5f);
            subTitleRect.anchorMax = new Vector2(0.5f, 0.5f);
            subTitleRect.anchoredPosition = new Vector2(0, 90);
            subTitleRect.sizeDelta = new Vector2(600, 50);
            var subTitleTmp = subTitleGo.AddComponent<TextMeshProUGUI>();
            subTitleTmp.text = "survive the night";
            subTitleTmp.fontSize = 28;
            subTitleTmp.fontStyle = FontStyles.Italic;
            subTitleTmp.alignment = TextAlignmentOptions.Center;
            subTitleTmp.color = new Color(0.7f, 0.6f, 0.4f, 1f);

            // ── 开始按钮 ──────────────────────────────────────────────────────
            var startBtnGo = CreateButton(panelGo.transform,
                "StartButton", "开始游戏",
                new Vector2(0, -10),
                new Vector2(320, 72),
                new Color(0.8f, 0.2f, 0.2f, 1f));

            // ── 退出按钮 ──────────────────────────────────────────────────────
            var quitBtnGo = CreateButton(panelGo.transform,
                "QuitButton", "退出游戏",
                new Vector2(0, -110),
                new Vector2(320, 72),
                new Color(0.2f, 0.2f, 0.25f, 1f));

            // ── 版本号 ────────────────────────────────────────────────────────
            var versionGo = new GameObject("VersionText");
            versionGo.transform.SetParent(panelGo.transform, false);
            var versionRect = versionGo.AddComponent<RectTransform>();
            versionRect.anchorMin = new Vector2(1f, 0f);
            versionRect.anchorMax = new Vector2(1f, 0f);
            versionRect.anchoredPosition = new Vector2(-20, 20);
            versionRect.sizeDelta = new Vector2(200, 30);
            var versionTmp = versionGo.AddComponent<TextMeshProUGUI>();
            versionTmp.text = "v0.1.0";
            versionTmp.fontSize = 18;
            versionTmp.alignment = TextAlignmentOptions.Right;
            versionTmp.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);

            // ── 绑定字段到 MainMenuPanel ──────────────────────────────────────
            var panelType = typeof(UI.Panel.MainMenuPanel);
            SetSerializedField(panel, panelType, "startButton", startBtnGo.GetComponent<Button>());
            SetSerializedField(panel, panelType, "quitButton", quitBtnGo.GetComponent<Button>());
            SetSerializedField(panel, panelType, "titleText", titleTmp);

            // 保存为 Prefab
            var prefabPath = "Assets/Prefabs/UIPanel/MainMenuPanel.prefab";
            EnsureDirectoryExists("Assets/Prefabs/UIPanel");
            PrefabUtility.SaveAsPrefabAssetAndConnect(panelGo, prefabPath, InteractionMode.AutomatedAction);
            Debug.Log($"[MainMenuSceneBuilder] MainMenuPanel prefab 已保存: {prefabPath}");
        }

        private static GameObject CreateButton(Transform parent, string name, string label,
            Vector2 anchoredPos, Vector2 size, Color bgColor)
        {
            var btnGo = new GameObject(name);
            btnGo.transform.SetParent(parent, false);
            var btnRect = btnGo.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.5f, 0.5f);
            btnRect.anchorMax = new Vector2(0.5f, 0.5f);
            btnRect.anchoredPosition = anchoredPos;
            btnRect.sizeDelta = size;

            var btnImg = btnGo.AddComponent<Image>();
            btnImg.color = bgColor;

            var btn = btnGo.AddComponent<Button>();
            var colors = btn.colors;
            colors.highlightedColor = Color.white * 1.2f;
            colors.pressedColor = bgColor * 0.7f;
            btn.colors = colors;

            // 按钮文字
            var textGo = new GameObject("Text");
            textGo.transform.SetParent(btnGo.transform, false);
            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 32;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            return btnGo;
        }

        private static void SetSerializedField(object target, System.Type type, string fieldName, object value)
        {
            var field = type.GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(target, value);
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var parts = path.Split('/');
                var current = parts[0];
                for (int i = 1; i < parts.Length; i++)
                {
                    var next = current + "/" + parts[i];
                    if (!AssetDatabase.IsValidFolder(next))
                        AssetDatabase.CreateFolder(current, parts[i]);
                    current = next;
                }
            }
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            var scenes = EditorBuildSettings.scenes;

            foreach (var s in scenes)
                if (s.path == scenePath)
                    return;

            var newScenes = new EditorBuildSettingsScene[scenes.Length + 1];
            System.Array.Copy(scenes, newScenes, scenes.Length);
            newScenes[scenes.Length] = new EditorBuildSettingsScene(scenePath, true);
            EditorBuildSettings.scenes = newScenes;

            Debug.Log($"[MainMenuSceneBuilder] 已添加到 Build Settings: {scenePath}");
        }
    }
}
