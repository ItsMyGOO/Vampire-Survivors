using Lua;
using UnityEngine;
using XLua;

namespace Game.Battle
{
    public class LuaBattleDriver : MonoBehaviour
    {
        private LuaTable battle;
        private LuaFunction tick;
        private LuaEnv luaEnv;

        RenderSystem renderSystem = new(new SpriteProvider());
        LuaRenderBridgeProxy renderBridge;
        private LuaTable world;

        void Start()
        {
            luaEnv = LuaMain.Env;
            luaEnv.DoString(@"require('battle.battle_entry')");

            battle = luaEnv.Global.Get<LuaTable>("Battle");
            var start = battle.Get<LuaFunction>("StartBattle");
            start.Call(battle, null);
            world = battle.Get<LuaTable>("world");

            renderBridge = new LuaRenderBridgeProxy(luaEnv);

            tick = battle.Get<LuaFunction>("Tick");
        }

        void Update()
        {
            tick.Call(battle, Time.deltaTime);

            var renderItems = renderBridge.Collect(world);
            renderSystem.Render(renderItems);
        }
    }
}