using Battle.Player;
using Cinemachine;
using UnityEngine;
using UI.Core;
using UI.Model;
using UI.Panel;

namespace Battle
{
    public class ECSGameManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera vCam;

        private BattleWorldContext context;

        private void Start()
        {
            context = BattleGameBuilder.Build(vCam);
            UIManager.Instance.ShowPanel<BattleHUDPanel>();
        }

        private void Update()
        {
            if (context == null) return;

            context.World.Update(Time.deltaTime);
            context.RenderSyncSystem.Update(context.World);
        }

        private void OnDestroy()
        {
            context?.Dispose();
            PlayerContext.Clear();
            ViewModelRegistry.Unregister<HUDViewModel>();
        }
    }
}