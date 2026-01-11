using System.Collections.Generic;
using ECS;
using Lua;
using UnityEngine;
using XLua;

public class RenderSystem
{
    private const int FORWARD_OFFSET = -90;

    private readonly SpriteProvider spriteProvider;
    private readonly RenderObjectPool pool;

    private readonly Dictionary<int, Transform> transforms = new();
    private readonly Dictionary<int, SpriteRenderer> renderers = new();

    // 用于 entity diff
    private readonly HashSet<int> aliveThisFrame = new();

    public RenderSystem(SpriteProvider spriteProvider, RenderObjectPool pool)
    {
        this.spriteProvider = spriteProvider;
        this.pool = pool;
    }

    public void Render(LuaTable items)
    {
        if (items == null)
            return;
        aliveThisFrame.Clear();
        for (int i = 1; i <= items.Length; i++)
        {
            var item = items.Get<int, LuaRenderItem>(i);
            var eid = item.eid;
            aliveThisFrame.Add(eid);

            if (!transforms.TryGetValue(eid, out var transform))
            {
                var go = pool.Get(eid.ToString());
                transform = go.transform;

                transforms[eid] = transform;
                renderers[eid] = go.GetComponent<SpriteRenderer>();
            }

            // Transform
            transform.position = new Vector3(item.posX, item.posY, item.posZ);

            // Sprite
            var renderer = renderers[eid];
            renderer.sortingOrder = -(int)(item.posY * 100);

            if ((item.flags & RenderFlags.UseFlipX) != 0)
            {
                renderer.flipX = item.dirX switch
                {
                    > 0 => false,
                    < 0 => true,
                    _ => renderer.flipX
                };
            }

            if ((item.flags & RenderFlags.UseRotation) != 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, item.rotation * Mathf.Rad2Deg + FORWARD_OFFSET);
            }

            spriteProvider.Get(item.sheet, item.spriteKey, sprite =>
            {
                if (renderer.sprite != sprite)
                    renderer.sprite = sprite;
            });
        }

        RecycleDeadEntities();
    }

    public void Render(List<LuaRenderItem> items)
    {
        if (items == null)
            return;
        aliveThisFrame.Clear();
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var eid = item.eid;
            aliveThisFrame.Add(eid);

            if (!transforms.TryGetValue(eid, out var transform))
            {
                var go = pool.Get(eid.ToString());
                transform = go.transform;

                transforms[eid] = transform;
                renderers[eid] = go.GetComponent<SpriteRenderer>();
            }

            // Transform
            transform.position = new Vector3(item.posX, item.posY, item.posZ);

            // Sprite
            var renderer = renderers[eid];
            renderer.sortingOrder = -(int)(item.posY * 100);

            if ((item.flags & RenderFlags.UseFlipX) != 0)
            {
                renderer.flipX = item.dirX switch
                {
                    > 0 => false,
                    < 0 => true,
                    _ => renderer.flipX
                };
            }

            if ((item.flags & RenderFlags.UseRotation) != 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, item.rotation * Mathf.Rad2Deg + FORWARD_OFFSET);
            }

            spriteProvider.Get(item.sheet, item.spriteKey, sprite =>
            {
                if (renderer.sprite != sprite)
                    renderer.sprite = sprite;
            });
        }

        RecycleDeadEntities();
    }

    private void RecycleDeadEntities()
    {
        if (transforms.Count == 0)
            return;

        // 为了避免遍历时修改集合
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
            var transform = transforms[eid];
            var go = transform.gameObject;

            // 清理状态（重要）
            var renderer = renderers[eid];
            renderer.sprite = null;
            renderer.flipX = false;
            renderer.sortingOrder = 0;

            transform.rotation = Quaternion.identity;

            pool.Release(go);

            transforms.Remove(eid);
            renderers.Remove(eid);
        }
    }
}