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

        RenderSystem renderSystem = new(new SpriteProvider(), new RenderObjectPool());
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
            float hori = Input.GetAxisRaw("Horizontal");
            float vert = Input.GetAxisRaw("Vertical");
            var inputData = new InputData() { hori = hori, vert = vert };
            luaEnv.Global.Set("InputData", inputData);

            tick.Call(battle, Time.deltaTime);

            var renderItems = renderBridge.Collect(world);
            renderSystem.Render(renderItems);
        }
    }
}