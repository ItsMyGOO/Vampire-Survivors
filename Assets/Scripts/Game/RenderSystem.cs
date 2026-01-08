using System.Collections.Generic;
using Lua;
using UnityEngine;
using XLua;

public class RenderSystem
{
    private const int FORWARD_OFFSET = -90;

    private SpriteProvider spriteProvider;

    private Dictionary<(int sheet, int key), Sprite> spriteCache = new();
    private Dictionary<int, Transform> transforms = new();
    private Dictionary<int, SpriteRenderer> renderers = new();

    public RenderSystem(SpriteProvider spriteProvider)
    {
        this.spriteProvider = spriteProvider;
    }

    public void Render(LuaTable items)
    {
        if (items == null)
            return;
        for (int i = 1; i <= items.Length; i++)
        {
            var item = items.Get<int, LuaRenderItem>(i);
            var eid = item.eid;

            if (!transforms.TryGetValue(eid, out var transform))
            {
                var go = new GameObject(eid.ToString());
                transform = go.transform;
                transforms.Add(eid, transform);
                renderers.Add(eid, go.AddComponent<SpriteRenderer>());
            }


            // Transform
            transform.position = new Vector3(item.posX, item.posY, item.posZ);

            // Sprite
            var renderer = renderers[eid];
            var sortingOrder = -(int)(item.posY * 100);
            renderer.sortingOrder = sortingOrder;

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
            Debug.DrawRay(transform.position, new Vector3(item.fx, item.fy, 0), Color.red);
        }
    }
}