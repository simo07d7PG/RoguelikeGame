using UnityEngine;
using UnityEngine.Tilemaps;

namespace FinalRogue
{
    [DisallowMultipleComponent]
    public class StageBounds : MonoBehaviour
    {
        const string WallsName = "BoundaryWalls";
        const float WallThickness = 0.5f;

        [SerializeField] bool useTilemapBounds = true;
        [SerializeField] Vector2 manualMin = new(-8f, -4f);
        [SerializeField] Vector2 manualMax = new(8f, 4f);
        [SerializeField] float innerPadding = 0.55f;
        [SerializeField] bool createWallColliders = true;

        Bounds2 worldPlayBounds;
        bool hasBounds;

        public bool HasBounds => hasBounds;
        public Bounds2 WorldPlayBounds => worldPlayBounds;

        void Awake() => RefreshBounds();

        public void RefreshBounds()
        {
            if (useTilemapBounds && TryGetTilemapBounds(transform, out Bounds2 tilemapBounds))
                worldPlayBounds = Inset(tilemapBounds, innerPadding);
            else
                worldPlayBounds = Inset(TransformManualBounds(), innerPadding);

            hasBounds = worldPlayBounds.IsValid;
            if (hasBounds && createWallColliders)
                RebuildWalls(worldPlayBounds);
        }

        public Vector2 ClampPosition(Vector2 worldPosition)
        {
            if (!hasBounds)
                return worldPosition;

            return new Vector2(
                Mathf.Clamp(worldPosition.x, worldPlayBounds.Min.x, worldPlayBounds.Max.x),
                Mathf.Clamp(worldPosition.y, worldPlayBounds.Min.y, worldPlayBounds.Max.y));
        }

        public Vector3 GetRandomWorldPosition()
        {
            if (!hasBounds)
                return transform.position;

            return new Vector3(
                Random.Range(worldPlayBounds.Min.x, worldPlayBounds.Max.x),
                Random.Range(worldPlayBounds.Min.y, worldPlayBounds.Max.y),
                transform.position.z);
        }

        static bool TryGetTilemapBounds(Transform stageRoot, out Bounds2 bounds)
        {
            if (TryCollectTilemapBounds(stageRoot.GetComponentsInChildren<Tilemap>(true), out bounds))
                return true;

            return TryCollectTilemapBounds(
                Object.FindObjectsByType<Tilemap>(FindObjectsInactive.Include, FindObjectsSortMode.None),
                out bounds);
        }

        static bool TryCollectTilemapBounds(Tilemap[] tilemaps, out Bounds2 bounds)
        {
            bounds = default;
            bool found = false;

            foreach (Tilemap tilemap in tilemaps)
            {
                if (tilemap == null || tilemap.cellBounds.size.x == 0 || tilemap.cellBounds.size.y == 0)
                    continue;

                tilemap.CompressBounds();
                Bounds local = tilemap.localBounds;
                Vector3 worldMin = tilemap.transform.TransformPoint(local.min);
                Vector3 worldMax = tilemap.transform.TransformPoint(local.max);

                var tileBounds = new Bounds2(
                    new Vector2(Mathf.Min(worldMin.x, worldMax.x), Mathf.Min(worldMin.y, worldMax.y)),
                    new Vector2(Mathf.Max(worldMin.x, worldMax.x), Mathf.Max(worldMin.y, worldMax.y)));

                bounds = found ? bounds.Encapsulate(tileBounds) : tileBounds;
                found = true;
            }

            return found;
        }

        Bounds2 TransformManualBounds()
        {
            Vector3 worldMin = transform.TransformPoint(manualMin);
            Vector3 worldMax = transform.TransformPoint(manualMax);
            return new Bounds2(worldMin, worldMax);
        }

        static Bounds2 Inset(Bounds2 bounds, float padding)
        {
            return new Bounds2(
                new Vector2(bounds.Min.x + padding, bounds.Min.y + padding),
                new Vector2(bounds.Max.x - padding, bounds.Max.y - padding));
        }

        void RebuildWalls(Bounds2 playBounds)
        {
            Transform wallsRoot = transform.Find(WallsName);
            if (wallsRoot == null)
            {
                var wallsObject = new GameObject(WallsName);
                wallsRoot = wallsObject.transform;
                wallsRoot.SetParent(transform, false);
            }

            for (int i = wallsRoot.childCount - 1; i >= 0; i--)
                Destroy(wallsRoot.GetChild(i).gameObject);

            float width = playBounds.Width + WallThickness * 2f;
            float height = playBounds.Height + WallThickness * 2f;
            Vector2 center = playBounds.Center;

            CreateWall(wallsRoot, "Top", new Vector2(center.x, playBounds.Max.y + WallThickness * 0.5f), new Vector2(width, WallThickness));
            CreateWall(wallsRoot, "Bottom", new Vector2(center.x, playBounds.Min.y - WallThickness * 0.5f), new Vector2(width, WallThickness));
            CreateWall(wallsRoot, "Left", new Vector2(playBounds.Min.x - WallThickness * 0.5f, center.y), new Vector2(WallThickness, height));
            CreateWall(wallsRoot, "Right", new Vector2(playBounds.Max.x + WallThickness * 0.5f, center.y), new Vector2(WallThickness, height));
        }

        void CreateWall(Transform parent, string wallName, Vector2 worldCenter, Vector2 size)
        {
            var wallObject = new GameObject(wallName);
            wallObject.transform.SetParent(parent, false);
            wallObject.transform.position = new Vector3(worldCenter.x, worldCenter.y, 0f);

            var collider = wallObject.AddComponent<BoxCollider2D>();
            collider.size = size;
            collider.isTrigger = false;
        }

        public readonly struct Bounds2
        {
            public readonly Vector2 Min;
            public readonly Vector2 Max;

            public Bounds2(Vector2 min, Vector2 max)
            {
                Min = new Vector2(Mathf.Min(min.x, max.x), Mathf.Min(min.y, max.y));
                Max = new Vector2(Mathf.Max(min.x, max.x), Mathf.Max(min.y, max.y));
            }

            public bool IsValid => Max.x > Min.x && Max.y > Min.y;
            public Vector2 Center => (Min + Max) * 0.5f;
            public float Width => Max.x - Min.x;
            public float Height => Max.y - Min.y;

            public Bounds2 Encapsulate(Bounds2 other)
            {
                return new Bounds2(
                    Vector2.Min(Min, other.Min),
                    Vector2.Max(Max, other.Max));
            }
        }
    }
}