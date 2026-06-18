using System;
using UnityEngine;

namespace FinalRogue
{
    [Serializable]
    public struct EnemySpawnEntry
    {
        public EnemyData enemyData;
        public int count;
    }

    public class StageArea : MonoBehaviour
    {
        [SerializeField] Transform playerSpawnPoint;
        [SerializeField] ExitPoint exitPoint;
        [SerializeField] EnemySpawnEntry[] enemies;
        [SerializeField] EnemySpawnArea enemySpawnArea;
        [SerializeField] StageBounds stageBounds;

        public Transform PlayerSpawnPoint => playerSpawnPoint != null ? playerSpawnPoint : transform;
        public ExitPoint Exit => exitPoint;
        public EnemySpawnEntry[] Enemies => enemies;
        public EnemySpawnArea SpawnArea => enemySpawnArea;
        public StageBounds Bounds => stageBounds;

        void Awake()
        {
            if (stageBounds == null)
                stageBounds = GetComponent<StageBounds>();
            if (stageBounds == null)
                stageBounds = gameObject.AddComponent<StageBounds>();

            if (enemySpawnArea == null)
                enemySpawnArea = GetComponentInChildren<EnemySpawnArea>(true);
        }

        public void Configure(Transform spawn, ExitPoint exit, EnemySpawnEntry[] entries, EnemySpawnArea spawnArea = null)
        {
            playerSpawnPoint = spawn;
            exitPoint = exit;
            enemies = entries;
            if (spawnArea != null)
                enemySpawnArea = spawnArea;
        }

        public Vector3 GetRandomEnemySpawnPosition()
        {
            if (enemySpawnArea != null && enemySpawnArea.HasValidArea)
                return enemySpawnArea.GetRandomWorldPosition();

            if (Bounds != null && Bounds.HasBounds)
                return Bounds.GetRandomWorldPosition();

            return PlayerSpawnPoint.position;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
            if (exitPoint != null)
                exitPoint.SetActive(false);
        }
    }
}