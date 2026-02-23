using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// 将游戏所需资产注册进 Addressables Default Local Group：
    ///   - Assets/Scene/MainMenuScene.unity        → "MainMenuScene"
    ///   - Assets/Scene/CharacterSelectScene.unity → "CharacterSelectScene"
    ///   - Assets/Scene/BattleScene.unity          → "BattleScene"
    ///   - Assets/Resources/Data/character_config.json → "character_config"
    ///
    /// 重复运行安全（幂等）。
    /// </summary>
    public static class AddressablesSetup
    {
        private static readonly (string path, string address)[] Entries =
        {
            ("Assets/Scene/MainMenuScene.unity",                    "MainMenuScene"),
            ("Assets/Scene/CharacterSelectScene.unity",             "CharacterSelectScene"),
            ("Assets/Scene/BattleScene.unity",                      "BattleScene"),
            ("Assets/Resources/Data/character_config.json",         "character_config"),
        };

        [MenuItem("Tools/Setup Addressables")]
        public static void Setup()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("[AddressablesSetup] AddressableAssetSettings 未找到，请先在 Window > Asset Management > Addressables > Groups 中初始化。");
                return;
            }

            // 找到 Default Local Group
            var group = settings.FindGroup("Default Local Group");
            if (group == null)
            {
                Debug.LogError("[AddressablesSetup] 找不到 'Default Local Group'，请确认 Addressables 已初始化。");
                return;
            }

            int added = 0;
            int skipped = 0;

            foreach (var (path, address) in Entries)
            {
                var guid = AssetDatabase.AssetPathToGUID(path);
                if (string.IsNullOrEmpty(guid))
                {
                    Debug.LogWarning($"[AddressablesSetup] 资产不存在，跳过: {path}");
                    skipped++;
                    continue;
                }

                // 检查是否已存在
                var existing = settings.FindAssetEntry(guid);
                if (existing != null)
                {
                    // 确保 address 正确
                    if (existing.address != address)
                    {
                        existing.address = address;
                        Debug.Log($"[AddressablesSetup] 更新 address: {path} → {address}");
                        added++;
                    }
                    else
                    {
                        Debug.Log($"[AddressablesSetup] 已存在，跳过: {address}");
                        skipped++;
                    }
                    continue;
                }

                // 新增
                var entry = settings.CreateOrMoveEntry(guid, group, readOnly: false, postEvent: false);
                entry.address = address;
                Debug.Log($"[AddressablesSetup] 已注册: {address}  ({path})");
                added++;
            }

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, null, true);
            AssetDatabase.SaveAssets();

            Debug.Log($"[AddressablesSetup] 完成。新增/更新: {added}，跳过: {skipped}");
        }
    }
}
