using ConfigHandler;
using ECS;
using ECS.Core;
using UnityEngine;

namespace Battle.Weapon
{
    /// <summary>
    /// 轨道武器生成服务
    /// 职责：根据 WeaponFinalStats 生成环绕型武器实体
    /// </summary>
    public static class OrbitSpawnService
    {
        public static void Spawn(
            World world,
            int ownerId,
            float startAngle,
            WeaponFinalStats stats,
            WeaponConfig cfg)
        {
            int entity = world.CreateEntity();

            // ================= Position =================
            var ownerPos = world.GetComponent<PositionComponent>(ownerId);
            world.AddComponent(entity, new PositionComponent(
                ownerPos.x,
                ownerPos.y
            ));

            // ================= Orbit =================
            world.AddComponent(entity, new OrbitComponent
            {
                centerEntity = ownerId,
                radius = stats.orbit.radius,
                angularSpeed = stats.orbit.speed,
                currentAngle = startAngle
            });

            // ================= Damage =================
            world.AddComponent(entity, new DamageSourceComponent
            {
                damage = stats.damage,
                knockBack = stats.knockback
            });

            // ================= Collider =================
            world.AddComponent(entity, new ColliderComponent(0.4f));

            // ================= Rotation =================
            world.AddComponent(entity, new RotationComponent(startAngle));
            
            // ================= View =================
            var view = cfg.view;
            world.AddComponent(entity, new SpriteKeyComponent
            {
                sheet = view.sprite.sheet,
                key = view.sprite.key
            });
            // ================= Debug =================
#if UNITY_EDITOR
            Debug.Log(
                $"[OrbitSpawnService] Spawn Orbit | dmg:{stats.damage:F1} r:{stats.orbit.radius:F1} spd:{stats.orbit.speed:F1}");
#endif
        }
    }
}