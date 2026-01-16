using Battle.Player;
using Battle.Upgrade;
using ECS.Core;
using UnityEngine;

namespace Battle
{
    public class ECSGameDebugController : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
                PlayerContext.Instance.ExpSystem.AddExpForTest(20);

            if (Input.GetKeyDown(KeyCode.F1))
            {
                PlayerContext.Instance.World.DebugPrint();
                PlayerContext.Instance.ExpSystem.PrintStatus();
                PlayerContext.Instance.UpgradeState.PrintStatus();
            }
        }
    }
}