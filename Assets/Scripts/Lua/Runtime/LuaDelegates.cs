using XLua;

namespace Lua
{
    [CSharpCallLua]
    public delegate int IntBinaryOp(int a, int b);
    
    [CSharpCallLua]
    public delegate object[] GetAnimConfigDelegate(string setId, string clipId);

}