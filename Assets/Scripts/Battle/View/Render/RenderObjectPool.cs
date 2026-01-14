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
}