using System.Collections.Generic;
using UnityEngine;

namespace FinalRogue
{
    public class StageManager : MonoBehaviour
    {
        public static StageManager Instance { get; private set; }

        [SerializeField] StageArea[] stages;
        [SerializeField] GameObject enemyPrefab;
        [SerializeField] Transform player;

        readonly HashSet<EnemyHealth> aliveEnemies = new();
        StageArea currentStage;
        int currentStageIndex = -1;
        int pendingEnemySpawns;

        public int CurrentStageIndex => currentStageIndex;
        public int StageCount => stages != null ? stages.Length : 0;
        public StageBounds CurrentBounds => currentStage != null ? currentStage.Bounds : null;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void Configure(StageArea[] stageList, GameObject enemy, Transform playerTransform)
        {
            if (stageList != null && stageList.Length > 0)
                stages = stageList;
            if (enemy != null)
                enemyPrefab = enemy;
            if (playerTransform != null)
                player = playerTransform;

#if UNITY_EDITOR
            EntitySetupUtility.SetFieldArray(this, "stages", stages);
            EntitySetupUtility.SetField(this, "enemyPrefab", enemyPrefab);
            EntitySetupUtility.SetField(this, "player", player);
#endif
        }

        public void LoadStage(int stageIndex, RunPersistentData runData)
        {
            if (stages == null || stages.Length == 0)
            {
                Debug.LogError($"{nameof(StageManager)}: No stages assigned.");
                return;
            }

            stageIndex = Mathf.Clamp(stageIndex, 0, stages.Length - 1);
            currentStageIndex = stageIndex;
            aliveEnemies.Clear();
            pendingEnemySpawns = 0;

            currentStage = stages[stageIndex];

            for (int i = 0; i < stages.Length; i++)
                stages[i].SetActive(i == stageIndex);

            currentStage.Bounds?.RefreshBounds();
            MovePlayerToStage(currentStage);

            ClearStageEnemies(currentStage);
            currentStage.Exit?.SetActive(false);
            SpawnEnemies(runData);
        }

        void MovePlayerToStage(StageArea stage)
        {
            if (player == null || stage == null)
                return;

            Transform spawn = stage.PlayerSpawnPoint;
            Vector3 previousPosition = player.position;

            player.SetParent(stage.transform, false);
            player.localRotation = Quaternion.identity;
            player.localPosition = spawn.localPosition;
            player.gameObject.SetActive(true);

            if (player.TryGetComponent(out Rigidbody2D rb))
            {
                rb.position = spawn.position;
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.WakeUp();
            }

            if (player.TryGetComponent(out GameInput gameInput))
                gameInput.EnsureEnabled();

            if (player.TryGetComponent(out PlayerController playerController))
                playerController.ResetMovementState();

            CameraFollowBootstrap.Instance?.NotifyPlayerWarped(previousPosition);
        }

        void ClearStageEnemies(StageArea stage)
        {
            if (stage == null)
                return;

            var enemiesInStage = stage.GetComponentsInChildren<EnemyController>(true);
            foreach (var enemy in enemiesInStage)
                Destroy(enemy.gameObject);
        }

        void SpawnEnemies(RunPersistentData runData)
        {
            if (currentStage == null || enemyPrefab == null)
                return;

            float damageMultiplier = runData != null ? runData.EnemyDamageMultiplier : 1f;
            var entries = currentStage.Enemies;
            if (entries == null)
                return;

            pendingEnemySpawns = CountPlannedSpawns(entries);

            foreach (var entry in entries)
            {
                if (entry.enemyData == null || entry.count <= 0)
                    continue;

                for (int i = 0; i < entry.count; i++)
                    SpawnEnemy(entry.enemyData, damageMultiplier);
            }

            CheckStageClear();
        }

        static int CountPlannedSpawns(EnemySpawnEntry[] entries)
        {
            int total = 0;
            if (entries == null)
                return total;

            foreach (var entry in entries)
            {
                if (entry.enemyData != null && entry.count > 0)
                    total += entry.count;
            }

            return total;
        }

        void SpawnEnemy(EnemyData enemyData, float damageMultiplier)
        {
            Vector3 spawnPos = currentStage.GetRandomEnemySpawnPosition();
            var enemyObject = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, currentStage.transform);
            if (!enemyObject.TryGetComponent(out EnemyController enemy))
            {
                pendingEnemySpawns = Mathf.Max(0, pendingEnemySpawns - 1);
                Destroy(enemyObject);
                return;
            }

            enemy.Initialize(enemyData, player, damageMultiplier);
            enemy.BeginSpawnPresentation(OnEnemySpawnCompleted);
        }

        void OnEnemySpawnCompleted(EnemyHealth enemy)
        {
            pendingEnemySpawns = Mathf.Max(0, pendingEnemySpawns - 1);
            RegisterEnemy(enemy);
            CheckStageClear();
        }

        void RegisterEnemy(EnemyHealth enemy)
        {
            aliveEnemies.Add(enemy);
        }

        public void UnregisterEnemy(EnemyHealth enemy)
        {
            aliveEnemies.Remove(enemy);
            CheckStageClear();
        }

        void CheckStageClear()
        {
            if (aliveEnemies.Count == 0 && pendingEnemySpawns == 0)
                currentStage?.Exit?.SetActive(true);
        }

        public void OnPlayerReachedExit()
        {
            GameManager.Instance?.OnStageCleared(currentStageIndex);
        }
    }
}