using UnityEngine;

namespace FinalRogue
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] GameInput gameInput;

        Rigidbody2D rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (gameInput == null)
                gameInput = GetComponent<GameInput>();
        }

        public void Configure(GameInput input, float speed)
        {
            gameInput = input;
            moveSpeed = speed;
            EntitySetupUtility.SetField(this, "gameInput", input);
            EntitySetupUtility.SetField(this, "moveSpeed", speed);
        }

        public void ResetMovementState()
        {
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.WakeUp();
            }
        }

        void FixedUpdate()
        {
            if (GameManager.Instance != null && GameManager.Instance.State != GameState.Playing)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            Vector2 input = gameInput != null ? gameInput.Move : Vector2.zero;
            if (input.sqrMagnitude > 1f)
                input.Normalize();

            rb.linearVelocity = input * moveSpeed;
            ClampToStageBounds();
        }

        void ClampToStageBounds()
        {
            StageBounds bounds = StageManager.Instance != null ? StageManager.Instance.CurrentBounds : null;
            if (bounds == null || !bounds.HasBounds)
                return;

            Vector2 clamped = bounds.ClampPosition(rb.position);
            if ((clamped - rb.position).sqrMagnitude <= 0.0001f)
                return;

            rb.position = clamped;
            Vector2 velocity = rb.linearVelocity;
            StageBounds.Bounds2 playBounds = bounds.WorldPlayBounds;
            if (Mathf.Approximately(clamped.x, playBounds.Min.x) && velocity.x < 0f)
                velocity.x = 0f;
            if (Mathf.Approximately(clamped.x, playBounds.Max.x) && velocity.x > 0f)
                velocity.x = 0f;
            if (Mathf.Approximately(clamped.y, playBounds.Min.y) && velocity.y < 0f)
                velocity.y = 0f;
            if (Mathf.Approximately(clamped.y, playBounds.Max.y) && velocity.y > 0f)
                velocity.y = 0f;
            rb.linearVelocity = velocity;
        }
    }
}