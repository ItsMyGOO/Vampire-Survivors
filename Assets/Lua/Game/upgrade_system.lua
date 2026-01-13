-- UpgradeSystem.lua
-- 升级系统 - 负责生成升级选项并应用升级效果

local UpgradeSystem = {}

-- 武器配置
local WEAPONS = {
    {
        id = "weapon_fireball",
        name = "火球术",
        description = "发射追踪火球攻击敌人",
        icon = "icon_fireball",
        type = "weapon",
        baseValue = 10
    },
    {
        id = "weapon_lightning",
        name = "闪电链",
        description = "释放连锁闪电",
        icon = "icon_lightning",
        type = "weapon",
        baseValue = 15
    },
    {
        id = "weapon_blade",
        name = "旋转刀刃",
        description = "环绕玩家旋转的刀刃",
        icon = "icon_blade",
        type = "weapon",
        baseValue = 8
    }
}

-- 被动属性配置
local PASSIVE_UPGRADES = {
    {
        id = "passive_max_health",
        name = "生命强化",
        description = "最大生命值 +20",
        icon = "icon_health",
        type = "passive",
        attributeType = "MaxHealth",
        value = 20
    },
    {
        id = "passive_move_speed",
        name = "迅捷之靴",
        description = "移动速度 +10%",
        icon = "icon_speed",
        type = "passive",
        attributeType = "MoveSpeed",
        value = 0.5
    },
    {
        id = "passive_attack_damage",
        name = "力量提升",
        description = "攻击力 +5",
        icon = "icon_damage",
        type = "passive",
        attributeType = "AttackDamage",
        value = 5
    },
    {
        id = "passive_attack_speed",
        name = "狂热",
        description = "攻击速度 +15%",
        icon = "icon_attack_speed",
        type = "passive",
        attributeType = "AttackSpeed",
        value = 0.15
    },
    {
        id = "passive_critical_chance",
        name = "精准打击",
        description = "暴击率 +5%",
        icon = "icon_crit",
        type = "passive",
        attributeType = "CriticalChance",
        value = 0.05
    },
    {
        id = "passive_area_size",
        name = "范围扩展",
        description = "技能范围 +15%",
        icon = "icon_area",
        type = "passive",
        attributeType = "AreaSize",
        value = 0.15
    },
    {
        id = "passive_projectile_count",
        name = "多重射击",
        description = "弹道数量 +1",
        icon = "icon_projectile",
        type = "passive",
        attributeType = "ProjectileCount",
        value = 1
    },
    {
        id = "passive_pickup_range",
        name = "磁力",
        description = "拾取范围 +1",
        icon = "icon_magnet",
        type = "passive",
        attributeType = "PickupRange",
        value = 1
    },
    {
        id = "passive_health_regen",
        name = "生命回复",
        description = "每秒回复生命值 +2",
        icon = "icon_regen",
        type = "passive",
        attributeType = "HealthRegen",
        value = 2
    },
    {
        id = "passive_cooldown_reduction",
        name = "急速冷却",
        description = "冷却时间 -10%",
        icon = "icon_cooldown",
        type = "passive",
        attributeType = "CooldownReduction",
        value = 0.1
    }
}

-- 玩家当前拥有的武器
local playerWeapons = {}

--- 初始化升级系统
function UpgradeSystem:Init()
    playerWeapons = {}
    print("[UpgradeSystem] Initialized")
end

--- 生成升级选项
--- @return table 三个升级选项的数组
function UpgradeSystem:RollOptions()
    local options = {}
    
    -- 随机决定选项类型（武器或被动）
    for i = 1, 3 do
        local option = self:GenerateRandomOption()
        table.insert(options, option)
    end
    
    print("[UpgradeSystem] Generated options:")
    for i, opt in ipairs(options) do
        print(string.format("  %d. [%s] %s - %s", i, opt.type, opt.name, opt.description))
    end
    
    return options
end

--- 生成随机升级选项
function UpgradeSystem:GenerateRandomOption()
    -- 50%概率选择武器，50%被动
    local roll = math.random()
    
    if roll < 0.4 and #playerWeapons < #WEAPONS then
        -- 生成武器选项（如果还有未获得的武器）
        return self:GenerateWeaponOption()
    else
        -- 生成被动属性选项
        return self:GeneratePassiveOption()
    end
end

--- 生成武器选项
function UpgradeSystem:GenerateWeaponOption()
    -- 找出未拥有的武器
    local availableWeapons = {}
    for _, weapon in ipairs(WEAPONS) do
        if not playerWeapons[weapon.id] then
            table.insert(availableWeapons, weapon)
        end
    end
    
    -- 如果所有武器都已拥有，返回被动属性
    if #availableWeapons == 0 then
        return self:GeneratePassiveOption()
    end
    
    -- 随机选择一个武器
    local weapon = availableWeapons[math.random(#availableWeapons)]
    
    return {
        id = weapon.id,
        name = weapon.name,
        description = weapon.description,
        icon = weapon.icon,
        type = "weapon",
        data = weapon
    }
end

--- 生成被动属性选项
function UpgradeSystem:GeneratePassiveOption()
    local passive = PASSIVE_UPGRADES[math.random(#PASSIVE_UPGRADES)]
    
    return {
        id = passive.id,
        name = passive.name,
        description = passive.description,
        icon = passive.icon,
        type = "passive",
        attributeType = passive.attributeType,
        value = passive.value,
        data = passive
    }
end

--- 应用升级选项
--- @param upgradeId string 升级选项ID
--- @return table 升级结果 { success: boolean, type: string, data: table }
function UpgradeSystem:ApplyUpgrade(upgradeId)
    print("[UpgradeSystem] Applying upgrade: " .. upgradeId)
    
    -- 查找选项配置
    local upgradeData = nil
    local upgradeType = nil
    
    -- 检查是否是武器
    for _, weapon in ipairs(WEAPONS) do
        if weapon.id == upgradeId then
            upgradeData = weapon
            upgradeType = "weapon"
            break
        end
    end
    
    -- 检查是否是被动属性
    if not upgradeData then
        for _, passive in ipairs(PASSIVE_UPGRADES) do
            if passive.id == upgradeId then
                upgradeData = passive
                upgradeType = "passive"
                break
            end
        end
    end
    
    if not upgradeData then
        print("[UpgradeSystem] Error: Upgrade not found - " .. upgradeId)
        return { success = false }
    end
    
    -- 应用升级
    if upgradeType == "weapon" then
        playerWeapons[upgradeId] = true
        print("[UpgradeSystem] Weapon added: " .. upgradeData.name)
    elseif upgradeType == "passive" then
        print(string.format("[UpgradeSystem] Passive applied: %s (%s +%.2f)", 
            upgradeData.name, upgradeData.attributeType, upgradeData.value))
    end
    
    return {
        success = true,
        type = upgradeType,
        attributeType = upgradeData.attributeType,
        value = upgradeData.value,
        data = upgradeData
    }
end

--- 获取玩家当前武器列表
function UpgradeSystem:GetPlayerWeapons()
    return playerWeapons
end

return UpgradeSystem
