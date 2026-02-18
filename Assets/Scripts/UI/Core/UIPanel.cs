using UnityEngine;
using System;

namespace UI.Core
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIPanel : MonoBehaviour
    {
        [Header("Panel Settings")]
        [SerializeField] protected bool hideOnAwake = true;
        [SerializeField] protected bool useAnimation = true;
        [SerializeField] protected float animationDuration = 0.25f;

        public enum PanelState { Hidden, Showing, Visible, Hiding }

        private PanelState currentState = PanelState.Hidden;

        protected CanvasGroup canvasGroup;
        private UIAnimation uiAnimation;

        public event Action OnShown;
        public event Action OnHidden;

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if(canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            if (useAnimation)
            {
                uiAnimation = GetComponent<UIAnimation>();
                if (uiAnimation == null)
                {
                    uiAnimation = gameObject.AddComponent<UIFadeAnimation>();
                }
            }

            // ⚠ 不再禁用 GameObject
            if (hideOnAwake)
                SetHiddenInstant();
            else
                SetVisibleInstant();

            OnInit();
        }

        protected virtual void Start()
        {
            OnStart();
        }

        protected virtual void OnInit() { }
        protected virtual void OnStart() { }

        #endregion

        #region Public API

        public virtual void Show()
        {
            if (currentState == PanelState.Visible || currentState == PanelState.Showing)
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

        public virtual void Hide()
        {
            if (currentState == PanelState.Hidden || currentState == PanelState.Hiding)
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

        public void Toggle()
        {
            if (IsVisible) Hide();
            else Show();
        }

        public virtual void Refresh() { }

        #endregion

        #region Internal State Control

        protected void SetVisibleInstant()
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            currentState = PanelState.Visible;
        }

        protected void SetHiddenInstant()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            currentState = PanelState.Hidden;
        }

        #endregion

        #region Hooks

        protected virtual void OnBeforeShow() { }
        protected virtual void OnAfterShow() { }
        protected virtual void OnBeforeHide() { }
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
