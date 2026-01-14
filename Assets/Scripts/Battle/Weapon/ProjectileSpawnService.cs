using ECS.Core;
using UnityEngine;
using ConfigHandler;

namespace ECS.Systems
{
    /// <summary>
    /// 投射物生成服务
    /// 职责：根据 WeaponConfig 和运行时属性生成投射物实体
    /// </summary>
    public static class ProjectileSpawnService
    {
        public static Vector2 Calculate(
            Vector2 baseDir,
            int index,
            int count,
            float spreadAngleDeg
        )
        {
            if (count <= 1) return baseDir;

            float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x);
            float spreadRad = spreadAngleDeg * Mathf.Deg2Rad;
            float start = baseAngle - spreadRad * 0.5f;
            float step = spreadRad / (count - 1);

            float angle = start + step * index;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        public static void Spawn(
            World world,
            int ownerId,
            PositionComponent ownerPos,
            Vector2 direction,
            WeaponConfig cfg,
            int level,
            WeaponRuntimeStatsComponent.WeaponStats stats = null)
        {
            var battle = cfg.battle;
            var view = cfg.view;

            int entity = world.CreateEntity();

            // ================= Position =================
            world.AddComponent(entity, new PositionComponent(
                ownerPos.x,
                ownerPos.y
            ));

            // ================= Velocity =================
            // 使用运行时属性或基础配置
            float speed = stats != null ? stats.GetFinalProjectileSpeed() : battle.projectile.speed;
            world.AddComponent(entity, new VelocityComponent(
                direction.x * speed,
                direction.y * speed
            ));

            // ================= Rotation =================
            float angle = Mathf.Atan2(direction.y, direction.x);
            world.AddComponent(entity, new RotationComponent(angle));

            // ================= Projectile =================
            float range = stats != null ? stats.GetFinalProjectileRange() : battle.projectile.range;
            float lifetime = range / speed;
            world.AddComponent(entity, new ProjectileComponent(
                speed: speed,
                pierce: 1,
                lifetime: lifetime
            ));

            // ================= Damage =================
            // 使用运行时属性计算最终伤害
            float finalDamage = stats != null ? stats.GetFinalDamage() : (battle.baseStats.damage * level);
            float finalKnockback = stats != null ? stats.GetFinalKnockback() : battle.baseStats.knockback;

            world.AddComponent(entity, new DamageSourceComponent
            {
                damage = finalDamage,
                knockBack = finalKnockback
            });

            // ================= Collider =================
            world.AddComponent(entity, new ColliderComponent(0.3f));

            // ================= View =================
            world.AddComponent(entity, new SpriteKeyComponent
            {
                sheet = view.sprite.sheet,
                key = view.sprite.key
            });

            // ================= LifeTime =================
            world.AddComponent(entity, new LifeTimeComponent(lifetime));

            // Debug日志：显示最终使用的属性
            if (stats != null)
            {
                Debug.Log($"[ProjectileSpawnService] 生成投射物 - 伤害:{finalDamage:F1}, 速度:{speed:F1}, 射程:{range:F1}");
            }
        }
    }
}