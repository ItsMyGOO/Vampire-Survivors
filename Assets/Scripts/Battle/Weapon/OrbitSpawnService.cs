using ECS.Core;
using UnityEngine;
using ConfigHandler;

namespace ECS.Systems
{
    /// <summary>
    /// 轨道武器生成服务
    /// 职责：生成环绕型武器实体
    /// </summary>
    public static class OrbitSpawnService
    {
        public static void Spawn(
            World world,
            int ownerId,
            float startAngle,
            WeaponConfig cfg,
            int level)
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
            world.AddComponent(entity, new OrbitComponent
            {
                centerEntity = ownerId,
                radius = battle.orbit.radius * Mathf.Sqrt(level),
                angularSpeed = battle.orbit.speed,
                currentAngle = startAngle
            });

            // ================= Damage =================
            world.AddComponent(entity, new DamageSourceComponent
            {
                damage = battle.baseStats.damage * level,
                knockBack = battle.baseStats.knockback
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
        }
    }
}