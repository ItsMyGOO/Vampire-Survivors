using Lua;
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
                item.transform.position = item.pos;
            item.renderer.flipX = item.velocityX switch
            {
                > 0 => false,
                < 0 => true,
                _ => item.renderer.flipX
            };
            // Sprite
            SpriteProvider.Get(item.sheet, item.spriteKey, sprite =>
            {
                if (item.renderer.sprite != sprite)
                    item.renderer.sprite = sprite;
            });
        }
    }
}