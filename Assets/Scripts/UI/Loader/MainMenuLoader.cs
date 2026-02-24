using UI.Core;
using UI.Panel;
using UnityEngine;

// [DEPRECATED] 旧流程遗留：MainMenuScene 的面板加载器。
// 新流程已跳过 MainMenuScene，直接进入 BattleScene。
// 保留此文件作为回滚参考，暂不删除。
namespace UI.Loader
{
    /// <summary>
    /// MainMenuScene 的面板加载器
    /// 职责：在 Start 时从 UIManager 的唯一 Canvas 动态实例化 MainMenuPanel prefab。
    /// UIManager 本身挂在同一场景的 UICanvas 上，并 DontDestroyOnLoad。
    /// </summary>
    public class MainMenuLoader : MonoBehaviour
    {
        [SerializeField] private UIPanel mainMenuPanelPrefab;
        [SerializeField] private UIPanel characterSelectPanelPrefab;

private void Start()
        {
            if (UIManager.Instance == null)
            {
                Debug.LogError("[MainMenuLoader] UIManager 不存在！");
                return;
            }

            // 加载 MainMenuPanel
            if (mainMenuPanelPrefab != null)
            {
                UIManager.Instance.LoadPanel<MainMenuPanel>(mainMenuPanelPrefab);
            }
            else
            {
                Debug.LogError("[MainMenuLoader] mainMenuPanelPrefab 未赋値！");
                return;
            }

            // 预加载 CharacterSelectPanel（隐藏状态，等待主菜单按鈕触发）
            if (characterSelectPanelPrefab != null)
            {
                UIManager.Instance.LoadPanel<CharacterSelectPanel>(characterSelectPanelPrefab);
            }
            else
            {
                Debug.LogWarning("[MainMenuLoader] characterSelectPanelPrefab 未赋値，选人界面将无法显示");
            }

            // 显示主菜单，隐藏其他
            UIManager.Instance.ShowPanel<MainMenuPanel>(hideOthers: true, addToStack: false);
            Debug.Log("[MainMenuLoader] MainMenuPanel 已显示");
        }
    }
}

