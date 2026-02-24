using Battle;
using Game;
using Lua;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    public LuaMain LuaPrefab;

    private void Start()
    {
        // 1. Lua 运行时（可选，XLua 层）
        if (LuaPrefab != null)
            Instantiate(LuaPrefab);

        // 2. 加载所有配置数据库
        GameConfigLoader.LoadAll();

        // 3. 异步叠加加载 BattleScene，加载完成后将其设为激活场景并卸载 Bootstrapper
        SceneLoader.LoadAdditiveAsync(
            GameSceneManager.BATTLE_SCENE,
            onComplete: () =>
            {
                SceneManager.SetActiveScene(
                    SceneManager.GetSceneByName(GameSceneManager.BATTLE_SCENE));

                SceneLoader.UnloadAsync("Bootstrapper");
            });
    }
}