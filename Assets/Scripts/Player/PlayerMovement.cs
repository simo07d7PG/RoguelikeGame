using RoguelikeGame.Core;
using RoguelikeGame.Utilities;
using UnityEngine;

namespace RoguelikeGame.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = GameConstants.DefaultMoveSpeed;

        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void ApplyMovement(Vector2 moveInput)
        {
            if (moveInput.sqrMagnitude < 0.01f)
            {
                _rigidbody.linearVelocity = Vector2.zero;
                return;
            }

            float speed = moveSpeed;
            if (Direction8.IsDiagonal(moveInput))
            {
                speed *= 0.70710678f;
            }

            _rigidbody.linearVelocity = moveInput * speed;
        }
    }
}