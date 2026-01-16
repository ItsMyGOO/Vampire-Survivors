using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Battle;
using Battle.Player;

namespace Game
{
    /// <summary>
    /// 游戏场景管理器 - 单例
    /// 负责场景切换和游戏状态管理
    /// </summary>
    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager Instance { get; private set; }

        // 场景名称常量
        public const string MAIN_MENU_SCENE = "MainScene";
        public const string BATTLE_SCENE = "BattleScene";

        // 当前游戏状态
        public enum GameState
        {
            MainMenu,
            InBattle,
            Paused,
            Loading
        }

        private GameState currentState = GameState.MainMenu;

        // 事件
        public event Action OnBattleStarted;
        public event Action OnBattleEnded;
        public event Action<GameState> OnStateChanged;

        private void Awake()
        {
            // 单例模式
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 开始战斗
        /// </summary>
        public void StartBattle()
        {
            if (currentState == GameState.Loading || currentState == GameState.InBattle)
            {
                Debug.LogWarning("[GameSceneManager] 已经在战斗中或正在加载");
                return;
            }

            Debug.Log("[GameSceneManager] 开始战斗");
            ChangeState(GameState.Loading);

            // 加载战斗场景
            SceneManager.LoadScene(BATTLE_SCENE, LoadSceneMode.Single);
            
            // 场景加载完成后的回调
            SceneManager.sceneLoaded += OnBattleSceneLoaded;
        }

        /// <summary>
        /// 战斗场景加载完成
        /// </summary>
        private void OnBattleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != BATTLE_SCENE)
                return;

            SceneManager.sceneLoaded -= OnBattleSceneLoaded;

            Debug.Log("[GameSceneManager] 战斗场景加载完成");
            ChangeState(GameState.InBattle);
            
            OnBattleStarted?.Invoke();
        }

        /// <summary>
        /// 退出战斗
        /// </summary>
        public void ExitBattle()
        {
            if (currentState != GameState.InBattle && currentState != GameState.Paused)
            {
                Debug.LogWarning("[GameSceneManager] 当前不在战斗中");
                return;
            }

            Debug.Log("[GameSceneManager] 退出战斗");
            ChangeState(GameState.Loading);

            // 清理战斗数据
            CleanupBattleData();

            // 返回主菜单
            SceneManager.LoadScene(MAIN_MENU_SCENE, LoadSceneMode.Single);
            
            // 场景加载完成后的回调
            SceneManager.sceneLoaded += OnMainMenuSceneLoaded;
        }

        /// <summary>
        /// 主菜单场景加载完成
        /// </summary>
        private void OnMainMenuSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != MAIN_MENU_SCENE)
                return;

            SceneManager.sceneLoaded -= OnMainMenuSceneLoaded;

            Debug.Log("[GameSceneManager] 主菜单场景加载完成");
            ChangeState(GameState.MainMenu);
            
            OnBattleEnded?.Invoke();
        }

        /// <summary>
        /// 清理战斗数据
        /// </summary>
        private void CleanupBattleData()
        {
            Debug.Log("[GameSceneManager] 清理战斗数据");

            // // 1. 清理 ECSGameManager
            // if (ECSGameManager.Instance != null)
            // {
            //     // ECSGameManager 会在 OnDestroy 中自动清理 World
            //     Destroy(ECSGameManager.Instance.gameObject);
            // }
            //
            // // 2. 清理 PlayerContext
            // if (PlayerContext.Instance != null)
            // {
            //     PlayerContext.Instance.Clear();
            // }
            //
            // // 3. 清理 LuaMain (如果还在用)
            // var luaMain = FindObjectOfType<Lua.LuaMain>();
            // if (luaMain != null)
            // {
            //     Destroy(luaMain.gameObject);
            // }
            //
            // // 4. 强制垃圾回收
            // System.GC.Collect();
            // Resources.UnloadUnusedAssets();

            Debug.Log("[GameSceneManager] 战斗数据清理完成");
        }

        /// <summary>
        /// 暂停游戏
        /// </summary>
        public void PauseGame()
        {
            if (currentState != GameState.InBattle)
                return;

            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            Debug.Log("[GameSceneManager] 游戏暂停");
        }

        /// <summary>
        /// 恢复游戏
        /// </summary>
        public void ResumeGame()
        {
            if (currentState != GameState.Paused)
                return;

            ChangeState(GameState.InBattle);
            Time.timeScale = 1f;
            Debug.Log("[GameSceneManager] 游戏恢复");
        }

        /// <summary>
        /// 重新开始战斗
        /// </summary>
        public void RestartBattle()
        {
            if (currentState == GameState.InBattle || currentState == GameState.Paused)
            {
                // 先清理,再重新开始
                CleanupBattleData();
            }

            StartBattle();
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("[GameSceneManager] 退出游戏");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// 改变游戏状态
        /// </summary>
        private void ChangeState(GameState newState)
        {
            if (currentState == newState)
                return;

            GameState oldState = currentState;
            currentState = newState;

            Debug.Log($"[GameSceneManager] 状态切换: {oldState} → {newState}");
            OnStateChanged?.Invoke(newState);
        }

        // ========== 属性 ==========
        public GameState CurrentState => currentState;
        public bool IsInBattle => currentState == GameState.InBattle;
        public bool IsPaused => currentState == GameState.Paused;

        private void OnDestroy()
        {
            // 清理事件
            SceneManager.sceneLoaded -= OnBattleSceneLoaded;
            SceneManager.sceneLoaded -= OnMainMenuSceneLoaded;
            
            // 恢复时间缩放
            Time.timeScale = 1f;
        }
    }
}
