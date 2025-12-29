using UnityEngine;
using XLua;

[LuaCallCSharp]
public struct LuaRenderItem
{
    public Transform transform;
    public SpriteRenderer renderer;
    public Vector3 pos;
    public float velocityX;
    public string sheet;
    public string spriteKey;
}