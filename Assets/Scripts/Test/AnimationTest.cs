using System;
using System.Collections.Generic;
using Lua;
using UnityEngine;
using XLua;

public class AnimationTest : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private LuaEnv luaEnv;

    RenderSystem renderSystem = new(new SpriteProvider());
    LuaRenderBridgeProxy renderBridge;
    private LuaTable world;

    void Start()
    {
        Application.targetFrameRate = 60;
        luaEnv = LuaMain.Env;

        luaEnv.DoString(@"require('test/animation_test')
                                require('Presentation/lua_render_bridge')
                            ");

        // 创建 Lua Entity
        luaEnv.Global.Get<Action<Transform, SpriteRenderer>>("CreateTestEntity")
            ?.Invoke(null, spriteRenderer);

        world = luaEnv.Global.Get<LuaTable>("MainWorld");
        renderBridge = new LuaRenderBridgeProxy(luaEnv);
    }

    // Update is called once per frame
    void Update()
    {
        // 驱动 ECS 系统更新
        var updateFunc = luaEnv.Global.Get<LuaFunction>("UpdateGame");
        updateFunc?.Call(Time.deltaTime);

        var renderItems = renderBridge.Collect(world);
        renderSystem.Render(renderItems);
    }
}