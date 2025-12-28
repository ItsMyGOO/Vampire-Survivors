using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class SpriteProvider
{
    static Dictionary<string, Sprite> cache = new();
    static HashSet<string> loading = new();

    public static void Get(string key, Action<Sprite> cb)
    {
        if (cache.TryGetValue(key, out var sprite))
        {
            cb(sprite);
            return;
        }

        if (!loading.Add(key))
            return;

        Addressables.LoadAssetAsync<Sprite>(key).Completed += h =>
        {
            loading.Remove(key);
            if (h.Status != AsyncOperationStatus.Succeeded)
                return;
            cache[key] = h.Result;
            cb(h.Result);
        };
    }
}