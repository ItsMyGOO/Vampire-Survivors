using System;
using System.Collections;
using UnityEngine;

namespace UI.Core
{
    /// <summary>
    /// UI淡入淡出动画
    /// </summary>
    public class UIFadeAnimation : UIAnimation
    {
        private CanvasGroup canvasGroup;
        private Coroutine currentAnimation;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        public override void PlayShowAnimation(float duration, Action onComplete)
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
            }

            currentAnimation = StartCoroutine(FadeIn(duration, onComplete));
        }

        public override void PlayHideAnimation(float duration, Action onComplete)
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
            }

            currentAnimation = StartCoroutine(FadeOut(duration, onComplete));
        }

        private IEnumerator FadeIn(float duration, Action onComplete)
        {
            float elapsed = 0f;
            float startAlpha = canvasGroup.alpha;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime; // 使用 unscaledDeltaTime 避免受暂停影响
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, elapsed / duration);
                yield return null;
            }

            canvasGroup.alpha = 1f;
            currentAnimation = null;
            onComplete?.Invoke();
        }

        private IEnumerator FadeOut(float duration, Action onComplete)
        {
            float elapsed = 0f;
            float startAlpha = canvasGroup.alpha;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
                yield return null;
            }

            canvasGroup.alpha = 0f;
            currentAnimation = null;
            onComplete?.Invoke();
        }
    }
}
