// ============================================
// 文件: WeaponFireSystem.cs - 完整版本
// 使用 WeaponConfigDB 加载武器配置
// ============================================

using ECS.Core;
using UnityEngine;
using ConfigHandler;

namespace ECS.Systems
{
    /// <summary>
    /// 武器发射系统 - 完整实现
    /// 支持两种武器类型:
    /// 1. Projectile (投射型): 周期性发射投射物，如飞刀、子弹
    /// 2. Orbit (轨道型): 生成一次围绕玩家旋转的武器，如环绕飞刀
    /// </summary>
    public class WeaponFireSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            // 遍历所有有武器槽的实体（通常是玩家）
            foreach (var (entity, weaponSlots) in world.GetComponents<WeaponSlotsComponent>())
            {
                if (!world.HasComponent<PositionComponent>(entity)) 
                    continue;

                var ownerPos = world.GetComponent<PositionComponent>(entity);

                // 遍历每个武器槽
                foreach (var weapon in weaponSlots.weapons)
                {
                    // 从配置数据库获取武器定义
                    if (!WeaponConfigDB.Instance.TryGetWeapon(weapon.weapon_type, out var weaponDef))
                    {
                        Debug.LogWarning($"未找到武器定义: {weapon.weapon_type}");
                        continue;
                    }

                    // 根据武器类型处理
                    if (weaponDef.Type == WeaponType.Projectile)
                    {
                        HandleProjectileWeapon(world, entity, weapon, weaponDef, ownerPos, deltaTime);
                    }
                    else if (weaponDef.Type == WeaponType.Orbit)
                    {
                        HandleOrbitWeapon(world, entity, weapon, weaponDef, ownerPos);
                    }
                }
            }
        }

        /// <summary>
        /// 处理投射型武器
        /// </summary>
        private void HandleProjectileWeapon(World world, int ownerId, 
            WeaponSlotsComponent.WeaponData weapon, WeaponDef weaponDef, 
            PositionComponent ownerPos, float deltaTime)
        {
            // 更新冷却计时器
            weapon.cooldown -= deltaTime;

            if (weapon.cooldown > 0)
                return;

            // 重置冷却
            weapon.cooldown = weaponDef.Interval;

            // 选择目标（最近的敌人）
            int targetId = FindNearestEnemy(world, ownerPos);

            // 计算发射方向
            Vector2 direction = CalculateDirection(world, ownerPos, targetId);

            // 发射投射物
            int count = Mathf.RoundToInt(weaponDef.BaseCount * weapon.level);
            for (int i = 0; i < count; i++)
            {
                CreateProjectile(world, ownerId, ownerPos, direction, weaponDef, weapon.level);
            }
        }

        /// <summary>
        /// 处理轨道型武器
        /// </summary>
        private void HandleOrbitWeapon(World world, int ownerId, 
            WeaponSlotsComponent.WeaponData weapon, WeaponDef weaponDef, 
            PositionComponent ownerPos)
        {
            // 轨道武器只生成一次
            if (weapon.orbitSpawned)
                return;

            weapon.orbitSpawned = true;

            // 生成轨道武器
            int count = Mathf.RoundToInt(weaponDef.BaseCount * weapon.level);
            float angleStep = 360f / count;

            for (int i = 0; i < count; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                CreateOrbitWeapon(world, ownerId, angle, weaponDef, weapon.level);
            }
        }

        /// <summary>
        /// 创建投射物实体
        /// </summary>
        private void CreateProjectile(World world, int ownerId, PositionComponent ownerPos, 
            Vector2 direction, WeaponDef weaponDef, int level)
        {
            int projectileId = world.CreateEntity();

            // 位置组件
            world.AddComponent(projectileId, new PositionComponent(ownerPos.x, ownerPos.y));

            // 速度组件
            float speed = weaponDef.BaseSpeed;
            world.AddComponent(projectileId, new VelocityComponent(
                direction.x * speed, 
                direction.y * speed
            ));

            // 旋转组件（让投射物朝向移动方向）
            float angle = Mathf.Atan2(direction.y, direction.x) ;
            world.AddComponent(projectileId, new RotationComponent(angle));
 
            // 投射物组件
            world.AddComponent(projectileId, new ProjectileComponent(
                speed: speed,
                pierce: 1,  // 默认穿透1个敌人
                lifetime: weaponDef.Range / speed  // 根据射程计算生命周期
            ));

            // 伤害来源组件
            float damage = weaponDef.BaseDamage * level;
            float knockBack = weaponDef.Knockback;
            world.AddComponent(projectileId, new DamageSourceComponent()
            {
                damage = damage,
                knockBack = knockBack
            });

            // 碰撞体组件
            world.AddComponent(projectileId, new ColliderComponent(0.3f));

            // 精灵组件
            world.AddComponent(projectileId, new SpriteKeyComponent 
            { 
                sheet = weaponDef.Sheet,
                key = weaponDef.Key 
            });

            // 生命周期组件
            world.AddComponent(projectileId, new LifeTimeComponent(weaponDef.Range / speed));
        }

        /// <summary>
        /// 创建轨道武器实体
        /// </summary>
        private void CreateOrbitWeapon(World world, int ownerId, float startAngle, 
            WeaponDef weaponDef, int level)
        {
            int orbitId = world.CreateEntity();

            // 位置组件（初始位置在玩家身上，之后由 OrbitSystem 更新）
            var ownerPos = world.GetComponent<PositionComponent>(ownerId);
            world.AddComponent(orbitId, new PositionComponent(ownerPos.x, ownerPos.y));

            // 轨道组件
            world.AddComponent(orbitId, new OrbitComponent
            {
                centerEntity = ownerId,
                radius = weaponDef.BaseRadius * Mathf.Sqrt(level), // 半径随等级增长
                angularSpeed = weaponDef.OrbitSpeed,
                currentAngle = startAngle
            });

            // 伤害来源组件
            float damage = weaponDef.BaseDamage * level;
            world.AddComponent(orbitId, new DamageSourceComponent()
            {
                damage = damage,
                knockBack = weaponDef.Knockback
            });

            // 碰撞体组件
            world.AddComponent(orbitId, new ColliderComponent(0.4f));

            // 精灵组件
            world.AddComponent(orbitId, new SpriteKeyComponent 
            { 
                sheet = weaponDef.Sheet,
                key = weaponDef.Key 
            });

            // 旋转组件（让武器旋转）
            world.AddComponent(orbitId, new RotationComponent(startAngle * Mathf.Rad2Deg));
        }

        /// <summary>
        /// 查找最近的敌人
        /// </summary>
        private int FindNearestEnemy(World world, PositionComponent playerPos)
        {
            int nearestEnemy = -1;
            float minDistSq = float.MaxValue;

            foreach (var (entity, enemyTag) in world.GetComponents<EnemyTagComponent>())
            {
                if (!world.HasComponent<PositionComponent>(entity)) 
                    continue;

                var enemyPos = world.GetComponent<PositionComponent>(entity);
                float dx = enemyPos.x - playerPos.x;
                float dy = enemyPos.y - playerPos.y;
                float distSq = dx * dx + dy * dy;

                if (distSq < minDistSq)
                {
                    minDistSq = distSq;
                    nearestEnemy = entity;
                }
            }

            return nearestEnemy;
        }

        /// <summary>
        /// 计算发射方向
        /// </summary>
        private Vector2 CalculateDirection(World world, PositionComponent ownerPos, int targetId)
        {
            // 如果没有目标，默认向右发射
            if (targetId == -1)
                return Vector2.right;

            // 计算朝向目标的方向
            var targetPos = world.GetComponent<PositionComponent>(targetId);
            if (targetPos == null)
                return Vector2.right;

            float dx = targetPos.x - ownerPos.x;
            float dy = targetPos.y - ownerPos.y;
            float dist = Mathf.Sqrt(dx * dx + dy * dy);

            if (dist < 0.001f)
                return Vector2.right;

            return new Vector2(dx / dist, dy / dist);
        }
    }
}
