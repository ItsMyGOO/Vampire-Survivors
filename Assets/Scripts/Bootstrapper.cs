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
        Battle.GameConfigLoader.LoadAll();

        // 3. 异步叠加加载 MainMenuScene，加载完成后将其设为激活场景并卸载 Bootstrapper
        Game.SceneLoader.LoadAdditiveAsync(
            Game.GameSceneManager.MAIN_MENU_SCENE,
            onComplete: () =>
            {
                SceneManager.SetActiveScene(
                    SceneManager.GetSceneByName(Game.GameSceneManager.MAIN_MENU_SCENE));

                Game.SceneLoader.UnloadAsync("Bootstrapper");
            });
    }
}