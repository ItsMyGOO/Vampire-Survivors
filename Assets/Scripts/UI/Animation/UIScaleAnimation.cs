using System;
using System.Collections;
using UnityEngine;

namespace UI.Core
{
    /// <summary>
    /// UI缩放动画
    /// </summary>
    public class UIScaleAnimation : UIAnimation
    {
        [Header("Scale Settings")]
        [SerializeField] private Vector3 showFromScale = new Vector3(0.5f, 0.5f, 1f);
        [SerializeField] private Vector3 showToScale = Vector3.one;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Coroutine currentAnimation;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            
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

            currentAnimation = StartCoroutine(ScaleIn(duration, onComplete));
        }

        public override void PlayHideAnimation(float duration, Action onComplete)
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
            }

            currentAnimation = StartCoroutine(ScaleOut(duration, onComplete));
        }

        private IEnumerator ScaleIn(float duration, Action onComplete)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                float curveValue = scaleCurve.Evaluate(t);

                if (rectTransform != null)
                {
                    rectTransform.localScale = Vector3.Lerp(showFromScale, showToScale, curveValue);
                }

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = Mathf.Lerp(0f, 1f, curveValue);
                }

                yield return null;
            }

            if (rectTransform != null)
            {
                rectTransform.localScale = showToScale;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }

            currentAnimation = null;
            onComplete?.Invoke();
        }

        private IEnumerator ScaleOut(float duration, Action onComplete)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                float curveValue = scaleCurve.Evaluate(t);

                if (rectTransform != null)
                {
                    rectTransform.localScale = Vector3.Lerp(showToScale, showFromScale, curveValue);
                }

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = Mathf.Lerp(1f, 0f, curveValue);
                }

                yield return null;
            }

            if (rectTransform != null)
            {
                rectTransform.localScale = showFromScale;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }

            currentAnimation = null;
            onComplete?.Invoke();
        }
    }
}
