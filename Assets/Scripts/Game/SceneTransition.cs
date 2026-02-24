using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// 场景过渡淡入淡出系统
    /// 挂载在 DontDestroyOnLoad 的 Canvas（覆盖全屏的黑色 Image）上
    /// 由 SceneLoader 驱动，业务层无需直接调用
    /// </summary>
    public class SceneTransition : MonoBehaviour
    {
        public static SceneTransition Instance { get; private set; }

        [SerializeField] private Image fadeImage;
        [SerializeField] private float fadeDuration = 0.3f;

        private Coroutine _currentFade;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 初始完全透明，不阻挡交互
            SetAlpha(0f);
            SetRaycast(false);
        }

        // ── 公开接口 ────────────────────────────────────────────

        /// <summary>
        /// 先淡出（黑），执行 action，再淡入（透明）
        /// </summary>
        public void CrossFade(Action action)
        {
            if (_currentFade != null)
                StopCoroutine(_currentFade);
            _currentFade = StartCoroutine(CrossFadeRoutine(action));
        }

        /// <summary>
        /// 仅淡出到黑
        /// </summary>
        public void FadeOut(Action onComplete = null)
        {
            if (_currentFade != null)
                StopCoroutine(_currentFade);
            _currentFade = StartCoroutine(FadeRoutine(0f, 1f, onComplete));
        }

        /// <summary>
        /// 仅从黑淡入透明
        /// </summary>
        public void FadeIn(Action onComplete = null)
        {
            if (_currentFade != null)
                StopCoroutine(_currentFade);
            _currentFade = StartCoroutine(FadeRoutine(1f, 0f, onComplete));
        }

        // ── 协程 ────────────────────────────────────────────────

        private IEnumerator CrossFadeRoutine(Action action)
        {
            // 淡出到黑
            yield return FadeRoutine(0f, 1f, null);

            // 执行场景加载等操作
            action?.Invoke();

            // 等一帧让新场景开始初始化
            yield return null;
            yield return null;

            // 从黑淡入
            yield return FadeRoutine(1f, 0f, null);
        }

        private IEnumerator FadeRoutine(float from, float to, Action onComplete)
        {
            SetRaycast(true);
            float elapsed = 0f;
            SetAlpha(from);

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                SetAlpha(Mathf.Lerp(from, to, elapsed / fadeDuration));
                yield return null;
            }

            SetAlpha(to);

            // 淡入完成后取消射线阻挡
            if (Mathf.Approximately(to, 0f))
                SetRaycast(false);

            onComplete?.Invoke();
        }

        // ── 工具 ────────────────────────────────────────────────

        private void SetAlpha(float alpha)
        {
            if (fadeImage == null) return;
            var c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
        }

        private void SetRaycast(bool block)
        {
            if (fadeImage == null) return;
            fadeImage.raycastTarget = block;
        }

        // ── 静态工厂：自动创建（若场景中不存在） ──────────────

        public static SceneTransition GetOrCreate()
        {
            if (Instance != null) return Instance;

            var go = new GameObject("[SceneTransition]");
            DontDestroyOnLoad(go);

            // Canvas
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;

            go.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            // 全屏黑色 Image
            var imgGo = new GameObject("FadeImage");
            imgGo.transform.SetParent(go.transform, false);
            var img = imgGo.AddComponent<Image>();
            img.color = Color.black;

            var rect = imgGo.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var transition = go.AddComponent<SceneTransition>();
            transition.fadeImage = img;
            transition.fadeDuration = 0.3f;

            return transition;
        }
    }
}
