using ECS.Core;
using UniRx;

namespace Game.Battle
{
    public class PlayerContext
    {
        private static PlayerContext _instance;
        public static PlayerContext Instance => _instance ??= new PlayerContext();

        public readonly BoolReactiveProperty IsInitialized = new(false);
        public World World { get; private set; }
        public int PlayerEntityId { get; private set; }

        public void Initialize(World world, int playerEntityId)
        {
            World = world;
            PlayerEntityId = playerEntityId;
            IsInitialized.Value = true;
        }
    }
}