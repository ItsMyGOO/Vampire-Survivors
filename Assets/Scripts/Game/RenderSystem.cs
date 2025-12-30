using System.Collections.Generic;
using Lua;
using UnityEngine;
using XLua;

public class RenderSystem
{
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

            var renderer = renderers[eid];
            // Transform
            transform.position = new Vector3(item.posX, item.posY, item.posZ);
            renderer.flipX = item.velocityX switch
            {
                > 0 => false,
                < 0 => true,
                _ => renderer.flipX
            };
            // Sprite

            spriteProvider.Get(item.sheet, item.spriteKey, sprite =>
            {
                if (renderer.sprite != sprite)
                    renderer.sprite = sprite;
            });
        }
    }
}