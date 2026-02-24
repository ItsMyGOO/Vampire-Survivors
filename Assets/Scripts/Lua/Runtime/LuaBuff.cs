using System;
using Core.Buff;
using XLua;

public class LuaBuff : IBuff
{
    private LuaTable _luaTable;

    public LuaBuff(LuaTable table)
    {
        _luaTable = table;
    }

    public void OnAdd(ISkillTarget owner)
    {
        _luaTable.Get<Action<ISkillTarget>>("OnAdd")?.Invoke(owner);
    }

    public void OnRemove()
    {
        _luaTable.Get<Action>("OnRemove")?.Invoke();
    }

    public void Modify(SkillContext ctx)
    {
        _luaTable.Get<Action<SkillContext>>("ModifyDamage")?.Invoke(ctx);
    }
}