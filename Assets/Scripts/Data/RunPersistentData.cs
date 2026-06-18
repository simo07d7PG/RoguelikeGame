using System;
using UnityEngine;

namespace FinalRogue
{
    [Serializable]
    public class RunPersistentData
    {
        public int currentHealth = 100;
        public int maxHealth = 100;

        [SerializeField] int coins;

        public WeaponData currentWeapon;
        public int currentAmmo = -1;

        public int loopCount;
        public float damageMultiplierPerLoop = 0.25f;

        public int Coins
        {
            get => coins;
            set => coins = Mathf.Max(0, value);
        }

        public float EnemyDamageMultiplier => 1f + loopCount * damageMultiplierPerLoop;

        public void AddCoins(int amount)
        {
            if (amount > 0)
                coins += amount;
        }

        public bool SpendCoins(int amount)
        {
            if (amount <= 0 || coins < amount)
                return false;

            coins -= amount;
            return true;
        }
    }
}