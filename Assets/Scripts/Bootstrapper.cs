using Lua;
using UI.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Bootstrapper : MonoBehaviour
{
    public LuaMain LuaPrefab;
    public UIManager UIManager;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(LuaPrefab);
        Instantiate(UIManager);
        Addressables.LoadSceneAsync("Assets/Scene/BattleScene.unity");
    }
}