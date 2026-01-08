using System;
using XLua;

[LuaCallCSharp]
public struct LuaRenderItem
{
    public int eid;

    public float posX, posY, posZ;

    public float dirX;
    public float rotation;

    public string sheet, spriteKey;

    public float fx, fy;

    public RenderFlags flags;
}

[Flags]
public enum RenderFlags : byte
{
    None = 0,
    UseFlipX = 1 << 0,
    UseRotation = 1 << 1,
}