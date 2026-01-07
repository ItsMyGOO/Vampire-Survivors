---
--- Created by echo.
--- DateTime: 2026/1/6 17:46
---
return {

    -- 1️⃣ 发射型武器（飞刀 / 子弹）
    ProjectileKnife = {
        type = "Projectile",
        interval = 0.6,
        base_damage = 10,
        base_speed = 12,
        base_count = 1,
        range = 6,

        pipeline = {
            "CalcInterval",
            "CalcCount",
            "SelectTarget",
            "CalcDirection",
            "EmitProjectile"
        }
    },

    OrbitKnife = {
        type = "Orbit",
        count = 2,
        radius = 1.8,
        angular_speed = 180,
        base_damage = 6,

        pipeline = {
            "CalcOrbitCount",
            "CalcOrbitRadius",
            "SpawnOrbit"
        }
    }
}
