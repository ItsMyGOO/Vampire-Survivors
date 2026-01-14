using Lua;
using UnityEngine;
using XLua;
using Random = UnityEngine.Random;

namespace Test
{
    public class BattleTest : MonoBehaviour
    {
        [Header("Prefabs")] public GameObject playerPrefab;
        public GameObject enemyPrefab;

        private LuaEnv luaEnv;

        void Start()
        {
            Application.targetFrameRate = 60;
            
            luaEnv = LuaMain.Env;

            // 注册 C# 对象给 Lua 使用
            luaEnv.Global.Set("CSPlayerPrefab", playerPrefab);
            luaEnv.Global.Set("CSEnemyPrefab", enemyPrefab);

            // 启动 Lua 主逻辑
            luaEnv.DoString(@"require('test/battle_test')");
            
            // 生成玩家
            luaEnv.Global.Get<LuaFunction>("SpawnPlayer").Call(0, 0);

            // 生成 10 个敌人（随机位置）
            for (int i = 0; i < 10; i++)
            {
                float x = Random.Range(-8f, 8f);
                float y = Random.Range(-8f, 8f);
                luaEnv.Global.Get<LuaFunction>("SpawnEnemy").Call(x, y);
            }
        }

        void Update()
        {
            // 驱动 ECS 系统更新
            var updateFunc = luaEnv.Global.Get<LuaFunction>("UpdateGame");
            updateFunc?.Call(Time.deltaTime);
        }
    }
}