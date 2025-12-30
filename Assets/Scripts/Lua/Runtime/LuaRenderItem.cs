using UnityEngine;
using XLua;

[LuaCallCSharp]
public struct LuaRenderItem
{
    public int eid;
    public Vector3 pos;
    public float velocityX;
    public string sheet;
    public string spriteKey;
}