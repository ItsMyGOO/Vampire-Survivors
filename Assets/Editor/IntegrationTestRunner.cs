using ConfigHandler;
using Session;
using UnityEditor;
using UnityEngine;
using Battle;

namespace Editor
{
    /// <summary>
    /// 集成测试工具：验证主菜单→选角→战斗的完整数据流
    /// 在 Editor 模式下运行，不需要进入 PlayMode
    /// </summary>
    public static class IntegrationTestRunner
    {
        [MenuItem("Tools/Integration Test/Run Flow Validation")]
        public static void RunFlowValidation()
        {
            Debug.Log("=== 集成测试开始 ===");
            int pass = 0;
            int fail = 0;

            // ── Test 1: 配置加载 ─────────────────────────────────
            Debug.Log("\n[Test 1] 配置数据库加载");
            GameConfigLoader.LoadAll();

            var charDB = CharacterConfigDB.Instance;
            if (charDB != null)
            {
                var chars = charDB.GetAllCharacters();
                if (chars != null && chars.Count > 0)
                {
                    Debug.Log($"  ✓ CharacterConfigDB 加载成功，共 {chars.Count} 个角色");
                    foreach (var c in chars)
                        Debug.Log($"    - [{c.id}] {c.displayName}");
                    pass++;
                }
                else
                {
                    Debug.LogError("  ✗ CharacterConfigDB 为空");
                    fail++;
                }
            }
            else
            {
                Debug.LogError("  ✗ CharacterConfigDB.Instance 为 null");
                fail++;
            }

            // ── Test 2: Build Settings 场景验证 ──────────────────
            Debug.Log("\n[Test 2] Build Settings 场景验证");
            var scenes = new[] { "Bootstrapper", "MainMenuScene", "CharacterSelectScene", "BattleScene" };
            var buildScenes = EditorBuildSettings.scenes;

            foreach (var expected in scenes)
            {
                bool found = false;
                foreach (var s in buildScenes)
                {
                    if (s.path.Contains(expected) && s.enabled)
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    Debug.Log($"  ✓ {expected} 在 Build Settings 中");
                    pass++;
                }
                else
                {
                    Debug.LogError($"  ✗ {expected} 不在 Build Settings 或未启用");
                    fail++;
                }
            }

            // ── Test 3: Prefab 存在性验证 ─────────────────────────
            Debug.Log("\n[Test 3] 关键 Prefab 验证");
            var prefabs = new[]
            {
                "Assets/Prefabs/UICanvas.prefab",
                "Assets/Prefabs/UIPanel/MainMenuPanel.prefab",
                "Assets/Prefabs/UIPanel/CharacterSelectPanel.prefab",
                "Assets/Prefabs/UIPanel/CharacterCardItem.prefab",
            };

            foreach (var prefabPath in prefabs)
            {
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (asset != null)
                {
                    Debug.Log($"  ✓ {System.IO.Path.GetFileName(prefabPath)}");
                    pass++;
                }
                else
                {
                    Debug.LogError($"  ✗ 缺少 {prefabPath}");
                    fail++;
                }
            }

            // ── Test 4: GameSessionData 流程模拟 ─────────────────
            Debug.Log("\n[Test 4] GameSessionData 选角流程模拟");
            GameSessionData.Reset();

            if (!GameSessionData.HasSelection)
            {
                Debug.Log("  ✓ Reset 后 HasSelection = false");
                pass++;
            }
            else
            {
                Debug.LogError("  ✗ Reset 后 HasSelection 仍为 true");
                fail++;
            }

            // 模拟选角
            if (CharacterConfigDB.Instance != null)
            {
                var chars = CharacterConfigDB.Instance.GetAllCharacters();
                if (chars != null && chars.Count > 0)
                {
                    var firstChar = chars[0];
                    GameSessionData.SelectCharacter(firstChar.id);

                    if (GameSessionData.HasSelection && GameSessionData.SelectedCharacterId == firstChar.id)
                    {
                        Debug.Log($"  ✓ 选角成功: {firstChar.id} ({firstChar.displayName})");
                        pass++;
                    }
                    else
                    {
                        Debug.LogError($"  ✗ 选角写入失败");
                        fail++;
                    }

                    // 清理
                    GameSessionData.Reset();
                }
            }

            // ── Test 5: CharacterDef 属性完整性 ──────────────────
            Debug.Log("\n[Test 5] CharacterDef 属性完整性");
            if (CharacterConfigDB.Instance != null)
            {
                var chars = CharacterConfigDB.Instance.GetAllCharacters();
                bool allValid = true;
                foreach (var c in chars)
                {
                    if (string.IsNullOrEmpty(c.id) || string.IsNullOrEmpty(c.displayName))
                    {
                        Debug.LogError($"  ✗ 角色 id/displayName 缺失: {c.id}");
                        allValid = false;
                        fail++;
                    }
                }
                if (allValid)
                {
                    Debug.Log($"  ✓ 所有 {chars.Count} 个角色数据完整");
                    pass++;
                }
            }

            // ── 结果汇总 ─────────────────────────────────────────
            Debug.Log($"\n=== 集成测试结束 === 通过: {pass}  失败: {fail}");
            if (fail == 0)
                Debug.Log("🎉 所有测试通过！流程就绪。");
            else
                Debug.LogWarning($"⚠ 有 {fail} 项测试失败，请查看上方日志。");
        }

        [MenuItem("Tools/Integration Test/Reset GameSessionData")]
        public static void ResetSession()
        {
            GameSessionData.Reset();
            Debug.Log("[IntegrationTest] GameSessionData 已重置");
        }
    }
}
