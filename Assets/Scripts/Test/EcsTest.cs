using Lua;
using UnityEngine;
using XLua;

namespace Test
{
    public class EcsTest : MonoBehaviour
    {
        [Header("Prefabs")] public GameObject playerPrefab;
        public GameObject enemyPrefab;

        private LuaEnv luaEnv;

        void Start()
        {
            luaEnv = LuaMain.Env;

            // 注册 C# 对象给 Lua 使用
            luaEnv.Global.Set("CSPlayerPrefab", playerPrefab);
            luaEnv.Global.Set("CSEnemyPrefab", enemyPrefab);
            luaEnv.Global.Set("CSLuaManager", this); // 提供日志等接口

            // 启动 Lua 主逻辑
            luaEnv.DoString(@"require('test/ecs_test')");


            // 生成玩家
            luaEnv.Global.Get<LuaFunction>("SpawnPlayer").Call(0, 0);

            // 生成 10 个敌人（随机位置）
            for (int i = 0; i < 10; i++)
            {
                float x = Random.Range(-8f, 8f);
                float z = Random.Range(-8f, 8f);
                luaEnv.Global.Get<LuaFunction>("SpawnEnemy").Call(x, z);
            }
        }

        void Update()
        {
            // 驱动 ECS 系统更新
            var updateFunc = luaEnv.Global.Get<LuaFunction>("UpdateGame");
            updateFunc?.Call(Time.deltaTime);
        }

        // 可供 Lua 调用的日志函数
        public void Log(string msg) => Debug.Log("[Lua] " + msg);
    }
}