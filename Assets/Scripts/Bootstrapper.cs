using Lua;
using UnityEngine;

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

        // 3. 进入主菜单（GameSceneManager 单例在 MainMenuScene 里，通过直接加载场景启动）
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }
}