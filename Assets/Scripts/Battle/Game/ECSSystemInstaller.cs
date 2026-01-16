using ECS.Core;
using ECS.Systems;

namespace Battle
{
    public static class ECSSystemInstaller
    {
        public static void Install(World world)
        {
            world.RegisterSystem(new PlayerInputSystem());
            world.RegisterSystem(new MagnetSystem());
            world.RegisterSystem(new PickupSystem());
            world.RegisterSystem(new EnemySpawnSystem());
            world.RegisterSystem(new AIMovementSystem());
            world.RegisterSystem(new MovementSystem());
            world.RegisterSystem(new WeaponFireSystem());
            world.RegisterSystem(new OrbitSystem());
            world.RegisterSystem(new AttackHitSystem());
            world.RegisterSystem(new KnockBackSystem());
            world.RegisterSystem(new EnemyDeathSystem());
            world.RegisterSystem(new PlayerAnimationSystem());
            world.RegisterSystem(new AnimationCommandSystem());
            world.RegisterSystem(new AnimationSystem());
        }
    }
}