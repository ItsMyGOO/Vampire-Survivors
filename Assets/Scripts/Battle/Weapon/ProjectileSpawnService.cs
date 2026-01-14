using ConfigHandler;
using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    public static class ProjectileSpawnService
    {
        public static Vector2 Calculate(
            Vector2 baseDir,
            int index,
            int count,
            float spreadAngleDeg)
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
            int owner,
            PositionComponent ownerPos,
            Vector2 direction,
            WeaponFinalStats stats,
            WeaponConfig cfg)
        {
            int entity = world.CreateEntity();

            world.AddComponent(entity, new PositionComponent(ownerPos.x, ownerPos.y));

            world.AddComponent(entity, new VelocityComponent(
                direction.x * stats.projectile.speed,
                direction.y * stats.projectile.speed));

            float angle = Mathf.Atan2(direction.y, direction.x);
            world.AddComponent(entity, new RotationComponent(angle));

            float lifetime = stats.projectile.range / stats.projectile.speed;
            world.AddComponent(entity, new ProjectileComponent(
                stats.projectile.speed,
                1,
                lifetime));

            world.AddComponent(entity, new DamageSourceComponent
            {
                damage = stats.damage,
                knockBack = stats.knockback
            });

            world.AddComponent(entity, new ColliderComponent(0.3f));
            world.AddComponent(entity, new LifeTimeComponent(lifetime));

            // ================= View =================
            var view = cfg.view;
            world.AddComponent(entity, new SpriteKeyComponent
            {
                sheet = view.sprite.sheet,
                key = view.sprite.key
            });
        }
    }
}