using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Core
{
    /// <summary>
    /// UI管理器 - 单例
    /// 统一管理所有UI面板的显示、隐藏、层级
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Root")] [SerializeField] private Canvas mainCanvas;
        [SerializeField] private Transform panelContainer;

        [Header("Settings")] [SerializeField] private bool hideAllOnStart = false;

        // 面板注册表
        private Dictionary<Type, UIPanel> panels = new Dictionary<Type, UIPanel>();
        private Stack<UIPanel> panelStack = new Stack<UIPanel>(); // 面板栈,用于返回上一个面板

        // 当前显示的面板
        private UIPanel currentPanel;

        // 事件
        public event Action<UIPanel> OnPanelShown;
        public event Action<UIPanel> OnPanelHidden;

        private void Awake()
        {
            // 单例模式
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            // 如果是全局UI管理器,设置为 DontDestroyOnLoad
            // DontDestroyOnLoad(gameObject);

            InitializeManager();
        }

        private void Start()
        {
            DontDestroyOnLoad(mainCanvas);
            
            if (hideAllOnStart)
            {
                HideAllPanels();
            }
        }

        /// <summary>
        /// 初始化管理器
        /// </summary>
        private void InitializeManager()
        {
            // 自动查找所有UI面板并注册
            if (panelContainer != null)
            {
                var allPanels = panelContainer.GetComponentsInChildren<UIPanel>(true);
                foreach (var panel in allPanels)
                {
                    RegisterPanel(panel);
                }

                Debug.Log($"[UIManager] 自动注册了 {allPanels.Length} 个面板");
            }
        }

        /// <summary>
        /// 注册面板
        /// </summary>
        public void RegisterPanel<T>(T panel) where T : UIPanel
        {
            Type type = typeof(T);

            if (panels.ContainsKey(type))
            {
                Debug.LogWarning($"[UIManager] 面板 {type.Name} 已经注册过了");
                return;
            }

            panels[type] = panel;

            // 监听面板事件
            panel.OnShown += () => HandlePanelShown(panel);
            panel.OnHidden += () => HandlePanelHidden(panel);

            Debug.Log($"[UIManager] 注册面板: {type.Name}");
        }

        /// <summary>
        /// 注册面板 (非泛型版本)
        /// </summary>
        public void RegisterPanel(UIPanel panel)
        {
            Type type = panel.GetType();

            if (panels.ContainsKey(type))
            {
                Debug.LogWarning($"[UIManager] 面板 {type.Name} 已经注册过了");
                return;
            }

            panels[type] = panel;

            // 监听面板事件
            panel.OnShown += () => HandlePanelShown(panel);
            panel.OnHidden += () => HandlePanelHidden(panel);

            Debug.Log($"[UIManager] 注册面板: {type.Name}");
        }

        /// <summary>
        /// 显示面板
        /// </summary>
        public T ShowPanel<T>(bool hideOthers = false, bool addToStack = true) where T : UIPanel
        {
            Type type = typeof(T);

            if (!panels.TryGetValue(type, out UIPanel panel))
            {
                Debug.LogError($"[UIManager] 面板 {type.Name} 未注册");
                return null;
            }

            // 如果需要隐藏其他面板
            if (hideOthers)
            {
                HideAllPanels(panel);
            }

            // 添加到面板栈
            if (addToStack && currentPanel != null && currentPanel != panel)
            {
                panelStack.Push(currentPanel);
            }

            // 显示面板
            panel.Show();
            currentPanel = panel;

            return panel as T;
        }

        /// <summary>
        /// 隐藏面板
        /// </summary>
        public void HidePanel<T>() where T : UIPanel
        {
            Type type = typeof(T);

            if (!panels.TryGetValue(type, out UIPanel panel))
            {
                Debug.LogError($"[UIManager] 面板 {type.Name} 未注册");
                return;
            }

            panel.Hide();

            if (currentPanel == panel)
            {
                currentPanel = null;
            }
        }

        /// <summary>
        /// 获取面板
        /// </summary>
        public T GetPanel<T>() where T : UIPanel
        {
            Type type = typeof(T);

            if (panels.TryGetValue(type, out UIPanel panel))
            {
                return panel as T;
            }

            Debug.LogWarning($"[UIManager] 面板 {type.Name} 未注册");
            return null;
        }

        /// <summary>
        /// 切换面板 (显示目标面板,隐藏当前面板)
        /// </summary>
        public T SwitchPanel<T>(bool addToStack = true) where T : UIPanel
        {
            return ShowPanel<T>(hideOthers: true, addToStack: addToStack);
        }

        /// <summary>
        /// 返回上一个面板
        /// </summary>
        public UIPanel GoBack()
        {
            if (panelStack.Count == 0)
            {
                Debug.LogWarning("[UIManager] 面板栈为空,无法返回");
                return null;
            }

            // 隐藏当前面板
            if (currentPanel != null)
            {
                currentPanel.Hide();
            }

            // 显示上一个面板
            UIPanel previousPanel = panelStack.Pop();
            previousPanel.Show();
            currentPanel = previousPanel;

            return previousPanel;
        }

        /// <summary>
        /// 清空面板栈
        /// </summary>
        public void ClearStack()
        {
            panelStack.Clear();
        }

        /// <summary>
        /// 隐藏所有面板
        /// </summary>
        public void HideAllPanels(UIPanel except = null)
        {
            foreach (var panel in panels.Values)
            {
                if (panel != except && panel.IsVisible)
                {
                    panel.Hide();
                }
            }
        }

        /// <summary>
        /// 刷新所有面板
        /// </summary>
        public void RefreshAllPanels()
        {
            foreach (var panel in panels.Values)
            {
                if (panel.IsVisible)
                {
                    panel.Refresh();
                }
            }
        }

        /// <summary>
        /// 检查面板是否显示
        /// </summary>
        public bool IsPanelVisible<T>() where T : UIPanel
        {
            Type type = typeof(T);

            if (panels.TryGetValue(type, out UIPanel panel))
            {
                return panel.IsVisible;
            }

            return false;
        }

        // ========== 事件处理 ==========

        private void HandlePanelShown(UIPanel panel)
        {
            Debug.Log($"[UIManager] 面板显示: {panel.GetType().Name}");
            OnPanelShown?.Invoke(panel);
        }

        private void HandlePanelHidden(UIPanel panel)
        {
            Debug.Log($"[UIManager] 面板隐藏: {panel.GetType().Name}");
            OnPanelHidden?.Invoke(panel);
        }

        // ========== 属性 ==========

        public UIPanel CurrentPanel => currentPanel;
        public int StackCount => panelStack.Count;

        private void OnDestroy()
        {
            // 清理事件
            foreach (var panel in panels.Values)
            {
                if (panel != null)
                {
                    panel.OnShown -= () => HandlePanelShown(panel);
                    panel.OnHidden -= () => HandlePanelHidden(panel);
                }
            }

            panels.Clear();
            panelStack.Clear();
        }
    }
}