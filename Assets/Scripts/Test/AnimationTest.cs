using System;
using System.Collections.Generic;
using Lua;
using UnityEngine;
using XLua;

public class AnimationTest : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private LuaEnv luaEnv;

    SpriteRenderSystem renderSystem = new();
    List<Entity> entities = new();

    void Start()
    {
        Application.targetFrameRate = 60;
        luaEnv = LuaMain.Env;

        luaEnv.DoString(@"require('test/animation_test')");

        // 创建 Lua Entity
        luaEnv.Global.Get<Action<SpriteRenderer>>("CreateTestEntity")
            ?.Invoke(spriteRenderer);

        // Unity 侧 mirror entity（最小桥）
        var e = new Entity
        {
            SpriteKeyComponent = new SpriteKeyComponent(),
            SpriteRendererComponent = new SpriteRendererComponent
            {
                renderer = spriteRenderer
            }
        };

        entities.Add(e);

        // Lua → C# 同步 key（演示用）
        luaEnv.Global.Set("SyncSpriteKey", new Action<string>(key =>
        {
            e.SpriteKeyComponent.key = key;
        }));
    }

    // Update is called once per frame
    void Update()
    {
        // 驱动 ECS 系统更新
        var updateFunc = luaEnv.Global.Get<LuaFunction>("UpdateGame");
        updateFunc?.Call(Time.deltaTime);
        renderSystem.Update(entities);
    }
}