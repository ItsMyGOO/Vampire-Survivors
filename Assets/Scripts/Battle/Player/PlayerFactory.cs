using Battle.Weapon;
using ConfigHandler;
using ECS;
using ECS.Core;

namespace Battle.Player
{
    public static class PlayerFactory
    {
public static int CreatePlayer(World world, CharacterDef characterDef = null)
        {
            int id = world.CreateEntity();

            world.AddComponent(id, new PlayerTagComponent());
            world.AddComponent(id, new PositionComponent());

            // 从角色配置读取移速，fallback 到默认值
            float moveSpeed = characterDef?.moveSpeed ?? 2f;
            world.AddComponent(id, new VelocityComponent { speed = moveSpeed });

            // 从角色配置读取 HP，fallback 到默认值
            float maxHp = characterDef?.maxHealth ?? 100f;
            float hpRegen = characterDef?.healthRegen ?? 0f;
            world.AddComponent(id, new HealthComponent(maxHp, maxHp, hpRegen));

            world.AddComponent(id, new ColliderComponent(0.5f));

            float pickupRange = characterDef?.pickupRange ?? 1f;
            world.AddComponent(id, new PickupRangeComponent(pickupRange));
            world.AddComponent(id, new MagnetComponent(5f));
            world.AddComponent(id, new SpriteKeyComponent());
            world.AddComponent(id, new CameraFollowComponent());

            string clipSetId = characterDef?.clipSetId ?? "Player";
            world.AddComponent(id, new AnimationComponent
            {
                ClipSetName = clipSetId,
                DefaultAnim = "Idle"
            });

            // 玩家属性组件 — 从角色配置覆盖基础属性
            var baseAttr = BaseAttributeComponent.CreateDefault();
            baseAttr.moveSpeed      = moveSpeed;
            baseAttr.maxHealth      = maxHp;
            baseAttr.healthRegen    = hpRegen;
            baseAttr.armor          = characterDef?.armor          ?? 0f;
            baseAttr.attackDamage   = characterDef?.attackDamage   ?? 0f;
            baseAttr.attackSpeed    = characterDef?.attackSpeed    ?? 1f;
            baseAttr.criticalChance = characterDef?.criticalChance ?? 0f;
            baseAttr.criticalDamage = characterDef?.criticalDamage ?? 1.5f;
            baseAttr.pickupRange    = pickupRange;
            baseAttr.expGain        = characterDef?.expGain        ?? 1f;
            baseAttr.cooldownReduction = characterDef?.cooldownReduction ?? 0f;
            baseAttr.damageReduction   = characterDef?.damageReduction   ?? 0f;
            baseAttr.dodgeChance       = characterDef?.dodgeChance       ?? 0f;
            world.AddComponent(id, baseAttr);

            var modifierCollection = AttributeModifierCollectionComponent.Create();
            world.AddComponent(id, modifierCollection);

            var passiveState = PassiveUpgradeStateComponent.Create();
            world.AddComponent(id, passiveState);

            world.AddComponent(id, new AttributeDirtyComponent());

            // 从角色配置加载初始武器，fallback 到默认配置
            var weaponStats = new WeaponRuntimeStatsComponent();
            if (characterDef?.startingWeapons != null && characterDef.startingWeapons.Count > 0)
            {
                foreach (var w in characterDef.startingWeapons)
                    weaponStats.AddWeapon(w.weaponId, w.level);
            }
            else
            {
                weaponStats.AddWeapon("ProjectileKnife");
                weaponStats.AddWeapon("OrbitKnife");
            }
            world.AddComponent(id, weaponStats);

            return id;
        }
    }
}
