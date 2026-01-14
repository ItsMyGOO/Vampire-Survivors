using UnityEngine;
using ECS.Core;
using Game.Battle;

namespace Battle
{
    public class ECSGameDebugController : MonoBehaviour
    {
        private World world;
        private int playerId;

        public ECSGameDebugController Initialize(World w, int pid)
        {
            world = w;
            playerId = pid;
            return this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
                ExpSystem.Instance.AddExpForTest(20);

            if (Input.GetKeyDown(KeyCode.F1))
                world.DebugPrint();
        }
    }
}