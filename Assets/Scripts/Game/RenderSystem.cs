using UnityEngine;
using XLua;

public class RenderSystem
{
    public void Render(LuaTable items)
    {
        if (items == null)
            return;
        for (int i = 1; i <= items.Length; i++)
        {
            var item = items.Get<int, LuaRenderItem>(i);

            // Transform
            if (item.transform)
                item.transform.position =
                    new Vector3(item.x, item.y, 0);

            // Sprite
            SpriteProvider.Get(item.sheet, item.spriteKey, sprite =>
            {
                if (item.renderer.sprite != sprite)
                    item.renderer.sprite = sprite;
            });
        }
    }
}