using UnityEngine;

namespace RoguelikeGame.Interaction
{
    public class InteractableDetector : MonoBehaviour
    {
        [SerializeField] private float detectRadius = 0.6f;
        [SerializeField] private LayerMask interactableMask = ~0;
        [SerializeField] private float forwardOffset = 0.4f;

        public bool TryGetInteractable(Vector2 facingDirection, out IInteractable interactable)
        {
            interactable = null;

            if (facingDirection.sqrMagnitude < 0.01f)
            {
                return false;
            }

            Vector2 origin = (Vector2)transform.position + facingDirection.normalized * forwardOffset;
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, detectRadius, interactableMask);

            float closestDistance = float.MaxValue;
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform == transform || hits[i].transform.IsChildOf(transform))
                {
                    continue;
                }

                if (!hits[i].TryGetComponent(out IInteractable candidate))
                {
                    continue;
                }

                float distance = Vector2.Distance(origin, hits[i].transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    interactable = candidate;
                }
            }

            return interactable != null;
        }

        public bool TryGetCarryable(Vector2 facingDirection, out Items.ICarryable carryable)
        {
            carryable = null;

            if (facingDirection.sqrMagnitude < 0.01f)
            {
                return false;
            }

            Vector2 origin = (Vector2)transform.position + facingDirection.normalized * forwardOffset;
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, detectRadius, interactableMask);

            float closestDistance = float.MaxValue;
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform == transform || hits[i].transform.IsChildOf(transform))
                {
                    continue;
                }

                if (!hits[i].TryGetComponent(out Items.ICarryable candidate) || candidate.IsCarried)
                {
                    continue;
                }

                float distance = Vector2.Distance(origin, hits[i].transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    carryable = candidate;
                }
            }

            return carryable != null;
        }

        private void OnDrawGizmosSelected()
        {
            Vector2 facing = Application.isPlaying ? Vector2.down : Vector2.down;
            Vector2 origin = (Vector2)transform.position + facing * forwardOffset;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(origin, detectRadius);
        }
    }
}