using System.Collections.Generic;
using Battle.Player;
using Battle.Upgrade;
using UI.Core;
using UnityEngine;

namespace UI.Panel
{
    /// <summary>
    /// 升级选择面板 - View 层
    /// 显示三个升级选项卡片，玩家选择后触发升级并关闭面板
    /// 
    /// 接入方式：
    /// UpgradeService.OnUpgradeOptionsReady → UpgradeSelectPanel.Show(options)
    /// 玩家点击卡片 → UpgradeApplyService.ApplyUpgradeOption → Panel.Hide()
    /// </summary>
    public class UpgradeSelectPanel : UIPanel
    {
        [Header("Option Items")]
        [SerializeField] private UpgradeOptionItem[] optionItems;

        [Header("Settings")]
        [SerializeField] private bool pauseGameOnShow = true;

        private List<UpgradeOption> _currentOptions;

protected override void OnInit()
        {
            if (optionItems == null || optionItems.Length == 0)
                Debug.LogError("[UpgradeSelectPanel] optionItems 未配置，请在 Inspector 中绑定三个 UpgradeOptionItem");
        }

protected override void OnStart()
        {
            // 等 PlayerContext 就绪后再订阅（Battle 场景 Start 顺序保证）
            var upgradeService = PlayerContext.Instance?.UpgradeService;
            if (upgradeService == null)
            {
                Debug.LogError("[UpgradeSelectPanel] UpgradeService 未注册，无法订阅升级事件");
                return;
            }

            upgradeService.OnUpgradeOptionsReady += ShowOptions;
            upgradeService.testMode = false; // 关闭自动选择，由 UI 接管
            Debug.Log("[UpgradeSelectPanel] 已订阅 OnUpgradeOptionsReady");
        }


        /// <summary>
        /// 由 UpgradeService.OnUpgradeOptionsReady 事件调用
        /// </summary>
        public void ShowOptions(List<UpgradeOption> options)
        {
            if (options == null || options.Count == 0)
            {
                Debug.LogWarning("[UpgradeSelectPanel] 收到空的升级选项，忽略");
                return;
            }

            _currentOptions = options;
            BindOptions(options);
            Show();
        }

        protected override void OnBeforeShow()
        {
            if (pauseGameOnShow)
                Time.timeScale = 0f;
        }

        protected override void OnAfterHide()
        {
            if (pauseGameOnShow)
                Time.timeScale = 1f;
        }

        /// <summary>
        /// 将选项数据绑定到卡片
        /// </summary>
        private void BindOptions(List<UpgradeOption> options)
        {
            for (int i = 0; i < optionItems.Length; i++)
            {
                if (optionItems[i] == null) continue;

                if (i < options.Count)
                {
                    optionItems[i].gameObject.SetActive(true);
                    optionItems[i].Bind(options[i], OnOptionSelected);
                }
                else
                {
                    // 选项不足时隐藏多余的卡片
                    optionItems[i].gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 玩家点击某张卡片
        /// </summary>
        private void OnOptionSelected(UpgradeOption option)
        {
            Debug.Log($"[UpgradeSelectPanel] 玩家选择: {option.type} - {option.name}");

            // 触发升级应用
            var applyService = PlayerContext.Instance?.UpgradeApplyService;
            if (applyService == null)
            {
                Debug.LogError("[UpgradeSelectPanel] UpgradeApplyService 未注册");
                return;
            }

            applyService.ApplyUpgradeOption(option);

            // 关闭面板
            Hide();
        }
    

private void OnDestroy()
        {
            var upgradeService = PlayerContext.Instance?.UpgradeService;
            if (upgradeService != null)
                upgradeService.OnUpgradeOptionsReady -= ShowOptions;
        }
}
}
