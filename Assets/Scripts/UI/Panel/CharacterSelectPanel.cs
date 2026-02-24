using System.Collections.Generic;
using Battle;
using ConfigHandler;
using TMPro;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panel
{
    /// <summary>
    /// 角色选择面板
    /// 职责：
    ///   1. 从 CharacterConfigDB 读取角色列表，动态生成卡片
    ///   2. 点击卡片 → 更新详情面板 + 高亮选中卡
    ///   3. 点击"开始"→ 写 GameSessionData → GameSceneManager.StartBattle
    /// 不持有任何 ECS / 战斗逻辑
    /// </summary>
    public class CharacterSelectPanel : UIPanel
    {
        // ── 卡片列表区域 ────────────────────────────────────────
        [Header("Card List")]
        [SerializeField] private Transform cardContainer;
        [SerializeField] private CharacterCardItem cardItemPrefab;

        // ── 详情面板 ────────────────────────────────────────────
        [Header("Detail Panel")]
        [SerializeField] private TextMeshProUGUI detailNameText;
        [SerializeField] private TextMeshProUGUI detailDescriptionText;
        [SerializeField] private TextMeshProUGUI detailTraitsText;

        // ── 按钮 ────────────────────────────────────────────────
        [Header("Buttons")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button backButton;

        // ── 运行时状态 ──────────────────────────────────────────
        private readonly List<CharacterCardItem> _cards = new List<CharacterCardItem>();
        private CharacterDef _selectedDef;
        private CharacterCardItem _selectedCard;

        // ── UIPanel 生命周期 ────────────────────────────────────

protected override void OnInit()
        {
            if (startButton != null)
            {
                startButton.onClick.AddListener(OnStartClicked);
                // 初始状态：隐藏且不可交互，防止设备快速单击穿透
                startButton.gameObject.SetActive(false);
                startButton.interactable = false;
            }

            if (backButton != null)
                backButton.onClick.AddListener(OnBackClicked);
        }

        protected override void OnAfterShow()
        {
            BuildCardList();
        }

        // ── 卡片构建 ────────────────────────────────────────────

private void BuildCardList()
        {
            // Clear old cards
            foreach (var c in _cards)
                if (c != null) Destroy(c.gameObject);
            _cards.Clear();

            _selectedDef  = null;
            _selectedCard = null;
            startButton?.gameObject.SetActive(false);
            ClearDetail();

            if (cardItemPrefab == null || cardContainer == null)
            {
                Debug.LogError("[CharacterSelectPanel] cardItemPrefab or cardContainer not assigned");
                return;
            }

            // Ensure config is loaded
            if (CharacterConfigDB.Instance == null)
                Battle.GameConfigLoader.LoadAll();

            var db = CharacterConfigDB.Instance;
            if (db == null)
            {
                Debug.LogError("[CharacterSelectPanel] CharacterConfigDB load failed");
                return;
            }

            // Optional registry for Sprite lookup
            var registry = Game.CharacterRegistryLoader.Instance;

            var allChars = db.GetAllCharacters();
            foreach (var def in allChars)
            {
                var card    = Instantiate(cardItemPrefab, cardContainer);
                var portrait = registry != null
                    ? registry.GetDefinition(def.id)?.CardSprite
                    : null;
                card.Bind(def, OnCardSelected, portrait);
                _cards.Add(card);
            }

            // Select first by default
            if (_cards.Count > 0)
                OnCardSelected(_cards[0].Data);
        }

        // ── 卡片选中回调 ────────────────────────────────────────

private void OnCardSelected(CharacterDef def)
        {
            _selectedDef = def;

            // 更新高亮
            foreach (var c in _cards)
                c.SetHighlight(c.Data == def);

            // 记录选中的卡片引用
            foreach (var c in _cards)
                if (c.Data == def) { _selectedCard = c; break; }

            // 更新详情
            RefreshDetail(def);

            // 有选择才开启按钮
            if (startButton != null)
            {
                startButton.gameObject.SetActive(true);
                startButton.interactable = true;
            }
        }

        // ── 详情面板 ────────────────────────────────────────────

        private void RefreshDetail(CharacterDef def)
        {
            if (detailNameText != null)
                detailNameText.text = def.displayName;

            if (detailDescriptionText != null)
                detailDescriptionText.text = def.description;

            if (detailTraitsText != null)
            {
                if (def.traits != null && def.traits.Count > 0)
                    detailTraitsText.text = "• " + string.Join("\n• ", def.traits);
                else
                    detailTraitsText.text = string.Empty;
            }
        }

        private void ClearDetail()
        {
            if (detailNameText != null) detailNameText.text = string.Empty;
            if (detailDescriptionText != null) detailDescriptionText.text = string.Empty;
            if (detailTraitsText != null) detailTraitsText.text = string.Empty;
        }

        // ── 按钮回调 ────────────────────────────────────────────

private void OnStartClicked()
        {
            if (_selectedDef == null)
            {
                Debug.LogWarning("[CharacterSelectPanel] 未选择角色，无法开始战斗");
                return;
            }

            // 立即禁用，防止重复点击
            startButton.interactable = false;

            Session.GameSessionData.SelectCharacter(_selectedDef.id);
            Debug.Log($"[CharacterSelectPanel] 选择角色: {_selectedDef.id}，进入战斗");

            Game.GameSceneManager.Instance?.StartBattle();
        }

private void OnBackClicked()
        {
            // 直接切回 MainMenuPanel，不跳到新场景
            if (UIManager.Instance != null)
                UIManager.Instance.ShowPanel<MainMenuPanel>(hideOthers: true, addToStack: false);
            else
                Game.GameSceneManager.Instance?.LoadMainMenu();
        }

        // ── 清理 ────────────────────────────────────────────────

        private void OnDestroy()
        {
            if (startButton != null) startButton.onClick.RemoveListener(OnStartClicked);
            if (backButton != null)  backButton.onClick.RemoveListener(OnBackClicked);
        }
    }
}
