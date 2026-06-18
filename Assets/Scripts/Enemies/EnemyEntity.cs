using UnityEngine;

namespace FinalRogue
{
    [DefaultExecutionOrder(-200)]
    [AddComponentMenu("FinalRogue/Enemy Entity")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(EnemyHealth))]
    [RequireComponent(typeof(EnemyController))]
    public class EnemyEntity : MonoBehaviour
    {
        void Reset() => SetupEntity();
        void Awake() => SetupEntity();

        void SetupEntity()
        {
            Rigidbody2D rb = EntitySetupUtility.EnsureComponent<Rigidbody2D>(gameObject);
            CircleCollider2D collider = EntitySetupUtility.EnsureComponent<CircleCollider2D>(gameObject);
            EntitySetupUtility.EnsureComponent<SpriteRenderer>(gameObject);
            EntitySetupUtility.EnsureComponent<EnemyHealth>(gameObject);
            EnemyController controller = EntitySetupUtility.EnsureComponent<EnemyController>(gameObject);

            EntitySetupUtility.ConfigureRigidbody2D(rb);
            EntitySetupUtility.ConfigureCircleCollider2D(collider, Vector2.zero, 0.5f);

            Transform firePoint = EntitySetupUtility.EnsureChild(transform, "FirePoint");
            firePoint.localPosition = new Vector3(0f, 0.35f, 0f);
            controller.Configure(firePoint);
        }
    }
}