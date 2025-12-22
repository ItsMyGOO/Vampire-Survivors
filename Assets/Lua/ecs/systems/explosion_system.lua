---
--- Created by echo.
--- DateTime: 2025/12/22 15:38
---
-- 处理爆炸伤害
local Explosion = require("ecs.components.aoe_explosion")
local Health = require("ecs.components.health")
local Transform = require("ecs.components.transform")

local ExplosionSystem = {
    world = nil
}

function ExplosionSystem:Update(dt)
    local explosions = self.world.components[Explosion.__name]
    local healths = self.world.components[Health.__name]
    local transforms = self.world.components[Transform.__name]

    if not explosions or not healths then return end

    for eid, expl in pairs(explosions) do
        if not expl.hasExploded then
            expl.hasExploded = true

            -- 获取爆炸中心
            local trans = transforms[eid]
            local cx, cy, cz = trans.x, trans.y, trans.z

            -- 遍历所有带 Health 的实体，计算距离
            for heId, health in pairs(healths) do
                if heId ~= eid then  -- 不伤自己
                    local hTrans = transforms[heId]
                    local dx = hTrans.x - cx
                    local dz = hTrans.z - cz
                    local distSq = dx*dx + dz*dz

                    if distSq <= expl.radius * expl.radius then
                        health.value = health.value - expl.damage
                        if health.value <= 0 then
                            -- 死亡事件
                            CS.LuaManager.Instance.CallCSharp("OnEnemyKilled", hTrans.go)
                            self.world:RemoveEntity(heId)
                        end
                    end
                end
            end

            -- 播放特效（通过 C#）
            CS.LuaManager.Instance.CallCSharp("PlayExplosionEffect", cx, cy, cz)
        end
    end
end

return ExplosionSystem