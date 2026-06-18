using Unity.Cinemachine;
using UnityEngine;

namespace FinalRogue
{
    [DefaultExecutionOrder(-180)]
    [DisallowMultipleComponent]
    public class CameraFollowBootstrap : MonoBehaviour
    {
        static CameraFollowBootstrap instance;

        CinemachineCamera cinemachineCamera;
        CameraFollowController legacyFollow;
        Transform player;

        public static CameraFollowBootstrap Instance => instance;

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
                return;
            }

            instance = this;
            TryBindPlayer();
        }

        void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }

        public void BindPlayer(Transform playerTransform)
        {
            player = playerTransform;
            ApplyBinding();
        }

        void TryBindPlayer()
        {
            if (player != null)
                return;

            PlayerEntity playerEntity = EntitySetupUtility.FindFirst<PlayerEntity>();
            if (playerEntity != null)
                player = playerEntity.transform;
            else
            {
                PlayerController playerController = EntitySetupUtility.FindFirst<PlayerController>();
                if (playerController != null)
                    player = playerController.transform;
            }

            ApplyBinding();
        }

        void ApplyBinding()
        {
            if (player == null)
                return;

            cinemachineCamera = EntitySetupUtility.FindFirst<CinemachineCamera>();
            if (cinemachineCamera != null)
            {
                cinemachineCamera.Follow = player;
                cinemachineCamera.Priority = 10;
                EnsureCinemachineFollow(cinemachineCamera);
                return;
            }

            legacyFollow = EntitySetupUtility.FindFirst<CameraFollowController>();
            if (legacyFollow == null && Camera.main != null)
                legacyFollow = Camera.main.gameObject.AddComponent<CameraFollowController>();

            if (legacyFollow != null)
                legacyFollow.Configure(player, player.GetComponent<GameInput>());
        }

        static void EnsureCinemachineFollow(CinemachineCamera camera)
        {
            if (camera == null)
                return;

            CinemachineFollow follow = camera.GetComponent<CinemachineFollow>();
            if (follow == null)
                follow = camera.gameObject.AddComponent<CinemachineFollow>();

            follow.FollowOffset = new Vector3(0f, 0f, -10f);
        }

        public void NotifyPlayerWarped(Vector3 previousWorldPosition)
        {
            if (player == null)
                return;

            Vector3 delta = player.position - previousWorldPosition;
            if (delta.sqrMagnitude < 0.0001f)
                return;

            cinemachineCamera?.OnTargetObjectWarped(player, delta);
            legacyFollow?.SnapToTarget();
        }
    }
}