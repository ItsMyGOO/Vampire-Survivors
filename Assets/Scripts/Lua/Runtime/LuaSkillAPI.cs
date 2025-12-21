using UnityEngine;
using XLua;

[LuaCallCSharp]
public class LuaSkillAPI
{
    public static void CastFireball(int casterId, int targetId)
    {
        Debug.Log($"{casterId} CastFireball to {targetId}");
    }

    public static void ApplyDamage(int targetId, int damage)
    {
        Debug.Log($"[C#] target {targetId} take damage {damage}");

        // 这里是真正的扣血
        // EntityManager.Get(targetId).HP -= damage;
    }
}