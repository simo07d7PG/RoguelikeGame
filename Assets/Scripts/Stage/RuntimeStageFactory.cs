using UnityEngine;

namespace FinalRogue
{
    public static class RuntimeStageFactory
    {
        const string RootName = "RuntimeStages";

        static readonly Vector3 SpawnLocalPosition = Vector3.zero;
        static readonly Vector3 ExitLocalPosition = new(0f, -6f, 0f);
        static readonly Vector2 EnemySpawnAreaStart = new(-6f, -3f);
        static readonly Vector2 EnemySpawnAreaEnd = new(6f, 3f);

        static readonly int[] EnemyCountsPerStage = { 2, 3, 4 };

        public static StageArea[] GetOrCreate(EnemyData enemyData, Vector3 origin)
        {
            Transform root = FindRoot();
            if (root != null)
            {
                StageArea[] existing = CollectStages(root);
                if (HasValidMarkers(existing))
                    return existing;

                Object.Destroy(root.gameObject);
            }

            root = new GameObject(RootName).transform;
            var stages = new StageArea[EnemyCountsPerStage.Length];
            for (int i = 0; i < EnemyCountsPerStage.Length; i++)
                stages[i] = CreateStage(root, $"Stage{i + 1}", i, enemyData, origin);

            return stages;
        }

        static Transform FindRoot()
        {
            GameObject existing = GameObject.Find(RootName);
            return existing != null ? existing.transform : null;
        }

        static StageArea[] CollectStages(Transform root)
        {
            var stages = root.GetComponentsInChildren<StageArea>(true);
            System.Array.Sort(stages, (a, b) => string.Compare(a.name, b.name, System.StringComparison.Ordinal));
            return stages;
        }

        static bool HasValidMarkers(StageArea[] stages)
        {
            if (stages == null || stages.Length == 0)
                return false;

            foreach (StageArea stage in stages)
            {
                Transform spawn = stage.PlayerSpawnPoint;
                if (spawn == null || spawn.GetComponent<PlayerSpawnPoint>() == null)
                    return false;

                EnemySpawnArea enemySpawnArea = stage.SpawnArea;
                if (enemySpawnArea == null || !enemySpawnArea.HasValidArea)
                    return false;
            }

            return true;
        }

        static StageArea CreateStage(Transform parent, string stageName, int layoutIndex, EnemyData enemyData, Vector3 origin)
        {
            var stageObject = new GameObject(stageName);
            stageObject.transform.SetParent(parent, false);
            stageObject.transform.position = origin;

            PlayerSpawnPoint spawn = CreatePlayerSpawn(stageObject.transform, SpawnLocalPosition);
            ExitPoint exit = CreateExit(stageObject.transform, ExitLocalPosition);
            EnemySpawnArea enemySpawnArea = CreateEnemySpawnArea(stageObject.transform, EnemySpawnAreaStart, EnemySpawnAreaEnd);
            var entries = BuildEnemyEntries(enemyData, EnemyCountsPerStage[layoutIndex]);

            var bounds = stageObject.AddComponent<StageBounds>();
            bounds.RefreshBounds();

            var stageArea = stageObject.AddComponent<StageArea>();
            stageArea.Configure(spawn.transform, exit, entries, enemySpawnArea);
            stageObject.SetActive(false);
            return stageArea;
        }

        static PlayerSpawnPoint CreatePlayerSpawn(Transform parent, Vector3 localPosition)
        {
            var spawnObject = new GameObject("PlayerSpawn");
            spawnObject.transform.SetParent(parent, false);
            spawnObject.transform.localPosition = localPosition;

            var marker = spawnObject.AddComponent<PlayerSpawnPoint>();
            marker.Setup(new Vector2(1.5f, 1.5f), new Color(0.3f, 0.7f, 1f, 0.85f));
            return marker;
        }

        static ExitPoint CreateExit(Transform parent, Vector3 localPosition)
        {
            var exitObject = new GameObject("Exit");
            exitObject.transform.SetParent(parent, false);
            exitObject.transform.localPosition = localPosition;
            exitObject.transform.localScale = new Vector3(3f, 3f, 1f);

            var spriteRenderer = exitObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = StageMarkerUtility.GetSquareSprite();
            spriteRenderer.color = new Color(0.2f, 1f, 0.45f, 0.4f);
            spriteRenderer.sortingOrder = 2;

            var collider = exitObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = Vector2.one;

            return exitObject.AddComponent<ExitPoint>();
        }

        static EnemySpawnArea CreateEnemySpawnArea(Transform parent, Vector2 startLocal, Vector2 endLocal)
        {
            var areaObject = new GameObject("EnemySpawnArea");
            areaObject.transform.SetParent(parent, false);

            Transform start = CreateEnemySpawnMarker(areaObject.transform, "SpawnAreaStart", startLocal,
                new Color(1f, 0.55f, 0.2f, 0.65f));
            Transform end = CreateEnemySpawnMarker(areaObject.transform, "SpawnAreaEnd", endLocal,
                new Color(1f, 0.25f, 0.2f, 0.65f));

            var area = areaObject.AddComponent<EnemySpawnArea>();
            area.Configure(start, end);
            return area;
        }

        static Transform CreateEnemySpawnMarker(Transform parent, string markerName, Vector2 localPosition, Color color)
        {
            var markerObject = new GameObject(markerName);
            markerObject.transform.SetParent(parent, false);
            markerObject.transform.localPosition = localPosition;

            var marker = markerObject.AddComponent<EnemySpawnAreaMarker>();
            marker.Setup(new Vector2(0.8f, 0.8f), color);
            return markerObject.transform;
        }

        static EnemySpawnEntry[] BuildEnemyEntries(EnemyData enemyData, int count)
        {
            if (enemyData == null || count <= 0)
                return System.Array.Empty<EnemySpawnEntry>();

            return new[]
            {
                new EnemySpawnEntry
                {
                    enemyData = enemyData,
                    count = count
                }
            };
        }
    }
}