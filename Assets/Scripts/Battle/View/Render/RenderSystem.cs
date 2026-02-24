using System;
using System.Collections.Generic;
using ECS;
using UnityEngine;

/// <summary>
/// Unity 渲染系统（View 层）
/// 由 RenderSyncSystem 驱动
/// </summary>
public class RenderSystem
{
    private const int FORWARD_OFFSET = -90;

    private readonly SpriteProvider spriteProvider;
    private readonly RenderObjectPool pool;

    private readonly Dictionary<int, Transform> transforms = new();
    private readonly Dictionary<int, SpriteRenderer> renderers = new();

    private readonly HashSet<int> aliveThisFrame = new();
    private readonly Dictionary<int, int> entityErrorCount = new();

    private const int MAX_ERROR_COUNT_PER_ENTITY = 5;

    private int totalRenderedThisFrame;
    private int totalErrorsThisFrame;

    private int currentCameraTarget = -1;

    public Action<Transform> OnCameraTargetChanged;

    public RenderSystem(SpriteProvider spriteProvider, RenderObjectPool pool)
    {
        this.spriteProvider = spriteProvider;
        this.pool = pool;
    }

    // =========================================================
    // Frame 生命周期
    // =========================================================

    public void BeginFrame()
    {
        totalRenderedThisFrame = 0;
        totalErrorsThisFrame = 0;
        aliveThisFrame.Clear();
    }

    public void EndFrame()
    {
        RecycleDeadEntities();

        if (totalErrorsThisFrame > 0)
        {
            Debug.LogWarning(
                $"[RenderSystem] 本帧渲染 {totalRenderedThisFrame} 个实体，错误 {totalErrorsThisFrame}");
        }
    }

    // =========================================================
    // ECS → Render 同步入口
    // =========================================================

    public void RenderEntity(
        int eid,
        PositionComponent position,
        VelocityComponent velocity,  bool hasVelocity,
        RotationComponent rotation,  bool hasRotation,
        SpriteKeyComponent spriteKey,
        bool isCameraFollowTarget)
    {
        aliveThisFrame.Add(eid);
        totalRenderedThisFrame++;

        try
        {
            if (!transforms.TryGetValue(eid, out var transform))
                CreateRenderObject(eid, out transform);

            UpdateTransform(transform, position, rotation, hasRotation);
            UpdateRenderer(renderers[eid], position, velocity, hasVelocity, spriteKey, eid);

            if (isCameraFollowTarget)
                TryMarkCameraTarget(eid);

            entityErrorCount.Remove(eid);
        }
        catch (Exception e)
        {
            HandleRenderError(eid, "RenderEntity", e);
        }
    }

    private void TryMarkCameraTarget(int eid)
    {
        if (currentCameraTarget == eid) return;
        currentCameraTarget = eid;
        OnCameraTargetChanged?.Invoke(transforms[eid]);
    }

    // =========================================================
    // 创建 / 更新
    // =========================================================

    private void CreateRenderObject(int eid, out Transform transform)
    {
        var go = pool.Get(eid.ToString());
        transform = go.transform;
        transforms[eid] = transform;

        if (!go.TryGetComponent(out SpriteRenderer renderer))
            renderer = go.AddComponent<SpriteRenderer>();

        renderers[eid] = renderer;
    }

    private void UpdateTransform(
        Transform transform,
        PositionComponent pos,
        RotationComponent rotation,
        bool hasRotation)
    {
        transform.position = new Vector3(pos.x, pos.y, 0);

        if (hasRotation)
        {
            float z = rotation.angle * Mathf.Rad2Deg + FORWARD_OFFSET;
            transform.rotation = Quaternion.Euler(0, 0, z);
        }
    }

    private void UpdateRenderer(
        SpriteRenderer renderer,
        PositionComponent pos,
        VelocityComponent velocity,
        bool hasVelocity,
        SpriteKeyComponent spriteKey,
        int eid)
    {
        renderer.sortingOrder = -(int)(pos.y * 100);

        if (hasVelocity)
        {
            if (velocity.x > 0) renderer.flipX = false;
            else if (velocity.x < 0) renderer.flipX = true;
        }

        if (spriteKey != null)
            LoadSprite(renderer, spriteKey.sheet, spriteKey.key, eid);
    }

    // =========================================================
    // Sprite 加载
    // =========================================================

    private void LoadSprite(SpriteRenderer renderer, string sheet, string key, int eid)
    {
        if (string.IsNullOrEmpty(sheet) || string.IsNullOrEmpty(key))
            return;

        try
        {
            spriteProvider.Get(sheet, key, sprite =>
            {
                if (!renderers.ContainsKey(eid)) return;
                if (renderer != null && renderer.sprite != sprite)
                    renderer.sprite = sprite;
            });
        }
        catch (Exception e)
        {
            HandleRenderError(eid, "LoadSprite", e);
        }
    }

    // =========================================================
    // 回收
    // =========================================================

    private void RecycleDeadEntities()
    {
        List<int> dead = null;

        foreach (var eid in transforms.Keys)
        {
            if (!aliveThisFrame.Contains(eid))
            {
                dead ??= new();
                dead.Add(eid);
            }
        }

        if (dead == null) return;

        foreach (var eid in dead)
            RecycleEntity(eid);
    }

    private void RecycleEntity(int eid)
    {
        var transform = transforms[eid];
        var renderer = renderers[eid];

        renderer.sprite = null;
        renderer.flipX = false;
        renderer.sortingOrder = 0;
        transform.rotation = Quaternion.identity;

        pool.Release(transform.gameObject);

        transforms.Remove(eid);
        renderers.Remove(eid);
        entityErrorCount.Remove(eid);
    }

    // =========================================================
    // 错误处理
    // =========================================================

    private void HandleRenderError(int eid, string context, Exception e)
    {
        totalErrorsThisFrame++;

        if (ShouldLogError(eid))
        {
            Debug.LogError(
                $"[RenderSystem] {context} (eid={eid}) 错误: {e.Message}\n{e.StackTrace}");
        }

        entityErrorCount[eid] = entityErrorCount.TryGetValue(eid, out var c) ? c + 1 : 1;
    }

    
    // =========================================================
    // 完整清理（模式切换时调用）
    // =========================================================

    /// <summary>
    /// 销毁所有由本系统管理的 GameObject（包括 active 和 pool 中缓存的），
    /// 清空内部状态。切换战斗模式时由 IBattleMode.Exit() 调用。
    /// </summary>
    public void DestroyAll()
    {
        foreach (var transform in transforms.Values)
        {
            if (transform != null)
                UnityEngine.Object.Destroy(transform.gameObject);
        }
        transforms.Clear();
        renderers.Clear();
        entityErrorCount.Clear();
        aliveThisFrame.Clear();
        currentCameraTarget = -1;

        pool.DestroyAll();
    }
private bool ShouldLogError(int eid)
    {
        return !entityErrorCount.TryGetValue(eid, out var count)
               || count < MAX_ERROR_COUNT_PER_ENTITY;
    }
}