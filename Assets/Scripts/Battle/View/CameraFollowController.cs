using Cinemachine;
using UnityEngine;

namespace Battle.View
{
    namespace Battle.View
    {
        public class CameraFollowController
        {
            private readonly CinemachineVirtualCamera vcam;

            public CameraFollowController(CinemachineVirtualCamera vcam)
            {
                this.vcam = vcam;
            }

            public void SetTarget(Transform target)
            {
                if (target == null)
                    return;

                if (vcam.Follow != target)
                {
                    vcam.Follow = target;
                    vcam.LookAt = target;
                }
            }
        }
    }
}