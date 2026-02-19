using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Core
{
    /// <summary>
    /// UI管理器 - 单例
    /// 统一管理所有UI面板的显示、隐藏、层级
    /// （修复版）
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Root")]
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private Transform panelContainer;

        [Header("Settings")]
        [SerializeField] private bool hideAllOnStart = false;

        private Dictionary<Type, UIPanel> panels = new();
        private Stack<UIPanel> panelStack = new();

        private UIPanel currentPanel;

        // 🔥 修复：保存委托引用
        private Dictionary<UIPanel, Action> shownCallbacks = new();
        private Dictionary<UIPanel, Action> hiddenCallbacks = new();

        public event Action<UIPanel> OnPanelShown;
        public event Action<UIPanel> OnPanelHidden;

        #region Unity Lifecycle

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            InitializeManager();
        }

        private void Start()
        {
            if (mainCanvas != null)
                DontDestroyOnLoad(mainCanvas.gameObject);

            if (hideAllOnStart)
                HideAllPanels();
        }

        private void OnDestroy()
        {
            foreach (var panel in panels.Values)
            {
                if (panel == null) continue;

                if (shownCallbacks.TryGetValue(panel, out var shown))
                    panel.OnShown -= shown;

                if (hiddenCallbacks.TryGetValue(panel, out var hidden))
                    panel.OnHidden -= hidden;
            }

            panels.Clear();
            panelStack.Clear();
            shownCallbacks.Clear();
            hiddenCallbacks.Clear();
        }

        #endregion

        #region Initialization

        private void InitializeManager()
        {
            if (panelContainer == null) return;

            var allPanels = panelContainer.GetComponentsInChildren<UIPanel>(true);

            foreach (var panel in allPanels)
            {
                RegisterPanel(panel);
            }

            Debug.Log($"[UIManager] 自动注册 {allPanels.Length} 个面板");
        }

        #endregion

        #region Register

        public void RegisterPanel(UIPanel panel)
        {
            Type type = panel.GetType();

            if (panels.ContainsKey(type))
            {
                Debug.LogWarning($"[UIManager] 面板 {type.Name} 已注册");
                return;
            }

            panels[type] = panel;

            // 🔥 正确保存委托引用
            Action onShown = () => HandlePanelShown(panel);
            Action onHidden = () => HandlePanelHidden(panel);

            shownCallbacks[panel] = onShown;
            hiddenCallbacks[panel] = onHidden;

            panel.OnShown += onShown;
            panel.OnHidden += onHidden;

            Debug.Log($"[UIManager] 注册面板: {type.Name}");
        }

        #endregion

        #region Show / Hide

        public T ShowPanel<T>(bool hideOthers = false, bool addToStack = true) where T : UIPanel
        {
            if (!panels.TryGetValue(typeof(T), out var panel))
            {
                Debug.LogError($"[UIManager] 面板 {typeof(T).Name} 未注册");
                return null;
            }

            if (panel == currentPanel)
                return panel as T;

            if (hideOthers)
                HideAllPanels(panel);

            if (addToStack && currentPanel != null && currentPanel != panel)
            {
                if (!panelStack.Contains(currentPanel))
                    panelStack.Push(currentPanel);
            }

            panel.Show();
            currentPanel = panel;

            return panel as T;
        }

        public void HidePanel<T>() where T : UIPanel
        {
            if (!panels.TryGetValue(typeof(T), out var panel))
            {
                Debug.LogError($"[UIManager] 面板 {typeof(T).Name} 未注册");
                return;
            }

            panel.Hide();

            if (currentPanel == panel)
                currentPanel = null;
        }

        public UIPanel GoBack()
        {
            if (panelStack.Count == 0)
            {
                Debug.LogWarning("[UIManager] 面板栈为空");
                return null;
            }

            if (currentPanel != null)
                currentPanel.Hide();

            var previous = panelStack.Pop();
            previous.Show();
            currentPanel = previous;

            return previous;
        }

        public void ClearStack()
        {
            panelStack.Clear();
        }

        #endregion

        #region Utilities

        public T GetPanel<T>() where T : UIPanel
        {
            if (panels.TryGetValue(typeof(T), out var panel))
                return panel as T;

            return null;
        }

        public bool IsPanelVisible<T>() where T : UIPanel
        {
            if (panels.TryGetValue(typeof(T), out var panel))
                return panel.IsVisible;

            return false;
        }

        public void HideAllPanels(UIPanel except = null)
        {
            foreach (var panel in panels.Values)
            {
                if (panel != except && panel.IsVisible)
                    panel.Hide();
            }

            if (except == null)
                currentPanel = null;
        }

        public void RefreshAllPanels()
        {
            foreach (var panel in panels.Values)
            {
                if (panel.IsVisible)
                    panel.Refresh();
            }
        }

        #endregion

        #region Event Handlers

        private void HandlePanelShown(UIPanel panel)
        {
            currentPanel = panel;
            OnPanelShown?.Invoke(panel);
        }

        private void HandlePanelHidden(UIPanel panel)
        {
            if (currentPanel == panel)
                currentPanel = null;

            OnPanelHidden?.Invoke(panel);
        }

        #endregion

        #region Properties

        public UIPanel CurrentPanel => currentPanel;
        public int StackCount => panelStack.Count;

        #endregion
    }
}
