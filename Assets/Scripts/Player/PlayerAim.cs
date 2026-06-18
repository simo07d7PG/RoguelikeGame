using UnityEngine;

namespace FinalRogue
{
    public class PlayerAim : MonoBehaviour
    {
        [SerializeField] Camera targetCamera;
        [SerializeField] GameInput gameInput;
        [SerializeField] Transform aimOrigin;
        [SerializeField] Transform aimVisual;
        [SerializeField] bool rotateVisual = true;

        Vector2 aimDirection = Vector2.right;

        void Awake()
        {
            if (targetCamera == null)
                targetCamera = Camera.main;
            if (gameInput == null)
                gameInput = GetComponent<GameInput>();
            if (aimOrigin == null)
                aimOrigin = transform;
        }

        public void Configure(GameInput input, Transform origin, Transform visual, Camera camera)
        {
            gameInput = input;
            aimOrigin = origin != null ? origin : transform;
            aimVisual = visual != null && visual != aimOrigin ? visual : null;
            if (camera != null)
                targetCamera = camera;

            EntitySetupUtility.SetField(this, "gameInput", input);
            EntitySetupUtility.SetField(this, "aimOrigin", aimOrigin);
            EntitySetupUtility.SetField(this, "aimVisual", aimVisual);
            if (targetCamera != null)
                EntitySetupUtility.SetField(this, "targetCamera", targetCamera);
        }

        void Update()
        {
            if (targetCamera == null || gameInput == null || aimOrigin == null)
                return;

            Vector2 screenPos = gameInput.GetMouseScreenPosition();
            Vector3 worldPos = targetCamera.ScreenToWorldPoint(screenPos);
            worldPos.z = aimOrigin.position.z;

            Vector2 direction = ((Vector2)worldPos - (Vector2)aimOrigin.position).normalized;
            if (direction.sqrMagnitude < 0.001f)
                return;

            aimDirection = direction;

            if (!rotateVisual || aimVisual == null)
                return;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            aimVisual.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }

        public Vector2 AimDirection => aimDirection;
    }
}