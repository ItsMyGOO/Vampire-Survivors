using System;
using System.Collections.Generic;
using ECS;
using UnityEngine;
using XLua;

/// <summary>
/// 渲染系统 - 带错误容错的版本
/// 改进点:
/// 1. 精灵加载失败时继续渲染其他实体
/// 2. 单个实体渲染出错不影响整体
/// 3. 详细的错误日志
/// 4. 性能统计
/// </summary>
public class RenderSystem
{
    private const int FORWARD_OFFSET = -90;

    private readonly SpriteProvider spriteProvider;
    private readonly RenderObjectPool pool;

    private readonly Dictionary<int, Transform> transforms = new Dictionary<int, Transform>();
    private readonly Dictionary<int, SpriteRenderer> renderers = new Dictionary<int, SpriteRenderer>();

    // 用于 entity diff
    private readonly HashSet<int> aliveThisFrame = new HashSet<int>();

    // ⭐ 错误追踪
    private readonly Dictionary<int, int> entityErrorCount = new Dictionary<int, int>();
    private const int MAX_ERROR_COUNT_PER_ENTITY = 5; // 单个实体最多记录5次错误

    // ⭐ 性能统计
    private int totalRenderedThisFrame = 0;
    private int totalErrorsThisFrame = 0;

    public RenderSystem(SpriteProvider spriteProvider, RenderObjectPool pool)
    {
        this.spriteProvider = spriteProvider;
        this.pool = pool;
    }

    /// <summary>
    /// 渲染 LuaTable 版本（兼容旧代码）
    /// </summary>
    public void Render(LuaTable items)
    {
        if (items == null)
            return;

        ResetFrameStats();
        aliveThisFrame.Clear();

        for (int i = 1; i <= items.Length; i++)
        {
            try
            {
                var item = items.Get<int, LuaRenderItem>(i);
                RenderSingleItem(item);
            }
            catch (Exception e)
            {
                HandleRenderError(-1, $"LuaTable索引 {i}", e);
            }
        }

        RecycleDeadEntities();
        LogFrameStats();
    }

    /// <summary>
    /// 渲染 List 版本
    /// </summary>
    public void Render(List<LuaRenderItem> items)
    {
        if (items == null)
            return;

        ResetFrameStats();
        aliveThisFrame.Clear();

        for (int i = 0; i < items.Count; i++)
        {
            try
            {
                var item = items[i];
                RenderSingleItem(item);
            }
            catch (Exception e)
            {
                int eid = items[i]?.eid ?? -1;
                HandleRenderError(eid, $"List索引 {i}", e);
            }
        }

        RecycleDeadEntities();
        LogFrameStats();
    }

    /// <summary>
    /// ⭐ 渲染单个实体 - 核心逻辑
    /// </summary>
    private void RenderSingleItem(LuaRenderItem item)
    {
        if (item == null)
        {
            totalErrorsThisFrame++;
            return;
        }

        var eid = item.eid;
        aliveThisFrame.Add(eid);
        totalRenderedThisFrame++;

        try
        {
            // 获取或创建 Transform 和 Renderer
            if (!transforms.TryGetValue(eid, out var transform))
            {
                CreateRenderComponents(eid, out transform);
            }

            // 更新 Transform
            UpdateTransform(transform, item);

            // 更新 Renderer
            var renderer = renderers[eid];
            UpdateRenderer(renderer, item, eid);

            // 清除错误计数（成功渲染）
            if (entityErrorCount.ContainsKey(eid))
            {
                entityErrorCount.Remove(eid);
            }
        }
        catch (Exception e)
        {
            HandleRenderError(eid, "渲染实体", e);
        }
    }

    /// <summary>
    /// ⭐ 创建渲染组件
    /// </summary>
    private void CreateRenderComponents(int eid, out Transform transform)
    {
        var go = pool.Get(eid.ToString());
        transform = go.transform;

        transforms[eid] = transform;
        renderers[eid] = go.GetComponent<SpriteRenderer>();

        if (renderers[eid] == null)
        {
            Debug.LogError($"[RenderSystem] 实体 {eid} 的 GameObject 没有 SpriteRenderer 组件!");
            renderers[eid] = go.AddComponent<SpriteRenderer>();
        }
    }

    /// <summary>
    /// ⭐ 更新 Transform
    /// </summary>
    private void UpdateTransform(Transform transform, LuaRenderItem item)
    {
        // 位置
        transform.position = new Vector3(item.posX, item.posY, item.posZ);

        // 旋转
        if ((item.flags & RenderFlags.UseRotation) != 0)
        {
            float rotationDegrees = item.rotation * Mathf.Rad2Deg + FORWARD_OFFSET;
            transform.rotation = Quaternion.Euler(0, 0, rotationDegrees);
        }
    }

    /// <summary>
    /// ⭐ 更新 Renderer
    /// </summary>
    private void UpdateRenderer(SpriteRenderer renderer, LuaRenderItem item, int eid)
    {
        // 排序层级（基于 Y 坐标）
        renderer.sortingOrder = -(int)(item.posY * 100);

        // 翻转
        if ((item.flags & RenderFlags.UseFlipX) != 0)
        {
            renderer.flipX = item.dirX switch
            {
                > 0 => false,
                < 0 => true,
                _ => renderer.flipX
            };
        }

        // ⭐ 加载精灵 - 带错误容错
        LoadSpriteForRenderer(renderer, item.sheet, item.spriteKey, eid);
    }

