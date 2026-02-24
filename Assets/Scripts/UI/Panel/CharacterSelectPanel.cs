using System;
using System.Collections.Generic;
using Battle;
using ConfigHandler;
using Game;
using Session;
using UI.Core;
using UnityEngine;

namespace UI.Panel
{
    /// <summary>
    /// 角色选择面板（Panel 模式，内嵌于 BattleScene）
    /// 职责：
    ///   1. 从 CharacterConfigDB 读取角色列表，动态生成卡片
    ///   2. 点击卡片 → 更新详情面板 + 高亮选中卡
    ///   3. 点击"确认" → 写 GameSessionData → invoke GameEvents.OnBattleStartRequested
    ///   4. 点击"返回" → 回到 StartMenuPanel
    /// 不持有任何 ECS / 战斗逻辑
    /// </summary>
    public class CharacterSelectPanel : UIPanel
    {
        // ── 卡片列表区域 ────────────────────────────────────────
        [Header("Card List")] [SerializeField] private Transform cardContainer;
        [SerializeField] private CharacterCardItem cardItemPrefab;

        // ── 运行时状态 ──────────────────────────────────────────
        private readonly List<CharacterCardItem> _cards = new List<CharacterCardItem>();
        private CharacterDef _selectedDef;
        private CharacterCardItem _selectedCard;

        /// <summary>
        /// 选角确认后触发（已写入 GameSessionData）。
        /// 供外部（PlaceholderBattleMode 或 ECSGameManager）监听。
        /// </summary>
        public event Action OnConfirmed;

        // ── UIPanel 生命周期 ────────────────────────────────────
        protected override void OnAfterShow()
        {
            BuildCardList();
        }

        // ── 卡片构建 ────────────────────────────────────────────

        private void BuildCardList()
        {
            foreach (var c in _cards)
                if (c != null)
                    Destroy(c.gameObject);
            _cards.Clear();

            _selectedDef = null;
            _selectedCard = null;

            if (cardItemPrefab == null || cardContainer == null)
            {
                Debug.LogError("[CharacterSelectPanel] cardItemPrefab or cardContainer not assigned");
                return;
            }

            if (CharacterConfigDB.Instance == null)
                GameConfigLoader.LoadAll();

            var db = CharacterConfigDB.Instance;
            if (db == null)
            {
                Debug.LogError("[CharacterSelectPanel] CharacterConfigDB load failed");
                return;
            }

            var registry = CharacterRegistryLoader.Instance;

            var allChars = db.GetAllCharacters();
            foreach (var def in allChars)
            {
                var card = Instantiate(cardItemPrefab, cardContainer);
                var portrait = registry != null
                    ? registry.GetDefinition(def.id)?.CardSprite
                    : null;
                card.Bind(def, OnCardSelected, portrait);
                _cards.Add(card);
            }
        }

        // ── 卡片选中回调 ────────────────────────────────────────

        private void OnCardSelected(CharacterDef def)
        {
            _selectedDef = def;

            foreach (var c in _cards)
                c.SetHighlight(c.Data == def);

            foreach (var c in _cards)
                if (c.Data == def)
                {
                    _selectedCard = c;
                    break;
                }

            OnStartClicked();
        }

        // ── 按钮回调 ────────────────────────────────────────────

        private void OnStartClicked()
        {
            if (_selectedDef == null)
            {
                Debug.LogWarning("[CharacterSelectPanel] 未选择角色，无法开始战斗");
                return;
            }

            GameSessionData.SelectCharacter(_selectedDef.id);
            Debug.Log($"[CharacterSelectPanel] 选择角色: {_selectedDef.id}，触发战斗切换");

            // 通知外部（ECSGameManager）切换模式
            OnConfirmed?.Invoke();
            GameEvents.RequestBattleStart();
        }

        // ── 清理 ────────────────────────────────────────────────
    }
}