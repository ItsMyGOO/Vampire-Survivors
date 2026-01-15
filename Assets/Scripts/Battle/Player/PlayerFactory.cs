using ECS.Core;
using ECS;

namespace Battle.Player
{
    public static class PlayerFactory
    {
        public static int CreatePlayer(World world)
        {
            int id = world.CreateEntity();

            world.AddComponent(id, new PlayerTagComponent());
            world.AddComponent(id, new PositionComponent());
            world.AddComponent(id, new VelocityComponent { speed = 2 });
            world.AddComponent(id, new HealthComponent(100, 100, 1f));
            world.AddComponent(id, new ColliderComponent(0.5f));
            world.AddComponent(id, new PickupRangeComponent(1f));
            world.AddComponent(id, new MagnetComponent(5f, 10f));
            world.AddComponent(id, new SpriteKeyComponent());
            world.AddComponent(id, new CameraFollowComponent());
            world.AddComponent(id, new AnimationComponent
            {
                ClipSetName = "Player",
                DefaultAnim = "Idle"
            });

            world.AddComponent(id, new WeaponSlotsComponent
            {
                weapons =
                {
                    new WeaponSlotsComponent.WeaponData("ProjectileKnife",1,1),
                    new WeaponSlotsComponent.WeaponData("OrbitKnife",1,1)
                }
            });

            return id;
        }
    }
}