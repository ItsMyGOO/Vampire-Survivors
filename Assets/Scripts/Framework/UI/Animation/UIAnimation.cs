using System;
using UnityEngine;

namespace UI.Core
{
    /// <summary>
    /// UI动画基类
    /// </summary>
    public abstract class UIAnimation : MonoBehaviour
    {
        public abstract void PlayShowAnimation(float duration, Action onComplete);
        public abstract void PlayHideAnimation(float duration, Action onComplete);
    }
}
