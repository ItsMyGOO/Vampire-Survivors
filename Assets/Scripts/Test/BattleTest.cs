using System;
using Lua;
using UnityEngine;
using XLua;

namespace Test
{
    public class BattleTest : MonoBehaviour
    {
        void Start()
        {
            var buffSystem = LuaMain.Env.DoString(@"
            local bs = BuffSystem.new()
            bs:add(FireBoost.new())
            return bs
        ")[0] as LuaTable;
        
            var fireball = LuaMain.Env.Global.Get<LuaTable>("FireBall");
        
            var cast = fireball.Get<Action<LuaTable>>("cast");
        
            var ctx = LuaMain.Env.NewTable();
            ctx.Set("caster", new { id = 1 });
            ctx.Set("target", new { id = 2 });
            ctx.Set("base_damage", 100);
            ctx.Set("buff_system", buffSystem);
        
            cast(ctx);
        }
    }
}