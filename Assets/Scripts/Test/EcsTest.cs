using Lua;
using UnityEngine;
using XLua;

namespace Test
{
    public class EcsTest : MonoBehaviour
    {
        private int entityId;
        private LuaEnv luaEnv;

        void Start()
        {
            luaEnv = LuaMain.Env;
            entityId = (int)luaEnv.Global.Get<LuaFunction>("SpawnEnemy")
                .Call(gameObject, 0, 0, 0)[0];
        }

        void Update()
        {
            // 触发 ECS 全局更新
            var worldUpdate = luaEnv.Global.Get<LuaFunction>("MainWorld.UpdateSystems");
            worldUpdate.Call(Time.deltaTime);
        }
    }
}