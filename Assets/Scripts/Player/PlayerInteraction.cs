using RoguelikeGame.Core;
using RoguelikeGame.Interaction;
using RoguelikeGame.Items;
using UnityEngine;

namespace RoguelikeGame.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private float dropDistance = GameConstants.DefaultDropDistance;

        private InteractableDetector _detector;
        private PlayerItemStack _stack;
        private PlayerFacing _facing;

        private void Awake()
        {
            _detector = GetComponent<InteractableDetector>();
            _stack = GetComponent<PlayerItemStack>();
            _facing = GetComponent<PlayerFacing>();
        }

        public void TryInteract()
        {
            Vector2 facingDirection = _facing.LastDirection;

            if (_stack.IsEmpty)
            {
                TryPickup(facingDirection);
                return;
            }

            if (_detector.TryGetInteractable(facingDirection, out IInteractable interactable))
            {
                ICarryable topItem = _stack.Peek();
                if (topItem != null && interactable.CanPlace(topItem))
                {
                    ICarryable placedItem = _stack.TryPop();
                    interactable.Place(placedItem);
                    return;
                }
            }

            DropTopItem(facingDirection);
        }

        private void TryPickup(Vector2 facingDirection)
        {
            if (_detector.TryGetInteractable(facingDirection, out IInteractable interactable)
                && interactable.CanPickup(null))
            {
                ICarryable takenItem = interactable.Take();
                if (takenItem != null)
                {
                    _stack.TryPush(takenItem);
                    return;
                }
            }

            if (_detector.TryGetCarryable(facingDirection, out ICarryable carryable))
            {
                _stack.TryPush(carryable);
            }
        }

        private void DropTopItem(Vector2 facingDirection)
        {
            ICarryable droppedItem = _stack.TryPop();
            if (droppedItem == null)
            {
                return;
            }

            Vector2 dropPosition = (Vector2)transform.position + facingDirection.normalized * dropDistance;
            droppedItem.OnDropped(dropPosition);
        }
    }
}