---
--- Created by echo.
--- DateTime: 2025/12/21 11:00
---
return {
    fireball = {
        base_damage = 100,
        cost = 20,
        tags = { "fire", "magic" }
    },
    bullet_rain = {
        duration = 5.0,        -- 持续时间
        interval = 0.2,        -- 每隔多久掉落一颗
        total_drops = 100,     -- 总掉落数
        min_height = 10.0,     -- 起始高度
        fall_speed = 3.0,      -- 下落速度
        damage = 50,
        explosion_radius = 2.5,
        spawn_area = { x = 20, z = 20 }     -- 在玩家周围 X×Z 区域内随机
    }
}
