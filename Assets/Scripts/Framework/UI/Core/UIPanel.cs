using System;
using UnityEngine;

namespace UI.Core
{
    /// <summary>
    /// UIPanel 基类
    /// 统一管理：
    /// 1. 显示隐藏状态
    /// 2. CanvasGroup 控制
    /// 3. 显示隐藏动画
    /// 4. 生命周期 Hook
    /// 
    /// 子类职责：
    /// - 只关心 UI 逻辑
    /// - 不要直接操作 CanvasGroup
    /// - 不要自己控制 alpha
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIPanel : MonoBehaviour
    {
        [Header("Panel Settings")]
        [SerializeField] protected bool hideOnAwake = true;
        [SerializeField] protected bool useAnimation = true;
        [SerializeField] protected float animationDuration = 0.25f;

        /// <summary>
        /// 面板状态机
        /// </summary>
        public enum PanelState
        {
            Hidden,     // 完全隐藏
            Showing,    // 正在显示动画
            Visible,    // 完全可见
            Hiding      // 正在隐藏动画
        }

        private PanelState currentState = PanelState.Hidden;

        protected CanvasGroup canvasGroup;
        private UIAnimation uiAnimation;

        public event Action OnShown;   // 显示完成
        public event Action OnHidden;  // 隐藏完成

        #region Unity Lifecycle

        /// <summary>
        /// 初始化阶段
        /// 职责：
        /// - 获取组件引用
        /// - 初始化动画模块
        /// - 设置初始可见状态
        /// - 调用 OnInit（子类初始化UI引用）
        /// </summary>
        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            if (useAnimation)
            {
                uiAnimation = GetComponent<UIAnimation>();
                if (uiAnimation == null)
                    uiAnimation = gameObject.AddComponent<UIFadeAnimation>();
            }

            if (hideOnAwake)
                SetHiddenInstant();
            else
                SetVisibleInstant();

            OnInit();
        }

        /// <summary>
        /// Start阶段
        /// 适合：
        /// - 注册事件
        /// - 请求数据
        /// </summary>
        protected virtual void Start()
        {
            OnStart();
        }

        /// <summary>
        /// 初始化UI引用
        /// 推荐用途：
        /// - GetComponent
        /// - 绑定按钮点击
        /// - 本地数据初始化
        /// ⚠ 不要在这里请求外部系统数据
        /// </summary>
        protected virtual void OnInit() { }

        /// <summary>
        /// 面板正式启动
        /// 推荐用途：
        /// - 注册事件监听
        /// - 请求系统数据
        /// </summary>
        protected virtual void OnStart() { }

        #endregion

        #region Public API

        /// <summary>
        /// 显示面板
        /// </summary>
        public virtual void Show()
        {
            if (IsVisible || IsAnimating)
                return;

            currentState = PanelState.Showing;

            OnBeforeShow();

            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;

            if (useAnimation && uiAnimation != null)
            {
                uiAnimation.PlayShowAnimation(animationDuration, () =>
                {
                    currentState = PanelState.Visible;
                    OnAfterShow();
                    OnShown?.Invoke();
                });
            }
            else
            {
                SetVisibleInstant();
                OnAfterShow();
                OnShown?.Invoke();
            }
        }

        /// <summary>
        /// 隐藏面板
        /// </summary>
        public virtual void Hide()
        {
            if (IsHidden || IsAnimating)
                return;

            currentState = PanelState.Hiding;

            OnBeforeHide();

            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            if (useAnimation && uiAnimation != null)
            {
                uiAnimation.PlayHideAnimation(animationDuration, () =>
                {
                    SetHiddenInstant();
                    currentState = PanelState.Hidden;
                    OnAfterHide();
                    OnHidden?.Invoke();
                });
            }
            else
            {
                SetHiddenInstant();
                currentState = PanelState.Hidden;
                OnAfterHide();
                OnHidden?.Invoke();
            }
        }

        /// <summary>
        /// 强制立即显示（无动画）
        /// </summary>
        public void ForceShow()
        {
            SetVisibleInstant();
            OnAfterShow();
            OnShown?.Invoke();
        }

        /// <summary>
        /// 强制立即隐藏（无动画）
        /// </summary>
        public void ForceHide()
        {
            SetHiddenInstant();
            OnAfterHide();
            OnHidden?.Invoke();
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public void Toggle()
        {
            if (IsVisible) Hide();
            else Show();
        }

        /// <summary>
        /// 刷新UI
        /// 推荐用于：
        /// - 数据变化时重绘
        /// - 不涉及显示隐藏
        /// </summary>
        public virtual void Refresh() { }

        #endregion

        #region Internal State Control

        /// <summary>
        /// 立即可见（无动画）
        /// </summary>
        protected void SetVisibleInstant()
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            currentState = PanelState.Visible;
        }

        /// <summary>
        /// 立即隐藏（无动画）
        /// </summary>
        protected void SetHiddenInstant()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            currentState = PanelState.Hidden;
        }

        #endregion

        #region Hooks（子类扩展点）

        /// <summary>
        /// 显示动画开始前
        /// 适合：
        /// - 重置UI状态
        /// - 播放音效
        /// </summary>
        protected virtual void OnBeforeShow() { }

        /// <summary>
        /// 显示完成后
        /// 适合：
        /// - 刷新数据
        /// - 设置焦点
        /// </summary>
        protected virtual void OnAfterShow() { }

        /// <summary>
        /// 隐藏动画开始前
        /// 适合：
        /// - 保存数据
        /// - 停止计时器
        /// </summary>
        protected virtual void OnBeforeHide() { }

        /// <summary>
        /// 隐藏完成后
        /// 适合：
        /// - 通知UI管理器
        /// - 释放资源
        /// </summary>
        protected virtual void OnAfterHide() { }

        #endregion

        #region Properties

        public PanelState CurrentState => currentState;
        public bool IsVisible => currentState == PanelState.Visible;
        public bool IsHidden => currentState == PanelState.Hidden;
        public bool IsAnimating => currentState == PanelState.Showing || currentState == PanelState.Hiding;

        #endregion
    }
}
