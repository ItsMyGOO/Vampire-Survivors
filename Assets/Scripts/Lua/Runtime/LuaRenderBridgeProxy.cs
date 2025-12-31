using XLua;

namespace Lua
{
    public class LuaRenderBridgeProxy
    {
        readonly LuaFunction _collectFunc;

        public LuaRenderBridgeProxy(LuaEnv env)
        {
            var bridge = env.Global.Get<LuaTable>("RenderBridge");
            _collectFunc = bridge.Get<LuaFunction>("Collect");
        }

        public LuaTable Collect(LuaTable world)
        {
            object[] ret = _collectFunc.Call(world);
            return ret[0] as LuaTable;
        }
    }
}