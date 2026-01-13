# 升级系统集成指南

## 概述
这个升级系统实现了以下功能：
1. 玩家升级时调用Lua逻辑生成三个随机选项（武器或被动属性）
2. 自动或手动选择一个选项
3. 将选项应用到C#的ECS系统中

## 文件结构

### C#文件
- `PlayerAttributeComponent.cs` - 玩家属性组件，包含所有可升级的属性
- `ExpSystem.cs` - 经验和升级系统，负责调用Lua和应用升级
- `UpgradeApplicationSystem.cs` - 升级应用系统，负责将属性同步到ECS组件
- `PlayerContext.cs` - 玩家上下文，管理玩家实体和经验数据

### Lua文件
- `upgrade_system.lua` - Lua升级系统，负责生成和配置升级选项

## 使用方法

### 1. 初始化系统

```csharp
// 在游戏初始化时
public class GameInitializer : MonoBehaviour
{
    private LuaEnv luaEnv;
    private World world;
    private int playerEntity;
    
    void Start()
    {
        // 创建LuaEnv
        luaEnv = new LuaEnv();
        
        // 创建ECS World
        world = new World();
        
        // 创建玩家实体
        playerEntity = world.CreateEntity();
        world.AddComponent(playerEntity, new PositionComponent(0, 0));
        world.AddComponent(playerEntity, new HealthComponent(100, 100, 0));
        world.AddComponent(playerEntity, new PlayerAttributeComponent());
        
        // 初始化PlayerContext
        PlayerContext.Instance.Initialize();
        PlayerContext.Instance.SetPlayerEntity(playerEntity);
        
        // 初始化ExpSystem
        ExpSystem.Instance.Init(luaEnv, PlayerContext.Instance, world, playerEntity);
        
        // 订阅升级选项事件（可选，用于UI显示）
        ExpSystem.Instance.OnUpgradeOptionsAvailable += OnUpgradeOptionsAvailable;
    }
    
    private void OnUpgradeOptionsAvailable(List<UpgradeOption> options)
    {
        Debug.Log("Upgrade options available:");
        foreach (var option in options)
        {
            Debug.Log($"- [{option.type}] {option.name}: {option.description}");
        }
        
        // 这里可以显示UI让玩家选择
        // 或者自动选择第一个
        // ExpSystem.Instance.ApplyUpgrade(options[0].id);
    }
}
```

### 2. 添加经验

```csharp
// 当玩家获得经验时
ExpSystem.Instance.AddExp(50);
```

### 3. 手动应用升级（如果需要UI选择）

```csharp
// 玩家在UI中选择了一个选项
public void OnPlayerSelectUpgrade(string upgradeId)
{
    ExpSystem.Instance.ApplyUpgrade(upgradeId);
}
```

### 4. 获取玩家属性

```csharp
// 获取玩家当前属性
PlayerAttributeComponent attributes = world.GetComponent<PlayerAttributeComponent>(playerEntity);

Debug.Log($"攻击力: {attributes.attackDamage}");
Debug.Log($"移动速度: {attributes.moveSpeed}");
Debug.Log($"暴击率: {attributes.criticalChance * 100}%");
```

## 配置升级选项

### 在Lua中添加新武器

编辑 `Assets/Lua/Game/upgrade_system.lua`：

```lua
local WEAPONS = {
    -- 添加新武器
    {
        id = "weapon_magic_missile",
        name = "魔法飞弹",
        description = "发射追踪魔法飞弹",
        icon = "icon_magic_missile",
        type = "weapon",
        baseValue = 12
    }
}
```

### 在Lua中添加新被动属性

```lua
local PASSIVE_UPGRADES = {
    -- 添加新被动
    {
        id = "passive_pierce",
        name = "穿透强化",
        description = "弹道穿透 +2",
        icon = "icon_pierce",
        type = "passive",
        attributeType = "PierceCount",
        value = 2
    }
}
```

## 扩展属性类型

### 1. 在C#中添加新属性

编辑 `PlayerAttributeComponent.cs`：

```csharp
public class PlayerAttributeComponent
{
    // 添加新属性
    public float newAttribute = 0f;
}

public enum AttributeType
{
    // 添加新枚举
    NewAttribute
}

// 在ApplyModifier中添加处理
public void ApplyModifier(AttributeModifier modifier)
{
    switch (modifier.attributeType)
    {
        case AttributeType.NewAttribute:
            newAttribute += modifier.value;
            break;
    }
}
```

### 2. 在Lua中使用新属性

```lua
{
    id = "passive_new_attr",
    name = "新属性",
    description = "新属性 +10",
    icon = "icon_new",
    type = "passive",
    attributeType = "NewAttribute",  -- 必须与C#枚举名称一致
    value = 10
}
```

## 注意事项

1. **属性类型命名**: Lua中的`attributeType`必须与C#的`AttributeType`枚举完全一致
2. **Lua索引**: Lua数组索引从1开始，不是0
3. **自动升级**: 当前实现会自动随机选择一个升级选项，如需UI选择请修改`ExpSystem.ProcessUpgrade()`
4. **武器系统**: 目前武器升级只是记录，需要额外实现武器生成和管理逻辑
5. **属性同步**: 某些属性(如生命值、移动速度)会自动同步到相关ECS组件

## 测试

```csharp
// 快速测试升级系统
public class UpgradeSystemTest : MonoBehaviour
{
    void Start()
    {
        // 添加大量经验触发升级
        ExpSystem.Instance.AddExp(1000);
    }
}
```

## 调试

启用日志查看升级过程：

```
[ExpSystem] Level Up! New Level: 2
[UpgradeSystem] Generated options:
  1. [passive] 生命强化 - 最大生命值 +20
  2. [weapon] 火球术 - 发射追踪火球攻击敌人
  3. [passive] 迅捷之靴 - 移动速度 +10%
[ExpSystem] Auto-selected upgrade: 生命强化
[UpgradeSystem] Passive applied: 生命强化 (MaxHealth +20.00)
[ExpSystem] Applied passive: MaxHealth +20
```
