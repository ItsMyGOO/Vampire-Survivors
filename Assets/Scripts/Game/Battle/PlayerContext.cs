using ECS.Core;
using UniRx;

namespace Game.Battle
{
    public class PlayerContext
    {
        private static PlayerContext _instance;
        public static PlayerContext Instance => _instance ??= new PlayerContext();

        public readonly BoolReactiveProperty IsInitialized = new(false);

        public Exp Exp { get; private set; }
        public int PlayerEntity { get; set; } = -1;
        public World World { get; set; }

        public void Initialize(World world, int playerEntity)
        {
            Exp = new Exp();
            World = world;
            PlayerEntity = playerEntity;
            IsInitialized.Value = true;
        }
    }

    public class Exp
    {
        public IntReactiveProperty level;
        public FloatReactiveProperty current_exp;
        public FloatReactiveProperty exp_to_next_level;
        public float exp_multiplier; // 经验倍率

        public Exp()
        {
            level = new(1);
            current_exp = new(0f);
            exp_to_next_level = new(10);
            exp_multiplier = 1.0f;
        }
    }
}
