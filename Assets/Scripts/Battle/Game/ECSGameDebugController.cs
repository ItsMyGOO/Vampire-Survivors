using Battle.Player;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// 战斗调试控制器
    /// PlayerContext 由 ECSGameManager 在 Start 后显式注入
    /// </summary>
    public class ECSGameDebugController : MonoBehaviour
    {
        private PlayerContext _playerContext;

        public void Init(PlayerContext playerContext)
        {
            _playerContext = playerContext;
        }

        private void Update()
        {
            if (_playerContext == null) return;

            if (Input.GetKeyDown(KeyCode.E))
                _playerContext.ExpSystem?.AddExpForTest(20);

            if (Input.GetKeyDown(KeyCode.F1))
            {
                _playerContext.World.DebugPrint();
                _playerContext.ExpSystem?.PrintStatus();
            }
        }
    }
}