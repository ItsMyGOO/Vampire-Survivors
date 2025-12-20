using XLua;

namespace Lua
{
    [CSharpCallLua]
    public delegate int IntBinaryOp(int a, int b);

    [CSharpCallLua]
    public delegate void LuaTick(float dt);

    [CSharpCallLua]
    public delegate bool Condition();
}