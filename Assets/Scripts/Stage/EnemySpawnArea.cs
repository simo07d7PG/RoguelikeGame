using UnityEngine;

namespace FinalRogue
{
    public class EnemySpawnArea : MonoBehaviour
    {
        [SerializeField] Transform areaStart;
        [SerializeField] Transform areaEnd;
        [SerializeField] float edgePadding = 0.6f;

        public void Configure(Transform start, Transform end)
        {
            areaStart = start;
            areaEnd = end;
        }

        public bool HasValidArea => areaStart != null && areaEnd != null;

        public Vector2 GetRandomLocalPosition()
        {
            if (!HasValidArea)
                return Vector2.zero;

            Vector2 start = areaStart.localPosition;
            Vector2 end = areaEnd.localPosition;
            float minX = Mathf.Min(start.x, end.x) + edgePadding;
            float maxX = Mathf.Max(start.x, end.x) - edgePadding;
            float minY = Mathf.Min(start.y, end.y) + edgePadding;
            float maxY = Mathf.Max(start.y, end.y) - edgePadding;

            if (maxX < minX)
                (minX, maxX) = (maxX, minX);
            if (maxY < minY)
                (minY, maxY) = (maxY, minY);

            return new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY));
        }

        public Vector3 GetRandomWorldPosition()
        {
            Vector2 local = GetRandomLocalPosition();
            return transform.TransformPoint(local);
        }
    }
}