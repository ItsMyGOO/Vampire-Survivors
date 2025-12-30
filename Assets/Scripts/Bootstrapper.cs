using Lua;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Bootstrapper : MonoBehaviour
{
    public LuaMain LuaPrefab;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(LuaPrefab);
        Addressables.LoadSceneAsync("Assets/Scene/BattleScene.unity");
    }
}