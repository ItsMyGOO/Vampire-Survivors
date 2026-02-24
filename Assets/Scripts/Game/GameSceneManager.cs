using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace Game
{
    /// <summary>
    /// 场景管理器 - 单例，DontDestroyOnLoad
    /// 负责所有场景切换和游戏状态跟踪，不持有任何战斗/ECS 状态
    /// 场景加载委托给 SceneLoader（异步）
    /// </summary>
    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager Instance { get; private set; }

        // ── 场景名称常量 ───────────────────────────────────────
        public const string MAIN_MENU_SCENE        = "MainMenuScene";
        public const string CHARACTER_SELECT_SCENE = "CharacterSelectScene";
        public const string BATTLE_SCENE           = "BattleScene";

        // ── 游戏状态 ───────────────────────────────────────────
        public enum GameState { MainMenu, CharacterSelect, Loading, InBattle, Paused }

        private GameState _currentState = GameState.MainMenu;

        // ── 事件 ───────────────────────────────────────────────
        public event Action<GameState> OnStateChanged;
        public event Action            OnBattleStarted;
        public event Action            OnBattleEnded;

        // ── Unity 生命周期 ─────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }

        // ── 通用场景加载（异步）───────────────────────────────
private void LoadScene(string sceneName, GameState stateAfterLoad)
        {
            if (_currentState == GameState.Loading)
            {
                Debug.LogWarning($"[GameSceneManager] 已在加载中，忽略: {sceneName}");
                return;
            }

            ChangeState(GameState.Loading);

            // 确保 SceneTransition 存在（若场景中未预置则自动创建）
            var transition = SceneTransition.GetOrCreate();

            transition.FadeOut(onComplete: () =>
            {
                SceneLoader.LoadAsync(sceneName, onComplete: () =>
                {
                    Debug.Log($"[GameSceneManager] 场景加载完成: {sceneName}");
                    ChangeState(stateAfterLoad);

                    if (stateAfterLoad == GameState.InBattle)
                        OnBattleStarted?.Invoke();

                    transition.FadeIn();
                });
            });
        }

        // ── 公开场景切换接口 ───────────────────────────────────

        /// <summary>主菜单</summary>
        public void LoadMainMenu()
        {
            Session.GameSessionData.Reset();
            LoadScene(MAIN_MENU_SCENE, GameState.MainMenu);
        }

        /// <summary>
        /// 已废弃：角色选择现在通过 UIManager 在主菜单场景内切换 Panel，无需跳转场景
        /// 保留此方法仅为兼容旧调用点
        /// </summary>
        [System.Obsolete("CharacterSelect 现在是 Panel 切换，请改为 UIManager.Instance.ShowPanel<CharacterSelectPanel>()")]
        public void LoadCharacterSelect()
        {
            Debug.LogWarning("[GameSceneManager] LoadCharacterSelect 已废弃，请改为 UIManager Panel 切换");
            if (UI.Core.UIManager.Instance != null)
                UI.Core.UIManager.Instance.ShowPanel<UI.Panel.CharacterSelectPanel>(hideOthers: true, addToStack: true);
        }

        /// <summary>开始战斗（需先调用 GameSessionData.SelectCharacter）</summary>
        public void StartBattle()
        {
            if (!Session.GameSessionData.HasSelection)
            {
                Debug.LogWarning("[GameSceneManager] 未选择角色，无法开始战斗");
                return;
            }
            LoadScene(BATTLE_SCENE, GameState.InBattle);
        }

        /// <summary>重新开始战斗</summary>
        public void RestartBattle()
        {
            LoadScene(BATTLE_SCENE, GameState.InBattle);
        }

        /// <summary>退出战斗，返回主菜单</summary>
        public void ExitBattle()
        {
            if (_currentState != GameState.InBattle && _currentState != GameState.Paused)
            {
                Debug.LogWarning("[GameSceneManager] 当前不在战斗中");
                return;
            }
            OnBattleEnded?.Invoke();
            LoadMainMenu();
        }

        // ── 暂停 / 恢复 ────────────────────────────────────────

        public void PauseGame()
        {
            if (_currentState != GameState.InBattle) return;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            if (_currentState != GameState.Paused) return;
            ChangeState(GameState.InBattle);
            Time.timeScale = 1f;
        }

        // ── 退出游戏 ───────────────────────────────────────────

        public void QuitGame()
        {
            Debug.Log("[GameSceneManager] 退出游戏");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // ── 属性 ───────────────────────────────────────────────

        public GameState CurrentState => _currentState;
        public bool IsInBattle        => _currentState == GameState.InBattle;
        public bool IsPaused          => _currentState == GameState.Paused;

        // ── 私有工具 ───────────────────────────────────────────

        private void ChangeState(GameState next)
        {
            if (_currentState == next) return;
            var prev = _currentState;
            _currentState = next;
            Debug.Log($"[GameSceneManager] {prev} -> {next}");
            OnStateChanged?.Invoke(next);
        }
    }
}