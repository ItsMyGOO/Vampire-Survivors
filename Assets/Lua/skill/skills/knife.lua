---
--- Created by echo.
--- DateTime: 2026/1/5 17:25
---
-- Skill_Knife.lua
Skill_Knife = {}
Skill_Knife.__index = Skill_Knife
setmetatable(Skill_Knife, { __index = SkillBase })

function Skill_Knife.new()
    return setmetatable(SkillBase.new(1.0), Skill_Knife)
end

function Skill_Knife:Fire(player)
    local stats = player.stats
    for i = 1, stats.projectileCount do
        print("Spawn Knife dmg:", stats.damage)
        -- SpawnProjectile(...)
    end
end
