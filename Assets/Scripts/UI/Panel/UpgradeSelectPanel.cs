using System.Collections.Generic;
using Battle.Upgrade;
using UI.Core;
using UnityEngine;

namespace UI.Panel
{
    /// <summary>
    /// 升级选择面板 - View 层
    /// 服务通过 BindServices 由外部注入，不直接访问全局状态
    /// </summary>
    public class UpgradeSelectPanel : UIPanel
    {
        [Header("Option Items")]
        [SerializeField] private UpgradeOptionItem[] optionItems;

        [Header("Settings")]
        [SerializeField] private bool pauseGameOnShow = true;

        private UpgradeService _upgradeService;
        private UpgradeApplyService _applyService;
        private List<UpgradeOption> _currentOptions;

        protected override void OnInit()
        {
            if (optionItems == null || optionItems.Length == 0)
                Debug.LogError("[UpgradeSelectPanel] optionItems 未配置，请在 Inspector 中绑定三个 UpgradeOptionItem");
        }

        /// <summary>
        /// 由 ECSGameManager 在场景初始化后调用，注入服务
        /// </summary>
        public void BindServices(UpgradeService upgradeService, UpgradeApplyService applyService)
        {
            _upgradeService = upgradeService;
            _applyService = applyService;

            _upgradeService.OnUpgradeOptionsReady += ShowOptions;
            _upgradeService.testMode = false; // 关闭自动选择，由 UI 接管
            Debug.Log("[UpgradeSelectPanel] 已绑定服务并订阅 OnUpgradeOptionsReady");
        }

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
                    optionItems[i].gameObject.SetActive(false);
                }
            }
        }

        private void OnOptionSelected(UpgradeOption option)
        {
            Debug.Log($"[UpgradeSelectPanel] 玩家选择: {option.type} - {option.name}");

            if (_applyService == null)
            {
                Debug.LogError("[UpgradeSelectPanel] UpgradeApplyService 未绑定");
                return;
            }

            _applyService.ApplyUpgradeOption(option);
            Hide();
        }

        private void OnDestroy()
        {
            if (_upgradeService != null)
                _upgradeService.OnUpgradeOptionsReady -= ShowOptions;
        }
    }
}