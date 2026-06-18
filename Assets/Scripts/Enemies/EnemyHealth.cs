using System;
using UnityEngine;

namespace FinalRogue
{
    public class EnemyHealth : MonoBehaviour, IDamageable, ITeamMember
    {
        int coinReward;

        public int CurrentHealth { get; private set; }
        public int MaxHealth { get; private set; }
        public bool IsAlive => CurrentHealth > 0;
        public bool CanTakeDamage { get; private set; } = true;
        public DamageTeam Team => DamageTeam.Enemy;

        public event Action<EnemyHealth> Died;

        public void Initialize(EnemyData enemyData)
        {
            MaxHealth = enemyData.maxHealth;
            CurrentHealth = MaxHealth;
            coinReward = enemyData.coinReward;
        }

        public void SetCombatEnabled(bool enabled) => CanTakeDamage = enabled;

        public void TakeDamage(int amount)
        {
            if (!IsAlive || !CanTakeDamage || amount <= 0)
                return;

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            if (!IsAlive)
                Died?.Invoke(this);
        }

        public void Heal(int amount)
        {
            if (!IsAlive || amount <= 0)
                return;

            CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
        }

        public int GetCoinReward() => coinReward;
    }
}