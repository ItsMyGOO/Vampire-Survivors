using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    /// <summary>
    /// 武器发射系统
    /// 职责: 根据武器配置发射投射物
    /// </summary>
    public class WeaponFireSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            foreach (var (entity, weaponSlots) in world.GetComponents<WeaponSlotsComponent>())
            {
                if (!world.HasComponent<PositionComponent>(entity)) continue;

                var position = world.GetComponent<PositionComponent>(entity);

                foreach (var weapon in weaponSlots.weapons)
                {
                    weapon.cooldown -= deltaTime;

                    if (weapon.cooldown <= 0)
                    {
                        FireWeapon(world, entity, weapon, position);
                        weapon.cooldown = weapon.fire_rate;
                    }
                }
            }
        }

        private void FireWeapon(World world, int ownerId, WeaponSlotsComponent.WeaponData weapon,
            PositionComponent ownerPos)
        {
            // TODO: 这里可以调用 Lua 获取武器配置
            // 暂时使用硬编码示例

            if (weapon.weapon_type == "knife")
            {
                // 刀是轨道武器，在 WeaponSlots 初始化时创建
                // 这里不需要发射
                return;
            }

            // 创建投射物
            int projectileId = world.CreateEntity();

            // 找到最近的敌人
            int targetId = FindNearestEnemy(world, ownerPos);
            Vector2 direction = Vector2.right; // 默认方向

            if (targetId != -1)
            {
                var targetPos = world.GetComponent<PositionComponent>(targetId);
                float dx = targetPos.x - ownerPos.x;
                float dy = targetPos.y - ownerPos.y;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                if (dist > 0)
                {
                    direction = new Vector2(dx / dist, dy / dist);
                }
            }

            // 添加组件
            world.AddComponent(projectileId, new PositionComponent(ownerPos.x, ownerPos.y));
            world.AddComponent(projectileId, new VelocityComponent(direction.x * 15f, direction.y * 15f));
            world.AddComponent(projectileId, new ProjectileComponent(15f, 1, 5.0f));
            world.AddComponent(projectileId, new DamageSourceComponent(20f * weapon.level, ownerId));
            world.AddComponent(projectileId, new ColliderComponent(0.3f));
            world.AddComponent(projectileId, new SpriteKeyComponent("projectile"));
            world.AddComponent(projectileId, new LifeTimeComponent(5.0f));
        }

        private int FindNearestEnemy(World world, PositionComponent playerPos)
        {
            int nearest = -1;
            float minDist = float.MaxValue;

            foreach (var (entity, _) in world.GetComponents<EnemyTagComponent>())
            {
                if (!world.HasComponent<PositionComponent>(entity)) continue;

                var enemyPos = world.GetComponent<PositionComponent>(entity);
                float dx = enemyPos.x - playerPos.x;
                float dy = enemyPos.y - playerPos.y;
                float dist = dx * dx + dy * dy;

                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = entity;
                }
            }

            return nearest;
        }
    }
}