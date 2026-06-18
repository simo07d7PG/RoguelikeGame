using System;
using System.Collections;
using UnityEngine;

namespace FinalRogue
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(EnemyHealth))]
    public class EnemyController : MonoBehaviour
    {
        const int SpawnBlinkCount = 5;

        [SerializeField] Transform firePoint;
        [SerializeField] int spawnBlinkCount = SpawnBlinkCount;
        [SerializeField] float spawnBlinkInterval = 0.14f;
        [SerializeField] float spawnFadeDuration = 0.55f;
        [SerializeField] float spawnMinAlpha = 0.12f;

        EnemyData data;
        EnemyHealth health;
        Rigidbody2D rb;
        CircleCollider2D circleCollider;
        Transform player;
        float fireTimer;
        float damageMultiplier = 1f;
        SpriteRenderer spriteRenderer;
        bool isSpawned;
        Coroutine spawnRoutine;
        Action<EnemyHealth> spawnCompleted;

        public bool IsSpawned => isSpawned;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<EnemyHealth>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            circleCollider = GetComponent<CircleCollider2D>();

            if (firePoint == null)
                firePoint = transform;

            health.Died += OnDied;
        }

        public void Configure(Transform fire)
        {
            firePoint = fire != null ? fire : transform;
            EntitySetupUtility.SetField(this, "firePoint", firePoint);
        }

        void OnDestroy()
        {
            if (health != null)
                health.Died -= OnDied;
        }

        public void Initialize(EnemyData enemyData, Transform playerTarget, float enemyDamageMultiplier)
        {
            data = enemyData;
            player = playerTarget;
            damageMultiplier = enemyDamageMultiplier;
            isSpawned = false;

            health.Initialize(data);
            if (spriteRenderer != null && data.sprite != null)
                spriteRenderer.sprite = data.sprite;
        }

        public void BeginSpawnPresentation(Action<EnemyHealth> onSpawnCompleted)
        {
            spawnCompleted = onSpawnCompleted;
            SetSpawnPresentationActive(true);

            if (spawnRoutine != null)
                StopCoroutine(spawnRoutine);

            spawnRoutine = StartCoroutine(SpawnPresentationRoutine());
        }

        void SetSpawnPresentationActive(bool presenting)
        {
            health.SetCombatEnabled(!presenting);
            if (circleCollider != null)
                circleCollider.enabled = !presenting;
            if (rb != null)
                rb.simulated = !presenting;

            if (presenting && rb != null)
                rb.linearVelocity = Vector2.zero;
        }

        IEnumerator SpawnPresentationRoutine()
        {
            float baseAlpha = spawnMinAlpha;
            SetAlpha(baseAlpha);

            for (int i = 0; i < spawnBlinkCount; i++)
            {
                float progress = (i + 1f) / spawnBlinkCount;
                float dimAlpha = Mathf.Lerp(spawnMinAlpha, 0.35f, progress);
                float brightAlpha = Mathf.Lerp(0.35f, 0.85f, progress);

                SetAlpha(dimAlpha);
                yield return new WaitForSeconds(spawnBlinkInterval * 0.5f);

                SetAlpha(brightAlpha);
                yield return new WaitForSeconds(spawnBlinkInterval * 0.5f);

                baseAlpha = Mathf.Lerp(baseAlpha, brightAlpha, 0.45f);
            }

            float fadeStart = spriteRenderer != null ? spriteRenderer.color.a : baseAlpha;
            float elapsed = 0f;
            while (elapsed < spawnFadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / spawnFadeDuration;
                SetAlpha(Mathf.Lerp(fadeStart, 1f, t));
                yield return null;
            }

            SetAlpha(1f);
            CompleteSpawnPresentation();
        }

        void CompleteSpawnPresentation()
        {
            spawnRoutine = null;
            isSpawned = true;
            SetSpawnPresentationActive(false);
            spawnCompleted?.Invoke(health);
            spawnCompleted = null;
        }

        void SetAlpha(float alpha)
        {
            if (spriteRenderer == null)
                return;

            Color color = spriteRenderer.color;
            color.a = Mathf.Clamp01(alpha);
            spriteRenderer.color = color;
        }

        void Update()
        {
            if (!isSpawned)
                return;

            if (GameManager.Instance != null && GameManager.Instance.State != GameState.Playing)
                return;

            if (!health.IsAlive || player == null || data == null)
                return;

            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                TryFire();
                fireTimer = data.fireInterval;
            }
        }

        void FixedUpdate()
        {
            if (!isSpawned)
                return;

            if (GameManager.Instance != null && GameManager.Instance.State != GameState.Playing)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            if (!health.IsAlive || player == null || data == null)
                return;

            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * data.moveSpeed;
        }

        void TryFire()
        {
            if (data.projectilePrefab == null || player == null)
                return;

            Vector2 direction = (player.position - firePoint.position).normalized;
            if (direction.sqrMagnitude < 0.001f)
                return;

            int damage = Mathf.RoundToInt(data.damage * damageMultiplier);
            var projectile = Instantiate(data.projectilePrefab, firePoint.position, Quaternion.identity);
            if (projectile.TryGetComponent(out Projectile projectileLogic))
                projectileLogic.Launch(direction, data.projectileSpeed, damage, DamageTeam.Enemy);
        }

        void OnDied(EnemyHealth _)
        {
            StageManager.Instance?.UnregisterEnemy(health);
            GameManager.Instance?.OnEnemyKilled(health.GetCoinReward());
            Destroy(gameObject);
        }
    }
}