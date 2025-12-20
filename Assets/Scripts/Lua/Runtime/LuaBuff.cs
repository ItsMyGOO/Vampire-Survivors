using Core;
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
        _luaTable.Get<System.Action<ISkillTarget>>("OnAdd")?.Invoke(owner);
    }

    public void OnRemove()
    {
        _luaTable.Get<System.Action>("OnRemove")?.Invoke();
    }

    public void Modify(SkillContext ctx)
    {
        _luaTable.Get<System.Action<SkillContext>>("ModifyDamage")?.Invoke(ctx);
    }
}