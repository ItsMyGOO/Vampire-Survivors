using ECS.Core;
using UnityEngine;
using ConfigHandler;

namespace ECS.Systems
{
    /// <summary>
    /// 投射物生成服务
    /// 职责：根据 WeaponConfig 生成投射物实体
    /// 不关心冷却、不关心目标选择
    /// </summary>
    public static class ProjectileSpawnService
    {
        public static void Spawn(
            World world,
            int ownerId,
            PositionComponent ownerPos,
            Vector2 direction,
            WeaponConfig cfg,
            int level)
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
            float speed = battle.projectile.speed;
            world.AddComponent(entity, new VelocityComponent(
                direction.x * speed,
                direction.y * speed
            ));

            // ================= Rotation =================
            float angle = Mathf.Atan2(direction.y, direction.x);
            world.AddComponent(entity, new RotationComponent(angle));

            // ================= Projectile =================
            float lifetime = battle.projectile.range / speed;
            world.AddComponent(entity, new ProjectileComponent(
                speed: speed,
                pierce: 1,
                lifetime: lifetime
            ));

            // ================= Damage =================
            world.AddComponent(entity, new DamageSourceComponent
            {
                damage = battle.baseStats.damage * level,
                knockBack = battle.baseStats.knockback
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
        }
    }
}
