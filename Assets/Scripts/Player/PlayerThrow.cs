using RoguelikeGame.Core;
using UnityEngine;

namespace RoguelikeGame.Player
{
    public class PlayerThrow : MonoBehaviour
    {
        [SerializeField] private float throwSpeed = GameConstants.DefaultThrowSpeed;
        [SerializeField] private float throwGravityScale = GameConstants.DefaultThrowGravityScale;
        [SerializeField] private LineRenderer lineRenderer;

        private PlayerItemStack _stack;
        private PlayerFacing _facing;
        private bool _wasAiming;

        private void Awake()
        {
            _stack = GetComponent<PlayerItemStack>();
            _facing = GetComponent<PlayerFacing>();
            SetTrajectoryVisible(false);
        }

        public void HandleThrowInput(bool throwHeld, bool throwReleased)
        {
            if (throwHeld && !_stack.IsEmpty)
            {
                _wasAiming = true;
                UpdateTrajectoryPreview();
                SetTrajectoryVisible(true);
                return;
            }

            SetTrajectoryVisible(false);

            if (throwReleased && _wasAiming && !_stack.IsEmpty)
            {
                ExecuteThrow();
            }

            if (_stack.IsEmpty)
            {
                _wasAiming = false;
            }
        }

        private void ExecuteThrow()
        {
            Items.ICarryable thrownItem = _stack.TryPop();
            if (thrownItem == null)
            {
                _wasAiming = false;
                return;
            }

            Vector2 direction = _facing.LastDirection.normalized;
            Vector2 velocity = direction * throwSpeed;
            thrownItem.OnThrown(velocity);
            _wasAiming = false;
        }

        private void UpdateTrajectoryPreview()
        {
            if (lineRenderer == null)
            {
                return;
            }

            Vector2 origin = transform.position;
            Vector2 direction = _facing.LastDirection.normalized;
            Vector2 gravity = Physics2D.gravity * throwGravityScale;

            lineRenderer.positionCount = GameConstants.ThrowTrajectoryPointCount;

            for (int i = 0; i < GameConstants.ThrowTrajectoryPointCount; i++)
            {
                float time = i * GameConstants.ThrowTrajectoryTimeStep;
                Vector2 position = origin
                    + direction * throwSpeed * time
                    + 0.5f * gravity * (time * time);

                lineRenderer.SetPosition(i, position);
            }
        }

        private void SetTrajectoryVisible(bool visible)
        {
            if (lineRenderer == null)
            {
                return;
            }

            lineRenderer.enabled = visible;
        }
    }
}