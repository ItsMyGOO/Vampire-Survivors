using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// 精灵提供者 - 带错误容错的版本
/// 改进点:
/// 1. 加载失败时不会阻塞其他资源加载
/// 2. 记录加载失败的资源，避免重复尝试
/// 3. 提供默认占位符精灵
/// 4. 详细的错误日志
/// </summary>
public class SpriteProvider
{
    // 已成功加载的图集缓存
    private readonly Dictionary<string, Dictionary<string, Sprite>> _sheetCache = new Dictionary<string, Dictionary<string, Sprite>>();
    
    // 正在加载中的图集及等待回调
    private readonly Dictionary<string, List<Action>> _loading = new Dictionary<string, List<Action>>();
    
    // ⭐ 加载失败的图集记录（避免重复尝试）
    private readonly HashSet<string> _failedSheets = new HashSet<string>();
    
    // ⭐ 默认占位符精灵（可选）
    private Sprite _fallbackSprite;

    /// <summary>
    /// 设置默认占位符精灵（加载失败时使用）
    /// </summary>
    public void SetFallbackSprite(Sprite sprite)
    {
        _fallbackSprite = sprite;
    }

    /// <summary>
    /// 获取精灵 - 带错误容错
    /// </summary>
    /// <param name="sheet">图集地址（Addressables key）</param>
    /// <param name="spriteName">精灵名称</param>
    /// <param name="cb">回调函数</param>
    public void Get(string sheet, string spriteName, Action<Sprite> cb)
    {
        // ⭐ 参数验证
        if (string.IsNullOrEmpty(sheet) || string.IsNullOrEmpty(spriteName))
        {
            Debug.LogWarning($"[SpriteProvider] 无效的参数: sheet='{sheet}', spriteName='{spriteName}'");
            cb?.Invoke(_fallbackSprite);
            return;
        }

        // ⭐ 检查是否是已知的失败图集
        if (_failedSheets.Contains(sheet))
        {
            // 不再尝试加载，直接返回占位符
            cb?.Invoke(_fallbackSprite);
            return;
        }

        // 已缓存 - 成功路径
        if (_sheetCache.TryGetValue(sheet, out var sprites))
        {
            if (sprites.TryGetValue(spriteName, out var sprite))
            {
                cb?.Invoke(sprite);
            }
            else
            {
                // 图集已加载，但找不到指定精灵
                Debug.LogWarning($"[SpriteProvider] 图集 '{sheet}' 中找不到精灵 '{spriteName}'");
                cb?.Invoke(_fallbackSprite);
            }
            return;
        }

        // 正在加载中 - 加入等待队列
        if (_loading.TryGetValue(sheet, out var waiters))
        {
            waiters.Add(() => Get(sheet, spriteName, cb));
            return;
        }

        // 开始加载新图集
        _loading[sheet] = new List<Action>
        {
            () => Get(sheet, spriteName, cb)
        };

        Debug.Log($"[SpriteProvider] 开始加载图集: {sheet}");

        // ⭐ 异步加载，带完整错误处理
        Addressables.LoadAssetAsync<Sprite[]>(sheet).Completed += handle =>
        {
            OnSheetLoadCompleted(sheet, handle);
        };
    }

