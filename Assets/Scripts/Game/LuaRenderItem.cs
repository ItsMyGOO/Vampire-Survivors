using UnityEngine;
using XLua;

[LuaCallCSharp]
public struct LuaRenderItem
{
    public Transform transform;
    public SpriteRenderer renderer;
    public float x;
    public float y;
    public float z;
    public string sheet;
    public string spriteKey;
}