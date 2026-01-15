using UnityEngine;
using System;

namespace UI.Core
{
    public abstract class UIPanel : MonoBehaviour
    {
        [Header("Panel Settings")]
        [SerializeField] protected bool hideOnAwake = true;
        [SerializeField] protected bool useAnimation = true;
        [SerializeField] protected float animationDuration = 0.3f;

        public enum PanelState { Hidden, Showing, Visible, Hiding }
        
        private PanelState currentState = PanelState.Hidden;
        private UIAnimation uiAnimation;

        public event Action OnShown;
        public event Action OnHidden;

        protected virtual void Awake()
        {
            if (useAnimation)
            {
                uiAnimation = GetComponent<UIAnimation>();
                if (uiAnimation == null)
                {
                    uiAnimation = gameObject.AddComponent<UIFadeAnimation>();
                }
            }

            if (hideOnAwake)
            {
                gameObject.SetActive(false);
                currentState = PanelState.Hidden;
            }
            else
            {
                currentState = PanelState.Visible;
            }

            OnInit();
        }

        protected virtual void Start() { OnStart(); }
        protected virtual void OnInit() { }
        protected virtual void OnStart() { }

        public virtual void Show()
        {
            if (currentState == PanelState.Visible || currentState == PanelState.Showing)
                return;

            currentState = PanelState.Showing;
            gameObject.SetActive(true);
            OnBeforeShow();

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
                currentState = PanelState.Visible;
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

            if (useAnimation && uiAnimation != null)
            {
                uiAnimation.PlayHideAnimation(animationDuration, () =>
                {
                    gameObject.SetActive(false);
                    currentState = PanelState.Hidden;
                    OnAfterHide();
                    OnHidden?.Invoke();
                });
            }
            else
            {
                gameObject.SetActive(false);
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

        protected virtual void OnBeforeShow() { }
        protected virtual void OnAfterShow() { }
        protected virtual void OnBeforeHide() { }
        protected virtual void OnAfterHide() { }
        public virtual void Refresh() { }

        public PanelState CurrentState => currentState;
        public bool IsVisible => currentState == PanelState.Visible;
        public bool IsHidden => currentState == PanelState.Hidden;
        public bool IsAnimating => currentState == PanelState.Showing || currentState == PanelState.Hiding;
    }
}
