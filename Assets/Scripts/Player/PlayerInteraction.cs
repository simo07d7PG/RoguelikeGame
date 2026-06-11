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

            if (!_stack.IsFull && TryPickup(facingDirection))
            {
                return;
            }

            if (!_stack.IsEmpty && TryPlace(facingDirection))
            {
                return;
            }

            if (!_stack.IsEmpty && !HasPickupableNearby(facingDirection))
            {
                DropTopItem(facingDirection);
            }
        }

        private bool TryPickup(Vector2 facingDirection)
        {
            if (TryPickupFromInteractable(facingDirection))
            {
                return true;
            }

            return TryPickupCarryable(facingDirection);
        }

        private bool TryPickupFromInteractable(Vector2 facingDirection)
        {
            if (!_detector.TryGetInteractable(facingDirection, out IInteractable interactable)
                || !interactable.CanPickup(null))
            {
                return false;
            }

            ICarryable takenItem = interactable.Take();
            return takenItem != null && _stack.TryPush(takenItem);
        }

        private bool TryPickupCarryable(Vector2 facingDirection)
        {
            if (!_detector.TryGetCarryable(facingDirection, out ICarryable carryable))
            {
                return false;
            }

            return _stack.TryPush(carryable);
        }

        private bool TryPlace(Vector2 facingDirection)
        {
            if (!_detector.TryGetInteractable(facingDirection, out IInteractable interactable))
            {
                return false;
            }

            ICarryable topItem = _stack.Peek();
            if (topItem == null || !interactable.CanPlace(topItem))
            {
                return false;
            }

            ICarryable placedItem = _stack.TryPop();
            interactable.Place(placedItem);
            return true;
        }

        private bool HasPickupableNearby(Vector2 facingDirection)
        {
            if (_detector.TryGetCarryable(facingDirection, out _))
            {
                return true;
            }

            if (_detector.TryGetInteractable(facingDirection, out IInteractable interactable)
                && interactable.CanPickup(null))
            {
                return true;
            }

            return false;
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