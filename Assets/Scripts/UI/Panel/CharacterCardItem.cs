using ConfigHandler;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panel
{
    /// <summary>
    /// 单个角色卡片 UI 元件
    /// 只负责展示数据，选中回调通过外部注入
    /// </summary>
    public class CharacterCardItem : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button selectButton;
        [SerializeField] private Image highlightBorder;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        private CharacterDef _data;
        private System.Action<CharacterDef> _onSelected;

        public CharacterDef Data => _data;

        private void Awake()
        {
            if (selectButton != null)
                selectButton.onClick.AddListener(OnClicked);
        }

        /// <summary>
        /// 由 CharacterSelectPanel 调用，绑定数据和回调
        /// </summary>
        public void Bind(CharacterDef def, System.Action<CharacterDef> onSelected)
        {
            _data = def;
            _onSelected = onSelected;

            if (nameText != null)
                nameText.text = def.displayName;

            if (descriptionText != null)
                descriptionText.text = def.description;

            SetHighlight(false);
        }

        public void SetHighlight(bool on)
        {
            if (highlightBorder != null)
                highlightBorder.enabled = on;
        }

        private void OnClicked()
        {
            _onSelected?.Invoke(_data);
        }

        private void OnDestroy()
        {
            if (selectButton != null)
                selectButton.onClick.RemoveListener(OnClicked);
        }
    }
}
