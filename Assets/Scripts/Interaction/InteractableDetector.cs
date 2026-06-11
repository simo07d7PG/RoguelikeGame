using RoguelikeGame.Grid;
using UnityEngine;

namespace RoguelikeGame.Interaction
{
    public class InteractableDetector : MonoBehaviour
    {
        [SerializeField] private float detectRadius = 0.6f;
        [SerializeField] private LayerMask interactableMask = ~0;
        [SerializeField] private float forwardOffset = 0.4f;
        [SerializeField] private bool useGridDetection = true;
        [SerializeField] private GridFacingResolver gridFacingResolver;

        private void Awake()
        {
            if (gridFacingResolver == null)
            {
                gridFacingResolver = GetComponent<GridFacingResolver>();
            }
        }

        public bool TryGetFacingCell(Vector2 facingDirection, out GridCoordinate cell)
        {
            cell = default;

            if (gridFacingResolver == null)
            {
                return false;
            }

            return gridFacingResolver.TryGetFacingCell(transform.position, facingDirection, out cell);
        }

        public bool TryGetInteractable(Vector2 facingDirection, out IInteractable interactable)
        {
            interactable = null;

            if (facingDirection.sqrMagnitude < 0.01f)
            {
                return false;
            }

            if (useGridDetection && gridFacingResolver != null
                && gridFacingResolver.TryGetFacingCell(transform.position, facingDirection, out GridCoordinate cell))
            {
                Vector2 cellCenter = gridFacingResolver.GetCellCenterWorld(cell);
                if (TryFindClosestInteractable(cellCenter, out interactable))
                {
                    return true;
                }
            }

            Vector2 origin = (Vector2)transform.position + facingDirection.normalized * forwardOffset;
            return TryFindClosestInteractable(origin, out interactable);
        }

        public bool TryGetCarryable(Vector2 facingDirection, out Items.ICarryable carryable)
        {
            carryable = null;

            if (facingDirection.sqrMagnitude < 0.01f)
            {
                return false;
            }

            if (useGridDetection && gridFacingResolver != null
                && gridFacingResolver.TryGetFacingCell(transform.position, facingDirection, out GridCoordinate cell))
            {
                Vector2 cellCenter = gridFacingResolver.GetCellCenterWorld(cell);
                if (TryFindClosestCarryable(cellCenter, out carryable))
                {
                    return true;
                }
            }

            Vector2 origin = (Vector2)transform.position + facingDirection.normalized * forwardOffset;
            return TryFindClosestCarryable(origin, out carryable);
        }

        private bool TryFindClosestInteractable(Vector2 origin, out IInteractable interactable)
        {
            interactable = null;
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, detectRadius, interactableMask);

            float closestDistance = float.MaxValue;
            for (int i = 0; i < hits.Length; i++)
            {
                if (IsPlayerCollider(hits[i].transform))
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

        private bool TryFindClosestCarryable(Vector2 origin, out Items.ICarryable carryable)
        {
            carryable = null;
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, detectRadius, interactableMask);

            float closestDistance = float.MaxValue;
            for (int i = 0; i < hits.Length; i++)
            {
                if (IsPlayerCollider(hits[i].transform))
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

        private bool IsPlayerCollider(Transform target)
        {
            return target == transform || target.IsChildOf(transform);
        }

        private void OnDrawGizmosSelected()
        {
            if (gridFacingResolver == null)
            {
                gridFacingResolver = GetComponent<GridFacingResolver>();
            }

            Gizmos.color = Color.yellow;

            if (Application.isPlaying && gridFacingResolver != null)
            {
                Vector2 facing = Vector2.down;
                if (gridFacingResolver.TryGetFacingCell(transform.position, facing, out GridCoordinate cell))
                {
                    Gizmos.DrawWireSphere(gridFacingResolver.GetCellCenterWorld(cell), detectRadius);
                    return;
                }
            }

            Vector2 origin = (Vector2)transform.position + Vector2.down * forwardOffset;
            Gizmos.DrawWireSphere(origin, detectRadius);
        }
    }
}