using UI.Core;
using UI.Panel;
using UnityEngine;

// [DEPRECATED] 旧流程遗留：CharacterSelectScene 的独立场景加载器。
// 新流程（BattleScene 常驻）中已由 UIManager.ShowPanel<CharacterSelectPanel>() 取代。
// 保留此文件作为回滚参考，暂不删除。
namespace UI.Loader
{
    /// <summary>
    /// CharacterSelectScene 的面板加载器
    /// 职责：在场景 Start 时，从 UIManager（DontDestroyOnLoad）的单一 Canvas 上
    /// 动态实例化 CharacterSelectPanel prefab，并立即显示。
    ///
    /// 不创建任何 Canvas / EventSystem，这些由 MainMenuScene 跨场景保留。
    /// </summary>
    public class CharacterSelectLoader : MonoBehaviour
    {
        [SerializeField] private UIPanel characterSelectPanelPrefab;

        private void Start()
        {
            if (UIManager.Instance == null)
            {
                Debug.LogError("[CharacterSelectLoader] UIManager 不存在！请确保 MainMenuScene 先加载。");
                return;
            }

            if (characterSelectPanelPrefab == null)
            {
                Debug.LogError("[CharacterSelectLoader] characterSelectPanelPrefab 未赋值！");
                return;
            }

            // 隐藏其他所有面板（如残留的 MainMenuPanel）
            UIManager.Instance.HideAllPanels();

            // 加载并显示 CharacterSelectPanel
            var panel = UIManager.Instance.LoadPanel<CharacterSelectPanel>(characterSelectPanelPrefab);
            if (panel != null)
            {
                UIManager.Instance.ShowPanel<CharacterSelectPanel>(hideOthers: true, addToStack: false);
                Debug.Log("[CharacterSelectLoader] CharacterSelectPanel 已显示");
            }
            else
            {
                Debug.LogError("[CharacterSelectLoader] 加载 CharacterSelectPanel 失败！");
            }
        }
    }
}
