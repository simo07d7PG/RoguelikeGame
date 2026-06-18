using UnityEngine;
using UnityEngine.InputSystem;

namespace FinalRogue
{
    public class CameraFollowController : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] Camera targetCamera;
        [SerializeField] GameInput gameInput;
        [SerializeField] float followSpeed = 8f;
        [SerializeField] float mouseOffsetStrength = 2f;
        [SerializeField] float mouseOffsetMax = 3f;
        [SerializeField] float zOffset = -10f;

        void Awake()
        {
            if (targetCamera == null)
                targetCamera = GetComponent<Camera>() ?? Camera.main;

            if (target == null)
                TryAutoWireTarget();
        }

        public void Configure(Transform followTarget, GameInput input)
        {
            target = followTarget;
            if (input != null)
                gameInput = input;

            if (targetCamera == null)
                targetCamera = GetComponent<Camera>() ?? Camera.main;

            SnapToTarget();
        }

        void TryAutoWireTarget()
        {
            PlayerEntity playerEntity = EntitySetupUtility.FindFirst<PlayerEntity>();
            if (playerEntity != null)
                target = playerEntity.transform;
            else
            {
                PlayerController playerController = EntitySetupUtility.FindFirst<PlayerController>();
                if (playerController != null)
                    target = playerController.transform;
            }

            if (target != null && gameInput == null)
                gameInput = target.GetComponent<GameInput>();
        }

        public void SnapToTarget()
        {
            if (target == null)
                return;

            Vector3 desired = target.position;
            desired.z = zOffset;
            transform.position = desired;
        }

        void LateUpdate()
        {
            if (target == null)
            {
                TryAutoWireTarget();
                if (target == null)
                    return;
            }

            Vector3 desired = target.position;
            desired.z = zOffset;

            if (targetCamera != null && gameInput != null && Mouse.current != null)
            {
                Vector2 screenPos = gameInput.GetMouseScreenPosition();
                Vector3 mouseWorld = targetCamera.ScreenToWorldPoint(screenPos);
                mouseWorld.z = target.position.z;

                Vector3 offset = mouseWorld - target.position;
                offset.z = 0f;
                offset = Vector3.ClampMagnitude(offset * mouseOffsetStrength, mouseOffsetMax);
                desired += offset;
            }

            transform.position = Vector3.Lerp(transform.position, desired, followSpeed * Time.deltaTime);
        }
    }
}