using System;
using UnityEngine;

namespace FinalRogue
{
    public class PlayerHealth : MonoBehaviour, IDamageable, ITeamMember
    {
        [SerializeField] int maxHealth = 100;

        public int CurrentHealth { get; private set; }
        public int MaxHealth => maxHealth;
        public bool IsAlive => CurrentHealth > 0;
        public DamageTeam Team => DamageTeam.Player;

        public event Action<int, int> HealthChanged;
        public event Action Died;

        void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void Configure(int health)
        {
            maxHealth = Mathf.Max(1, health);
            CurrentHealth = maxHealth;
        }

        public void Initialize(int current, int max)
        {
            maxHealth = Mathf.Max(1, max);
            CurrentHealth = Mathf.Clamp(current, 0, maxHealth);
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void TakeDamage(int amount)
        {
            if (!IsAlive || amount <= 0)
                return;

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            HealthChanged?.Invoke(CurrentHealth, maxHealth);

            if (!IsAlive)
                Died?.Invoke();
        }

        public void Heal(int amount)
        {
            if (!IsAlive || amount <= 0)
                return;

            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

    }
}