using UnityEngine;
using System;
using System.Collections.Generic;

namespace ECS
{
// ============================================
// 基础组件
// ============================================

    /// <summary>
    /// 位置组件
    /// </summary>
    [Serializable]
    public class PositionComponent
    {
        public float x;
        public float y;

        public PositionComponent()
        {
        }

        public PositionComponent(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    /// <summary>
    /// 速度组件
    /// </summary>
    [Serializable]
    public class VelocityComponent
    {
        public float x;
        public float y;

        public VelocityComponent()
        {
        }

        public VelocityComponent(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    /// <summary>
    /// 旋转组件
    /// </summary>
    [Serializable]
    public class RotationComponent
    {
        public float angle;

        public RotationComponent()
        {
        }

        public RotationComponent(float angle)
        {
            this.angle = angle;
        }
    }

    /// <summary>
    /// 生命值组件
    /// </summary>
    [Serializable]
    public class HealthComponent
    {
        public float current;
        public float max;
        public float regen; // 每秒回复

        public HealthComponent()
        {
        }

        public HealthComponent(float current, float max, float regen = 0f)
        {
            this.current = current;
            this.max = max;
            this.regen = regen;
        }
    }

// ============================================
// 视觉组件
// ============================================

    /// <summary>
    /// 精灵键组件
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
    /// 动画组件
    /// </summary>
    [Serializable]
    public class AnimationComponent
    {
        public string ClipSetId;
        public string ClipId;
        public int Frame;
        public float Time;
        public bool Playing;

        public string State;
        public string DefaultState;

        public AnimationComponent()
        {
        }

        public AnimationComponent(string defaultState)
        {
            this.DefaultState = defaultState;
        }
    }

    /// <summary>
    /// 动画指令组件
    /// </summary>
    [Serializable]
    public class AnimationCommandComponent
    {
        public string command; // "play", "stop", "pause"
        public string anim_name;
        public Action callback; // 完成后的回调

        public AnimationCommandComponent()
        {
        }

        public AnimationCommandComponent(string command, string animName, Action callback = null)
        {
            this.command = command;
            this.anim_name = animName;
            this.callback = callback;
        }
    }

// ============================================
// 战斗组件
// ============================================

    /// <summary>
    /// 伤害来源组件
    /// </summary>
    [Serializable]
    public class DamageSourceComponent
    {
        public float damage;
        public int owner_id;
        public string damage_type; // "physical", "fire", "ice", etc.

        public DamageSourceComponent()
        {
        }

        public DamageSourceComponent(float damage, int ownerId, string damageType = "physical")
        {
            this.damage = damage;
            this.owner_id = ownerId;
            this.damage_type = damageType;
        }
    }

    /// <summary>
    /// 碰撞体组件
    /// </summary>
    [Serializable]
    public class ColliderComponent
    {
        public float radius;
        public bool is_trigger;

        public ColliderComponent()
        {
            is_trigger = true;
        }

        public ColliderComponent(float radius, bool isTrigger = true)
        {
            this.radius = radius;
            this.is_trigger = isTrigger;
        }
    }

    /// <summary>
    /// 命中冷却组件
    /// </summary>
    [Serializable]
    public class HitCooldownComponent
    {
        public float duration;
        public float timer;

        public HitCooldownComponent()
        {
        }

        public HitCooldownComponent(float duration)
        {
            this.duration = duration;
            this.timer = 0f;
        }
    }

    /// <summary>
    /// 击退组件
    /// </summary>
    [Serializable]
    public class KnockBackComponent
    {
        public float force;
        public float duration;
        public float direction_x;
        public float direction_y;
        public float timer;

        public KnockBackComponent()
        {
        }

        public KnockBackComponent(float force, float duration, float dirX, float dirY)
        {
            this.force = force;
            this.duration = duration;
            this.direction_x = dirX;
            this.direction_y = dirY;
            this.timer = 0f;
        }
    }

// ============================================
// 武器/投射物组件
// ============================================

    /// <summary>
    /// 武器槽组件
    /// </summary>
    [Serializable]
    public class WeaponSlotsComponent
    {
        [Serializable]
        public class WeaponData
        {
            public string weapon_type;
            public int level;
            public float cooldown;
            public float fire_rate;

            public WeaponData()
            {
            }

            public WeaponData(string weaponType, int level = 1, float fireRate = 1.0f)
            {
                this.weapon_type = weaponType;
                this.level = level;
                this.fire_rate = fireRate;
                this.cooldown = 0f;
            }
        }

        public List<WeaponData> weapons;

        public WeaponSlotsComponent()
        {
            weapons = new List<WeaponData>();
        }
    }

    /// <summary>
    /// 投射物组件
    /// </summary>
    [Serializable]
    public class ProjectileComponent
    {
        public float speed;
        public int pierce; // 穿透次数
        public int hit_count; // 已命中次数
        public float lifetime;

        public ProjectileComponent()
        {
        }

        public ProjectileComponent(float speed, int pierce = 1, float lifetime = 5.0f)
        {
            this.speed = speed;
            this.pierce = pierce;
            this.hit_count = 0;
            this.lifetime = lifetime;
        }
    }

    /// <summary>
    /// 轨道运动组件
    /// </summary>
    [Serializable]
    public class OrbitComponent
    {
        public int center_entity;
        public float radius;
        public float angular_speed;
        public float current_angle;

        public OrbitComponent()
        {
        }

        public OrbitComponent(int centerEntity, float radius, float angularSpeed, float currentAngle = 0f)
        {
            this.center_entity = centerEntity;
            this.radius = radius;
            this.angular_speed = angularSpeed;
            this.current_angle = currentAngle;
        }
    }

    /// <summary>
    /// 追踪目标组件
    /// </summary>
    [Serializable]
    public class SeekComponent
    {
        public int target_id;
        public float turn_speed;

        public SeekComponent()
        {
        }

        public SeekComponent(int targetId, float turnSpeed = 5.0f)
        {
            this.target_id = targetId;
            this.turn_speed = turnSpeed;
        }
    }

// ============================================
// 特殊效果组件
// ============================================

    /// <summary>
    /// AOE 爆炸组件
    /// </summary>
    [Serializable]
    public class AoeExplosionComponent
    {
        public float radius;
        public float damage;
        public bool triggered;

        public AoeExplosionComponent()
        {
        }

        public AoeExplosionComponent(float radius, float damage)
        {
            this.radius = radius;
            this.damage = damage;
            this.triggered = false;
        }
    }

    /// <summary>
    /// 生命周期组件
    /// </summary>
    [Serializable]
    public class LifeTimeComponent
    {
        public float duration;
        public float elapsed;

        public LifeTimeComponent()
        {
        }

        public LifeTimeComponent(float duration)
        {
            this.duration = duration;
            this.elapsed = 0f;
        }
    }

    /// <summary>
    /// 掉落物体组件
    /// </summary>
    [Serializable]
    public class FallingBodyComponent
    {
        public float gravity;
        public float bounce;
        public float velocity_y;

        public FallingBodyComponent()
        {
            gravity = -9.8f;
            bounce = 0.5f;
        }

        public FallingBodyComponent(float gravity = -9.8f, float bounce = 0.5f, float initialVelocityY = 0f)
        {
            this.gravity = gravity;
            this.bounce = bounce;
            this.velocity_y = initialVelocityY;
        }
    }

// ============================================
// 标签组件
// ============================================

    /// <summary>
    /// 玩家标签组件
    /// </summary>
    [Serializable]
    public class PlayerTagComponent
    {
        // 空组件，仅用于标记
    }

    /// <summary>
    /// 敌人标签组件
    /// </summary>
    [Serializable]
    public class EnemyTagComponent
    {
        // 空组件，仅用于标记
    }

    /// <summary>
    /// 道具组件
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
            this.prop_type = propType;
            this.value = value;
        }
    }

// ============================================
// AI 组件
// ============================================

    /// <summary>
    /// 移动意图组件
    /// </summary>
    [Serializable]
    public class MoveIntentComponent
    {
        public float target_x;
        public float target_y;
        public float speed;

        public MoveIntentComponent()
        {
        }

        public MoveIntentComponent(float targetX, float targetY, float speed)
        {
            this.target_x = targetX;
            this.target_y = targetY;
            this.speed = speed;
        }
    }

    /// <summary>
    /// 转向行为组件
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
    /// 分离行为组件
    /// </summary>
    [Serializable]
    public class SeparationComponent
    {
        public float radius;
        public float strength;

        public SeparationComponent()
        {
            radius = 2.0f;
            strength = 1.0f;
        }

        public SeparationComponent(float radius, float strength)
        {
            this.radius = radius;
            this.strength = strength;
        }
    }
}