    /// <summary>
    /// ⭐ 加载精灵 - 异步加载，失败不阻塞
    /// </summary>
    private void LoadSpriteForRenderer(SpriteRenderer renderer, string sheet, string spriteKey, int eid)
    {
        // 参数验证
        if (string.IsNullOrEmpty(sheet) || string.IsNullOrEmpty(spriteKey))
        {
            // 空参数不记录错误（可能是故意的空精灵）
            return;
        }

        try
        {
            spriteProvider.Get(sheet, spriteKey, sprite =>
            {
                // ⭐ 回调可能在实体销毁后执行，需要检查
                if (!renderers.ContainsKey(eid))
                {
                    // 实体已被销毁，忽略
                    return;
                }

                try
                {
                    // 只有当精灵真的不同时才更新（避免不必要的更新）
                    if (renderer != null && renderer.sprite != sprite)
                    {
                        renderer.sprite = sprite;
                    }
                }
                catch (Exception e)
                {
                    // 回调中的错误
                    if (ShouldLogError(eid))
                    {
                        Debug.LogWarning($"[RenderSystem] 实体 {eid} 设置精灵失败: {e.Message}");
                    }
                }
            });
        }
        catch (Exception e)
        {
            // SpriteProvider.Get 本身的错误
            if (ShouldLogError(eid))
            {
                Debug.LogWarning($"[RenderSystem] 实体 {eid} 请求精灵失败 (sheet: {sheet}, key: {spriteKey}): {e.Message}");
            }
        }
    }

    /// <summary>
    /// ⭐ 回收已销毁的实体
    /// </summary>
    private void RecycleDeadEntities()
    {
        if (transforms.Count == 0)
            return;

        List<int> deadEids = null;

        foreach (var eid in transforms.Keys)
        {
            if (!aliveThisFrame.Contains(eid))
            {
                deadEids ??= new List<int>();
                deadEids.Add(eid);
            }
        }

        if (deadEids == null)
            return;

        foreach (var eid in deadEids)
        {
            try
            {
                RecycleSingleEntity(eid);
            }
            catch (Exception e)
            {
                Debug.LogError($"[RenderSystem] 回收实体 {eid} 时出错: {e.Message}");
            }
        }
    }

    /// <summary>
    /// ⭐ 回收单个实体
    /// </summary>
    private void RecycleSingleEntity(int eid)
    {
        var transform = transforms[eid];
        var go = transform.gameObject;

        // 清理状态
        var renderer = renderers[eid];
        if (renderer != null)
        {
            renderer.sprite = null;
            renderer.flipX = false;
            renderer.sortingOrder = 0;
        }

        transform.rotation = Quaternion.identity;

        // 归还对象池
        pool.Release(go);

        // 清理字典
        transforms.Remove(eid);
        renderers.Remove(eid);
        entityErrorCount.Remove(eid);
    }

    // ============================================
    // ⭐ 错误处理和日志
    // ============================================

    /// <summary>
    /// 处理渲染错误
    /// </summary>
    private void HandleRenderError(int eid, string context, Exception e)
    {
        totalErrorsThisFrame++;

        if (ShouldLogError(eid))
        {
            Debug.LogError($"[RenderSystem] {context} (实体 {eid}) 出错: {e.Message}\n{e.StackTrace}");
        }

        // 增加错误计数
        if (eid >= 0)
        {
            if (!entityErrorCount.ContainsKey(eid))
            {
                entityErrorCount[eid] = 0;
            }
            entityErrorCount[eid]++;
        }
    }

    /// <summary>
    /// 判断是否应该记录错误日志（避免日志刷屏）
    /// </summary>
    private bool ShouldLogError(int eid)
    {
        if (eid < 0)
            return true; // 未知实体的错误总是记录

        if (!entityErrorCount.TryGetValue(eid, out int count))
            return true; // 第一次错误总是记录

        // 前5次错误记录，之后不记录
        return count < MAX_ERROR_COUNT_PER_ENTITY;
    }

    /// <summary>
    /// 重置帧统计
    /// </summary>
    private void ResetFrameStats()
    {
        totalRenderedThisFrame = 0;
        totalErrorsThisFrame = 0;
    }

    /// <summary>
    /// 记录帧统计（仅在有错误时）
    /// </summary>
    private void LogFrameStats()
    {
        if (totalErrorsThisFrame > 0)
        {
            Debug.LogWarning($"[RenderSystem] 本帧统计: 渲染 {totalRenderedThisFrame} 个实体, {totalErrorsThisFrame} 个错误");
        }
    }

    // ============================================
    // ⭐ 公共调试接口
    // ============================================

    /// <summary>
    /// 打印渲染统计信息
    /// </summary>
    public void PrintStats()
    {
        Debug.Log($"[RenderSystem] 统计信息:\n" +
                  $"  活跃实体: {transforms.Count}\n" +
                  $"  有错误的实体: {entityErrorCount.Count}\n" +
                  $"  本帧渲染: {totalRenderedThisFrame}\n" +
                  $"  本帧错误: {totalErrorsThisFrame}");

        spriteProvider.PrintCacheStats();
    }

    /// <summary>
    /// 清理所有渲染缓存
    /// </summary>
    public void Clear()
    {
        foreach (var eid in new List<int>(transforms.Keys))
        {
            try
            {
                RecycleSingleEntity(eid);
            }
            catch (Exception e)
            {
                Debug.LogError($"[RenderSystem] 清理实体 {eid} 时出错: {e.Message}");
            }
        }

        transforms.Clear();
        renderers.Clear();
        entityErrorCount.Clear();
        aliveThisFrame.Clear();

        Debug.Log("[RenderSystem] 已清理所有渲染缓存");
    }
}