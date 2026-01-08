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
        sheet = "Assets/3rdParty/Undead Survivor/Sprites/Props.png",
        key = "Bullet 0",

        pipeline = {
            "CalcInterval",
            "CalcCount",
            "SelectTarget",
            "CalcDirection",
            "EmitProjectile"
        }
    },

    OrbitKnife = {
        type        = "Orbit",
        base_count  = 3,
        base_radius = 1.8,
        base_damage = 6,
        orbit_speed = 2.5,
        sheet       = "Assets/3rdParty/Undead Survivor/Sprites/Props.png",
        key         = "Bullet 2",

        pipeline    = {
            "CalcOrbitCount",
            "CalcOrbitRadius",
            "CalcOrbitAngle",
            "SpawnOrbit"
        }
    }

}
