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
                PlayerContext.Instance.World.DebugPrint();
        }

        public void PrintStatus()
        {
            var upgradeState = PlayerContext.Instance.UpgradeState;
            Debug.Log($"  拥有武器数: {upgradeState.weapons.Count}");
            foreach (var weapon in upgradeState.weapons)
            {
                Debug.Log($"    - {weapon.Key}: Lv.{weapon.Value}");
            }
        }
    }
}