    /// <summary>
    /// ⭐ 图集加载完成回调 - 集中处理成功和失败情况
    /// </summary>
    private void OnSheetLoadCompleted(string sheet, AsyncOperationHandle<Sprite[]> handle)
    {
        // 获取等待的回调列表
        if (!_loading.TryGetValue(sheet, out var callbacks))
        {
            Debug.LogWarning($"[SpriteProvider] 图集 '{sheet}' 加载完成，但找不到等待回调");
            return;
        }

        _loading.Remove(sheet);

        // ⭐ 检查加载状态
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // 成功加载
            HandleLoadSuccess(sheet, handle.Result, callbacks);
        }
        else
        {
            // 加载失败
            HandleLoadFailure(sheet, handle, callbacks);
        }
    }

    /// <summary>
    /// ⭐ 处理加载成功
    /// </summary>
    private void HandleLoadSuccess(string sheet, Sprite[] sprites, List<Action> callbacks)
    {
        Debug.Log($"[SpriteProvider] 图集 '{sheet}' 加载成功，包含 {sprites.Length} 个精灵");

        // 构建精灵字典
        var dict = new Dictionary<string, Sprite>();
        foreach (var sprite in sprites)
        {
            if (sprite != null)
            {
                dict[sprite.name] = sprite;
            }
        }

        // 缓存图集
        _sheetCache[sheet] = dict;

        // ⭐ 执行所有等待的回调
        foreach (var callback in callbacks)
        {
            try
            {
                callback?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"[SpriteProvider] 回调执行出错: {e.Message}\n{e.StackTrace}");
            }
        }
    }

    /// <summary>
    /// ⭐ 处理加载失败
    /// </summary>
    private void HandleLoadFailure(string sheet, AsyncOperationHandle<Sprite[]> handle, List<Action> callbacks)
    {
        // 记录失败的图集（避免重复尝试）
        _failedSheets.Add(sheet);

        // 详细的错误日志
        string errorMsg = handle.OperationException != null 
            ? handle.OperationException.Message 
            : "Unknown error";

        Debug.LogError($"[SpriteProvider] 图集 '{sheet}' 加载失败: {errorMsg}\n" +
                      $"状态: {handle.Status}\n" +
                      $"等待回调数: {callbacks.Count}");

        // ⭐ 即使加载失败，也要通知所有等待的回调（使用占位符）
        foreach (var callback in callbacks)
        {
            try
            {
                // 尝试解析回调，提取实际的回调函数并传入 null 或占位符
                callback?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"[SpriteProvider] 失败回调执行出错: {e.Message}");
            }
        }

        Debug.LogWarning($"[SpriteProvider] 已标记图集 '{sheet}' 为失败，后续请求将使用占位符");
    }

    /// <summary>
    /// ⭐ 清除失败记录（用于重试机制）
    /// </summary>
    public void ClearFailedSheets()
    {
        _failedSheets.Clear();
        Debug.Log("[SpriteProvider] 已清除失败图集记录");
    }

    /// <summary>
    /// ⭐ 检查图集是否加载失败
    /// </summary>
    public bool IsSheetFailed(string sheet)
    {
        return _failedSheets.Contains(sheet);
    }

    /// <summary>
    /// ⭐ 获取缓存统计信息
    /// </summary>
    public void PrintCacheStats()
    {
        Debug.Log($"[SpriteProvider] 缓存统计:\n" +
                  $"  已加载图集: {_sheetCache.Count}\n" +
                  $"  正在加载: {_loading.Count}\n" +
                  $"  加载失败: {_failedSheets.Count}");
    }

    /// <summary>
    /// ⭐ 预加载图集（可选，用于提前加载常用资源）
    /// </summary>
    public void PreloadSheet(string sheet, Action<bool> onComplete = null)
    {
        if (_sheetCache.ContainsKey(sheet))
        {
            Debug.Log($"[SpriteProvider] 图集 '{sheet}' 已在缓存中");
            onComplete?.Invoke(true);
            return;
        }

        if (_failedSheets.Contains(sheet))
        {
            Debug.LogWarning($"[SpriteProvider] 图集 '{sheet}' 之前加载失败");
            onComplete?.Invoke(false);
            return;
        }

        Debug.Log($"[SpriteProvider] 预加载图集: {sheet}");

        Addressables.LoadAssetAsync<Sprite[]>(sheet).Completed += handle =>
        {
            bool success = handle.Status == AsyncOperationStatus.Succeeded;
            
            if (success)
            {
                var dict = new Dictionary<string, Sprite>();
                foreach (var sprite in handle.Result)
                {
                    if (sprite != null)
                    {
                        dict[sprite.name] = sprite;
                    }
                }
                _sheetCache[sheet] = dict;
                Debug.Log($"[SpriteProvider] 预加载成功: {sheet}");
            }
            else
            {
                _failedSheets.Add(sheet);
                Debug.LogError($"[SpriteProvider] 预加载失败: {sheet}");
            }

            onComplete?.Invoke(success);
        };
    }

    /// <summary>
    /// 清理所有缓存（用于场景切换等）
    /// </summary>
    public void Clear()
    {
        _sheetCache.Clear();
        _loading.Clear();
        _failedSheets.Clear();
        Debug.Log("[SpriteProvider] 已清理所有缓存");
    }
}