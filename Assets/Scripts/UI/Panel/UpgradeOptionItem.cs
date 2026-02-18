using System;
using Battle.Upgrade;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panel
{
    /// <summary>
    /// 单个升级选项卡片 - View 层
    /// 负责显示一条 UpgradeOption 数据，并将点击事件通知外部
    /// </summary>
    public class UpgradeOptionItem : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Image iconImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Button selectButton;

        [Header("Type Colors")]
        [SerializeField] private Color weaponColor = new Color(0.9f, 0.6f, 0.2f, 1f);
        [SerializeField] private Color passiveColor = new Color(0.3f, 0.7f, 0.9f, 1f);

        private UpgradeOption _option;
        private Action<UpgradeOption> _onSelected;

        private void Awake()
        {
            selectButton.onClick.AddListener(OnClickSelect);
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        public void Bind(UpgradeOption option, Action<UpgradeOption> onSelected)
        {
            _option = option;
            _onSelected = onSelected;

            nameText.text = option.name;
            descriptionText.text = option.description;
            levelText.text = $"Lv.{option.nextLevel}";

            if (backgroundImage != null)
            {
                backgroundImage.color = option.type == UpgradeOptionType.Weapon ? weaponColor : passiveColor;
            }
        }

        private void OnClickSelect()
        {
            _onSelected?.Invoke(_option);
        }

        private void OnDestroy()
        {
            selectButton.onClick.RemoveListener(OnClickSelect);
        }
    }
}
