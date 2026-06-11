using RoguelikeGame.Utilities;
using UnityEngine;

namespace RoguelikeGame.Player
{
    public class PlayerFacing : MonoBehaviour
    {
        [SerializeField] private Vector2 defaultDirection = Direction8.Down;

        public Vector2 LastDirection { get; private set; }

        private void Awake()
        {
            LastDirection = defaultDirection.sqrMagnitude > 0.01f
                ? defaultDirection.normalized
                : Direction8.Down;
        }

        public void UpdateFacing(Vector2 moveInput)
        {
            if (moveInput.sqrMagnitude > 0.01f)
            {
                LastDirection = moveInput.normalized;
            }
        }
    }
}