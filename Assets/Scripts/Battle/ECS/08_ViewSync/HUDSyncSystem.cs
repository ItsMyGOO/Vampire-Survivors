using ECS.Core;
using UI.Model;

namespace ECS.SyncSystems
{
    public class HUDSyncSystem : ISystem
    {
        private readonly int playerId;
        private readonly HUDViewModel vm;

        public HUDSyncSystem(int playerId, HUDViewModel vm)
        {
            this.playerId = playerId;
            this.vm = vm;
        }

        public void Update(World world, float deltaTime)
        {
            // var hp = world.GetComponent<HealthComponent>(playerId);
            // var level = world.GetComponent<LevelComponent>(playerId);
            // var exp = world.GetComponent<ExperienceComponent>(playerId);
            //
            // vm.SetHealthRatio(hp.Current / (float)hp.Max);
            // vm.SetLevel(level.Level);
            // vm.SetExpRatio(exp.Current / (float)exp.Required);
            //
            // vm.SetBattleTime(world.Time);
        }
    }

}