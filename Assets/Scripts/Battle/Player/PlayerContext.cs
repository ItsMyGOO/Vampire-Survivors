using ECS.Core;
using UniRx;
using Battle;

namespace Game.Battle
{
    public class PlayerContext
    {
        private static PlayerContext _instance;
        public static PlayerContext Instance => _instance ??= new PlayerContext();

        public readonly BoolReactiveProperty IsInitialized = new(false);

        public ExpData ExpData { get; private set; }
        public int PlayerEntity { get; private set; } = -1;
        public World World { get; private set; }

        public PlayerUpgradeState UpgradeState { get; private set; }
        public WeaponUpgradeManager WeaponUpgradeManager { get; set; }

        public void Initialize(World world, int playerEntity)
        {
            World = world;
            PlayerEntity = playerEntity;
            UpgradeState = new PlayerUpgradeState();
        }

        public void BindExpData(ExpData expData)
        {
            ExpData = expData;
            IsInitialized.Value = true;
        }
    }
}