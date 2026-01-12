using System;
using System.Collections.Generic;
using ECS.Core;

namespace ECS
{
    public class Render
    {
        public static List<LuaRenderItem> Collect(World world)
        {
            var list = new List<LuaRenderItem>(256);

            // 等价于 Lua: for eid, pos in pairs(positions) do
            foreach (var (entity, position) in world.GetComponents<PositionComponent>())
            {
                SpriteKeyComponent spriteKey = null;
                VelocityComponent velocity = null;
                RotationComponent rotation = null;

                if (world.HasComponent<SpriteKeyComponent>(entity))
                    spriteKey = world.GetComponent<SpriteKeyComponent>(entity);

                if (world.HasComponent<VelocityComponent>(entity))
                    velocity = world.GetComponent<VelocityComponent>(entity);

                if (world.HasComponent<RotationComponent>(entity))
                    rotation = world.GetComponent<RotationComponent>(entity);

                // RenderItem 是 struct：局部变量，安全
                var item = new LuaRenderItem
                {
                    eid = entity,
                    posX = position.x,
                    posY = position.y,
                };

                RenderFlags flags = RenderFlags.None;

                if (velocity != null)
                {
                    item.dirX = velocity.x;
                    flags |= RenderFlags.UseFlipX;
                }

                if (rotation != null)
                {
                    item.rotation = rotation.angle;
                    flags |= RenderFlags.UseRotation;
                }

                item.flags = flags;

                if (spriteKey != null)
                {
                    item.sheet = spriteKey.sheet;
                    item.spriteKey = spriteKey.key;
                }

                list.Add(item);
            }

            return list;
        }
    }
    
    [Flags]
    public enum RenderFlags : byte
    {
        None = 0,
        UseFlipX = 1 << 0,
        UseRotation = 1 << 1,
    }
}