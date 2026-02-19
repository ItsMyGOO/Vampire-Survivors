using ECS.Core;
using ECS.Systems;

namespace Battle
{
    public static class ECSSystemInstaller
    {
        public static void Install(World world)
        {
            // --- 输入 ---
            world.RegisterSystem(new PlayerInputSystem());

            // --- 升级/属性流水线（意图 → 计算 → 同步） ---
            world.RegisterSystem(new PassiveUpgradeSystem());
            world.RegisterSystem(new AttributeCalculationSystem());
            world.RegisterSystem(new AttributeSyncSystem());

            // --- 拾取 / 磁铁 ---
            world.RegisterSystem(new MagnetSystem());
            world.RegisterSystem(new PickupSystem());

            // --- 生命周期 ---
            world.RegisterSystem(new EnemySpawnSystem());

            // --- 移动 ---
            world.RegisterSystem(new AIMovementSystem());
            world.RegisterSystem(new MovementSystem());

            // --- 战斗 ---
            world.RegisterSystem(new WeaponFireSystem());
            world.RegisterSystem(new OrbitSystem());
            world.RegisterSystem(new AttackHitSystem());
            world.RegisterSystem(new KnockBackSystem());
            world.RegisterSystem(new EnemyDeathSystem());

            // --- 表现 ---
            world.RegisterSystem(new PlayerAnimationSystem());
            world.RegisterSystem(new AnimationCommandSystem());
            world.RegisterSystem(new AnimationSystem());
        }
    }
}