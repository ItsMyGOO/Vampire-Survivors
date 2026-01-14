-- UpgradeSystem.lua
-- 升级系统 - 负责生成升级选项并应用升级效果

local UpgradeSystem = {}
local UpgradeService = CS.Game.UpgradeService

local ownedWeapons = {}

function UpgradeSystem:Init()
    ownedWeapons = {}
end

function UpgradeSystem:RollOptions(playerLevel)
    return UpgradeService.RollWeaponOptions(
        3,
        playerLevel,
        ownedWeapons
    )
end

function UpgradeSystem:ApplyUpgrade(weaponId)
    ownedWeapons[weaponId] = (ownedWeapons[weaponId] or 0) + 1
end

return UpgradeSystem
