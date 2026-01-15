namespace Battle.Weapon
{
    /// <summary>
    /// 武器在某一时刻的最终战斗数值
    /// System / Spawn / Damage 只依赖这个
    /// </summary>
    public class WeaponFinalStats
    {
        // ===== 通用 =====
        public float damage;
        public float knockback;

        public ProjectileFinalStats projectile;
        public OrbitFinalStats orbit;
    }

    public class ProjectileFinalStats
    {
        public int count;
        public float interval;
        public float speed;
        public float range;
    }

    public class OrbitFinalStats
    {
        public int count;
        public float radius;
        public float speed;
    }
}