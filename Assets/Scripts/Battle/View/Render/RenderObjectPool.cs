using System.Collections.Generic;
using UnityEngine;

public class RenderObjectPool
{
    private readonly Stack<GameObject> pool = new();

    public GameObject Get(string name)
    {
        if (pool.Count > 0)
        {
            var go = pool.Pop();
            go.name = name;
            go.SetActive(true);
            return go;
        }

        var newGo = new GameObject(name);
        newGo.AddComponent<SpriteRenderer>();
        return newGo;
    }

    public void Release(GameObject go)
    {
        go.SetActive(false);
        pool.Push(go);
    }

    /// <summary>
    /// 销毁池中所有缓存的 GameObject，彻底清理 View 对象。
    /// 切换战斗模式时由 IBattleMode.Exit() 调用。
    /// </summary>
    public void DestroyAll()
    {
        while (pool.Count > 0)
        {
            var go = pool.Pop();
            if (go != null)
                Object.Destroy(go);
        }
    }

}