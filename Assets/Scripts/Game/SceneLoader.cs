using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    /// <summary>
    /// 场景加载工具 - 纯静态，不持有任何状态
    /// 提供异步场景加载/卸载能力，通过 MonoBehaviour Carrier 驱动协程
    /// </summary>
    public static class SceneLoader
    {
        // ── 公开接口 ────────────────────────────────────────────

        /// <summary>
        /// 异步加载场景（Single 模式，替换当前场景）
        /// </summary>
        /// <param name="sceneName">目标场景名</param>
        /// <param name="onComplete">加载完成回调（在激活帧调用）</param>
        /// <param name="allowSceneActivation">是否自动激活（false 可在 90% 时暂停）</param>
        public static void LoadAsync(
            string   sceneName,
            Action   onComplete             = null,
            bool     allowSceneActivation   = true)
        {
            CoroutineCarrier.Run(
                LoadAsyncRoutine(sceneName, LoadSceneMode.Single, onComplete, allowSceneActivation));
        }

        /// <summary>
        /// 异步叠加加载场景（Additive 模式）
        /// </summary>
        public static void LoadAdditiveAsync(
            string sceneName,
            Action onComplete = null)
        {
            CoroutineCarrier.Run(
                LoadAsyncRoutine(sceneName, LoadSceneMode.Additive, onComplete, true));
        }

        /// <summary>
        /// 异步卸载场景
        /// </summary>
        public static void UnloadAsync(
            string sceneName,
            Action onComplete = null)
        {
            CoroutineCarrier.Run(UnloadAsyncRoutine(sceneName, onComplete));
        }

        // ── 内部协程 ────────────────────────────────────────────

        private static IEnumerator LoadAsyncRoutine(
            string        sceneName,
            LoadSceneMode mode,
            Action        onComplete,
            bool          allowSceneActivation)
        {
            var op = SceneManager.LoadSceneAsync(sceneName, mode);
            if (op == null)
            {
                Debug.LogError($"[SceneLoader] 无法找到场景: {sceneName}，请检查 Build Settings");
                onComplete?.Invoke();
                yield break;
            }

            op.allowSceneActivation = allowSceneActivation;

            while (!op.isDone)
            {
                // progress: 0.0 ~ 0.9 加载中，0.9 ~ 1.0 激活中
                yield return null;
            }

            Debug.Log($"[SceneLoader] 场景加载完成: {sceneName}");
            onComplete?.Invoke();
        }

        private static IEnumerator UnloadAsyncRoutine(string sceneName, Action onComplete)
        {
            var op = SceneManager.UnloadSceneAsync(sceneName);
            if (op == null)
            {
                Debug.LogWarning($"[SceneLoader] 无法卸载场景: {sceneName}（可能未加载）");
                onComplete?.Invoke();
                yield break;
            }

            while (!op.isDone)
                yield return null;

            Debug.Log($"[SceneLoader] 场景卸载完成: {sceneName}");
            onComplete?.Invoke();
        }

        // ── 协程承载体 ──────────────────────────────────────────

        /// <summary>
        /// 隐式单例 MonoBehaviour，仅用于驱动协程
        /// 自动创建，DontDestroyOnLoad，不暴露到业务层
        /// </summary>
        private static class CoroutineCarrier
        {
            private static CarrierBehaviour _instance;

            public static void Run(IEnumerator routine)
            {
                EnsureInstance();
                _instance.StartCoroutine(routine);
            }

            private static void EnsureInstance()
            {
                if (_instance != null) return;

                var go = new GameObject("[SceneLoader.Carrier]");
                UnityEngine.Object.DontDestroyOnLoad(go);
                _instance = go.AddComponent<CarrierBehaviour>();
            }
        }

        private class CarrierBehaviour : MonoBehaviour { }
    }
}
