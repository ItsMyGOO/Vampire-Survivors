using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SpriteProvider
{
    readonly Dictionary<string, Dictionary<string, Sprite>> _sheetCache = new();
    readonly Dictionary<string, List<Action>> _loading = new();

    /// <summary>
    /// sheet: Addressables key（如 farmer）
    /// spriteName: run_3
    /// </summary>
    public void Get(string sheet, string spriteName, Action<Sprite> cb)
    {
        // 已缓存
        if (_sheetCache.TryGetValue(sheet, out var sprites))
        {
            if (sprites.TryGetValue(spriteName, out var s))
                cb(s);
            return;
        }

        // 正在加载
        if (_loading.TryGetValue(sheet, out var waiters))
        {
            waiters.Add(() => Get(sheet, spriteName, cb));
            return;
        }

        // 开始加载 sheet
        _loading[sheet] = new List<Action>
        {
            () => Get(sheet, spriteName, cb)
        };

        Addressables.LoadAssetAsync<Sprite[]>(sheet).Completed += h =>
        {
            var callbacks = _loading[sheet];
            _loading.Remove(sheet);

            if (h.Status != AsyncOperationStatus.Succeeded)
                return;

            var dict = new Dictionary<string, Sprite>();
            foreach (var s in h.Result)
                dict[s.name] = s;

            _sheetCache[sheet] = dict;

            // 回放等待的请求
            foreach (var action in callbacks)
                action();
        };
    }
}