using System;
using System.Collections.Generic;

namespace ECS
{
// ============================================
// 基础组件（struct - 纯数值，无引用类型字段）
// ============================================

    /// <summary>
    /// 位置组件
    /// </summary>
    public struct PositionComponent
    {
        public float x;
        public float y;

        public PositionComponent(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    /// <summary>
    /// 速度组件
    /// </summary>
    public struct VelocityComponent
    {
        public float x;
        public float y;
        public float speed;

        public VelocityComponent(float x, float y, float speed = 1f)
        {
            this.x = x;
            this.y = y;
            this.speed = speed;
        }
    }

    /// <summary>
    /// 旋转组件
    /// </summary>
    public struct RotationComponent
    {
        public float angle;

        public RotationComponent(float angle)
        {
            this.angle = angle;
        }
    }

    /// <summary>
    /// 生命值组件
    /// </summary>
    public struct HealthComponent
    {
        public float current;
        public float max;
        public float regen; // 每秒回复

        public HealthComponent(float current, float max, float regen = 0f)
        {
            this.current = current;
            this.max = max;
            this.regen = regen;
        }
    }

// ============================================
// 视觉组件（class - 含 string 引用）
// ============================================

    /// <summary>
    /// 精灵键组件（保留 class：含 string 字段）
    /// </summary>
    [Serializable]
    public class SpriteKeyComponent
    {
        public string sheet;
        public string key;

        public SpriteKeyComponent()
        {
        }

        public SpriteKeyComponent(string key)
        {
            this.key = key;
        }
    }

    /// <summary>
    /// 动画组件（保留 class：含 string 字段）
    /// </summary>
    [Serializable]
    public class AnimationComponent
    {
        public string ClipSetName;
        public string AnimName;
        public int Frame;
        public float Time;
        public bool Playing;
        public string DefaultAnim;
    }

    /// <summary>
    /// 动画指令组件（保留 class：含 string 字段）
    /// </summary>
    [Serializable]
    public class AnimationCommandComponent
    {
        public string command; // "play", "stop", "pause"
        public string anim_name;

        public AnimationCommandComponent()
        {
        }

        public AnimationCommandComponent(string command, string animName)
        {
            this.command = command;
            anim_name = animName;
        }
    }

// ============================================
// 战斗组件（struct - 纯数值）
// ============================================

    /// <summary>
    /// 伤害来源组件
    /// </summary>
    public struct DamageSourceComponent
    {
        public float damage;
        public float knockBack;

        public DamageSourceComponent(float damage, float knockBack = 0f)
        {
            this.damage = damage;
            this.knockBack = knockBack;
        }
    }

    /// <summary>
    /// 碰撞体组件
    /// </summary>
    public struct ColliderComponent
    {
        public float radius;
        public bool is_trigger;

        public ColliderComponent(float radius, bool isTrigger = true)
        {
            this.radius = radius;
            is_trigger = isTrigger;
        }
    }

    /// <summary>
    /// 命中冷却组件
    /// </summary>
    public struct HitCooldownComponent
    {
        public float duration;
        public float timer;

        public HitCooldownComponent(float duration)
        {
            this.duration = duration;
            timer = 0f;
        }
    }

    /// <summary>
    /// 击退组件
    /// </summary>
    public struct KnockBackComponent
    {
        public float forceX;
        public float forceY;
        public float time;

        public KnockBackComponent(float forceX, float forceY, float time)
        {
            this.forceX = forceX;
            this.forceY = forceY;
            this.time = time;
        }
    }

// ============================================
// 武器/投射物组件（struct - 纯数值）
// ============================================

    /// <summary>
    /// 投射物组件
    /// </summary>
    public struct ProjectileComponent
    {
        public float speed;
        public int pierce;     // 穿透次数
        public int hit_count;  // 已命中次数
        public float lifetime;

        public ProjectileComponent(float speed, int pierce = 1, float lifetime = 5.0f)
        {
            this.speed = speed;
            this.pierce = pierce;
            hit_count = 0;
            this.lifetime = lifetime;
        }
    }

    /// <summary>
    /// 轨道运动组件
    /// </summary>
    public struct OrbitComponent
    {
        public int centerEntity;
        public float radius;
        public float angularSpeed;
        public float currentAngle;

        public OrbitComponent(int centerEntity, float radius, float angularSpeed, float currentAngle)
        {
            this.centerEntity = centerEntity;
            this.radius = radius;
            this.angularSpeed = angularSpeed;
            this.currentAngle = currentAngle;
        }
    }

    /// <summary>
    /// 追踪目标组件
    /// </summary>
    public struct SeekComponent
    {
        public int target_id;
        public float turn_speed;

        public SeekComponent(int targetId, float turnSpeed = 5.0f)
        {
            target_id = targetId;
            turn_speed = turnSpeed;
        }
    }

// ============================================
// 特殊效果组件（struct - 纯数值）
// ============================================

    /// <summary>
    /// AOE 爆炸组件
    /// </summary>
    public struct AoeExplosionComponent
    {
        public float radius;
        public float damage;
        public bool triggered;

        public AoeExplosionComponent(float radius, float damage)
        {
            this.radius = radius;
            this.damage = damage;
            triggered = false;
        }
    }

    /// <summary>
    /// 生命周期组件
    /// </summary>
    public struct LifeTimeComponent
    {
        public float duration;
        public float elapsed;

        public LifeTimeComponent(float duration)
        {
            this.duration = duration;
            elapsed = 0f;
        }
    }

// ============================================
// 标签组件（struct - 空标记）
// ============================================

    /// <summary>
    /// 玩家标签组件
    /// </summary>
    public struct PlayerTagComponent
    {
    }

    /// <summary>
    /// 敌人标签组件
    /// </summary>
    public struct EnemyTagComponent
    {
    }

    /// <summary>
    /// 相机跟随标签组件
    /// </summary>
    public struct CameraFollowComponent
    {
    }

// ============================================
// 道具组件（class - 含 string）
// ============================================

    /// <summary>
    /// 道具组件（保留 class：含 string 字段）
    /// </summary>
    [Serializable]
    public class PropComponent
    {
        public string prop_type; // "exp_gem", "health_potion", etc.
        public float value;

        public PropComponent()
        {
        }

        public PropComponent(string propType, float value)
        {
            prop_type = propType;
            this.value = value;
        }
    }

// ============================================
// AI 组件
// ============================================

    /// <summary>
    /// 转向行为组件（保留 class：含 List 和嵌套 class）
    /// </summary>
    [Serializable]
    public class SteeringComponent
    {
        [Serializable]
        public class BehaviorWeights
        {
            public float seek;
            public float separation;
            public float flee;

            public BehaviorWeights()
            {
                seek = 1.0f;
                separation = 0.5f;
            }
        }

        public List<string> behaviors; // "seek", "separation", "flee", etc.
        public BehaviorWeights weights;
        public float max_speed;

        public SteeringComponent()
        {
            behaviors = new List<string>();
            weights = new BehaviorWeights();
            max_speed = 5.0f;
        }
    }

    /// <summary>
    /// 分离行为组件（struct - 纯数值）
    /// </summary>
    public struct SeparationComponent
    {
        public float radius;
        public float strength;

        public SeparationComponent(float radius = 2.0f, float strength = 1.0f)
        {
            this.radius = radius;
            this.strength = strength;
        }
    }

// ============================================
// 拾取相关
// ============================================

    /// <summary>
    /// 拾取范围组件（struct - 纯数值）
    /// </summary>
    public struct PickupRangeComponent
    {
        public float radius;

        public PickupRangeComponent(float radius = 1.0f)
        {
            this.radius = radius;
        }
    }

    /// <summary>
    /// 可拾取组件（保留 class：含 string 字段）
    /// </summary>
    [Serializable]
    public class PickupableComponent
    {
        public string item_type; // "exp", "coin", "health", "magnet"
        public int value;
        public bool auto_pickup; // 是否自动拾取

        public PickupableComponent()
        {
            auto_pickup = true;
        }

        public PickupableComponent(string itemType, int value, bool autoPickup = true)
        {
            item_type = itemType;
            this.value = value;
            auto_pickup = autoPickup;
        }
    }

    /// <summary>
    /// 磁铁组件（struct - 纯数值）
    /// </summary>
    public struct MagnetComponent
    {
        public float radius;
        public float strength;
        public bool active;

        public MagnetComponent(float radius = 5.0f, float strength = 10.0f)
        {
            this.radius = radius;
            this.strength = strength;
            active = true;
        }
    }
// ============================================
// 生成状态组件
// ============================================

    /// <summary>
    /// 敌人生成状态组件（struct - 纯数值）
    /// 挂在全局 SpawnController 实体上，供 EnemySpawnSystem 读写。
    /// 将运行时状态从 System 内部移出，符合 ECS 架构规范。
    /// </summary>
    public struct SpawnStateComponent
    {
        public float gameTime;    // 累计游戏时间，用于难度递增
        public float spawnTimer;  // 距下次生成的计时器
        public float spawnInterval; // 当前生成间隔

        public SpawnStateComponent(float spawnInterval)
        {
            gameTime = 0f;
            spawnTimer = 0f;
            this.spawnInterval = spawnInterval;
        }
    }

// ============================================
// 游荡组件
// ============================================

    /// <summary>
    /// 游荡组件 —— 让实体在出生点附近随机行走。
    /// 不含任何逻辑，仅作数据容器。
    /// </summary>
    public struct WanderComponent
    {
        public float originX;       // 出生点 X（游荡中心）
        public float originY;       // 出生点 Y（游荡中心）
        public float radius;        // 游荡半径
        public float speed;         // 移动速度
        public float targetX;       // 当前目标点 X
        public float targetY;       // 当前目标点 Y
        public float waitTimer;     // 到达目标后的等待计时器
        public float waitDuration;  // 每次到达后等待时长（秒）
    }
}