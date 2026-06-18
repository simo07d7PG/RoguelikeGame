using UnityEngine;

namespace FinalRogue
{
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float lifetime = 5f;

        Vector2 direction;
        float speed;
        int damage;
        DamageTeam ownerTeam;
        float lifeTimer;

        public void Launch(Vector2 dir, float spd, int dmg, DamageTeam team)
        {
            direction = dir.normalized;
            speed = spd;
            damage = dmg;
            ownerTeam = team;
            lifeTimer = lifetime;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }

        void Update()
        {
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0f)
                Destroy(gameObject);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Projectile>() != null)
                return;

            var teamMember = other.GetComponent<ITeamMember>();
            if (teamMember == null || teamMember.Team == ownerTeam)
                return;

            if (other.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}