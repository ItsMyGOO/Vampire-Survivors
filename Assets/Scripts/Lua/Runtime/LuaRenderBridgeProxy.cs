using XLua;

namespace Lua
{
    public class LuaRenderBridgeProxy
    {
        LuaTable bridge;
        LuaFunction collectFunc;

        public LuaRenderBridgeProxy(LuaEnv env)
        {
            bridge = env.Global.Get<LuaTable>("LuaRenderBridge");
            collectFunc = bridge.Get<LuaFunction>("Collect");
        }

        public LuaTable Collect(LuaTable world)
        {
            object[] ret = collectFunc.Call(bridge, world);
            return ret[0] as LuaTable;
        }
    }
}