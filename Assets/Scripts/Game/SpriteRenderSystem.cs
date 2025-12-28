using System.Collections.Generic;
using UnityEngine;

public class SpriteRenderSystem
{
    public void Update(IEnumerable<Entity> entities)
    {
        foreach (var e in entities)
        {
            var keyComp = e.SpriteKeyComponent;
            var renderer = e.SpriteRendererComponent.renderer;

            if (keyComp.key == null)
                continue;

            SpriteProvider.Get(keyComp.key, sprite =>
            {
                if (renderer != null)
                    renderer.sprite = sprite;
            });
        }
    }
}

public class Entity
{
    public SpriteKeyComponent SpriteKeyComponent;
    public SpriteRendererComponent SpriteRendererComponent;
}

public class SpriteKeyComponent
{
    public string key;
}

public class SpriteRendererComponent
{
    public SpriteRenderer renderer;
}