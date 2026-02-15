using ECS.Core;
using ECS;
using Battle.Weapon;
using Battle.Upgrade;

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

            // 玩家属性组件 - 重构版
            var baseAttr = BaseAttributeComponent.CreateDefault();
            baseAttr.moveSpeed = 2f;  // 与 VelocityComponent 保持一致
            baseAttr.maxHealth = 100f; // 与 HealthComponent 保持一致
            world.AddComponent(id, baseAttr);
            
            // 修改器集合
            var modifierCollection = AttributeModifierCollectionComponent.Create();
            world.AddComponent(id, modifierCollection);
            
            // 被动状态
            var passiveState = PassiveUpgradeStateComponent.Create();
            world.AddComponent(id, passiveState);
            
            var weaponStats = new WeaponRuntimeStatsComponent();
            weaponStats.AddWeapon("ProjectileKnife", 1);
            weaponStats.AddWeapon("OrbitKnife", 1);
            world.AddComponent(id, weaponStats);

            return id;
        }
    }
}
