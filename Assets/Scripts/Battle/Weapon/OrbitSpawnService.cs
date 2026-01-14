using ECS.Core;
using UnityEngine;
using ConfigHandler;

namespace ECS.Systems
{
    /// <summary>
    /// 轨道武器生成服务
    /// 职责：生成环绕型武器实体（支持运行时属性）
    /// </summary>
    public static class OrbitSpawnService
    {
        public static void Spawn(
            World world,
            int ownerId,
            float startAngle,
            WeaponConfig cfg,
            int level,
            WeaponRuntimeStatsComponent.WeaponStats stats = null)
        {
            var battle = cfg.battle;
            var view = cfg.view;
            
            int entity = world.CreateEntity();

            // ================= Position =================
            var ownerPos = world.GetComponent<PositionComponent>(ownerId);
            world.AddComponent(entity, new PositionComponent(
                ownerPos.x,
                ownerPos.y
            ));

            // ================= Orbit =================
            // 使用运行时属性或基础配置
            float radius = stats != null ? stats.GetFinalOrbitRadius() : (battle.orbit.radius * Mathf.Sqrt(level));
            float speed = stats != null ? stats.GetFinalOrbitSpeed() : battle.orbit.speed;
            
            world.AddComponent(entity, new OrbitComponent
            {
                centerEntity = ownerId,
                radius = radius,
                angularSpeed = speed,
                currentAngle = startAngle
            });

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
            world.AddComponent(entity, new ColliderComponent(0.4f));

            // ================= Rotation =================
            world.AddComponent(entity, new RotationComponent(startAngle));
            
            world.AddComponent(entity, new SpriteKeyComponent
            {
                sheet = view.sprite.sheet,
                key = view.sprite.key
            });
            
            // Debug日志：显示最终使用的属性
            if (stats != null)
            {
                Debug.Log($"[OrbitSpawnService] 生成轨道武器 - 伤害:{finalDamage:F1}, 半径:{radius:F1}, 速度:{speed:F1}");
            }
        }
    }
}
