using UI.Model;

namespace UI.Core
{
    /// <summary>
    /// 支持自动 ViewModel 注入的 Panel
    /// </summary>
    public abstract class UIBindPanel<T> : UIPanel where T : class
    {
        protected T ViewModel { get; private set; }

        protected override void OnAfterShow()
        {
            base.OnAfterShow();

            ViewModel = ViewModelRegistry.Get<T>();

            if (ViewModel != null)
                OnViewModelReady();
        }

        protected override void OnBeforeHide()
        {
            OnViewModelDispose();
            ViewModel = null;

            base.OnBeforeHide();
        }

        /// <summary>
        /// 当 ViewModel 获取成功时调用
        /// </summary>
        protected virtual void OnViewModelReady() { }

        /// <summary>
        /// 隐藏时调用（用于取消订阅）
        /// </summary>
        protected virtual void OnViewModelDispose() { }
    }
}