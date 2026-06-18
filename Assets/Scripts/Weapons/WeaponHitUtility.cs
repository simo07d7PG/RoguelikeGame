using UnityEngine;

namespace FinalRogue
{
    public static class WeaponHitUtility
    {
        const float RayOriginOffset = 0.2f;

        public static bool TryFindEnemyHit(
            Vector2 origin,
            Vector2 direction,
            float range,
            Transform ownerRoot,
            LayerMask mask,
            out RaycastHit2D bestHit)
        {
            bestHit = default;
            if (direction.sqrMagnitude < 0.001f || range <= 0f)
                return false;

            direction.Normalize();
            Vector2 start = origin + direction * RayOriginOffset;
            float castDistance = Mathf.Max(0.01f, range - RayOriginOffset);

            RaycastHit2D[] hits = Physics2D.RaycastAll(start, direction, castDistance, mask);
            if (hits == null || hits.Length == 0)
                return false;

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider == null || ShouldIgnoreCollider(hit.collider, ownerRoot))
                    continue;

                if (!TryGetEnemyDamageable(hit.collider, out IDamageable damageable))
                    continue;

                bestHit = hit;
                return damageable != null;
            }

            return false;
        }

        static bool ShouldIgnoreCollider(Collider2D collider, Transform ownerRoot)
        {
            if (collider == null)
                return true;

            if (collider.isTrigger)
                return true;

            if (ownerRoot != null && (collider.transform == ownerRoot || collider.transform.IsChildOf(ownerRoot)))
                return true;

            Transform current = collider.transform;
            while (current != null)
            {
                if (current.name == "BoundaryWalls")
                    return true;
                current = current.parent;
            }

            return false;
        }

        static bool TryGetEnemyDamageable(Collider2D collider, out IDamageable damageable)
        {
            damageable = null;
            if (collider == null)
                return false;

            ITeamMember teamMember = collider.GetComponent<ITeamMember>();
            if (teamMember == null)
                teamMember = collider.GetComponentInParent<ITeamMember>();

            if (teamMember == null || teamMember.Team != DamageTeam.Enemy)
                return false;

            damageable = collider.GetComponent<IDamageable>();
            if (damageable == null)
                damageable = collider.GetComponentInParent<IDamageable>();

            return damageable != null;
        }
    }
}