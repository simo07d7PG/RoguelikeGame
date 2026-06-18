using System;
using UnityEngine;

namespace FinalRogue
{
    public class ShopManager : MonoBehaviour
    {
        [SerializeField] ShopCatalog catalog;

        public event Action ShopClosed;

        public void Configure(ShopCatalog shopCatalog)
        {
            if (shopCatalog != null)
                catalog = shopCatalog;

#if UNITY_EDITOR
            EntitySetupUtility.SetField(this, "catalog", catalog);
#endif
        }

        public bool TryBuyHeal(RunPersistentData runData, PlayerHealth playerHealth)
        {
            if (catalog == null || runData == null || playerHealth == null)
                return false;

            if (playerHealth.CurrentHealth >= playerHealth.MaxHealth)
                return false;

            if (!runData.SpendCoins(catalog.healCost))
                return false;

            playerHealth.Heal(catalog.healAmount);
            runData.currentHealth = playerHealth.CurrentHealth;
            return true;
        }

        public bool TryBuyWeapon(RunPersistentData runData, WeaponController weaponController, WeaponData weapon)
        {
            if (catalog == null || runData == null || weaponController == null || weapon == null)
                return false;

            if (!weapon.IsValidForCombat())
            {
                Debug.LogWarning($"{nameof(ShopManager)}: Weapon '{weapon.weaponName}' is missing combat configuration.");
                return false;
            }

            if (runData.currentWeapon == weapon)
                return false;

            if (!runData.SpendCoins(weapon.shopPrice))
                return false;

            GameManager.Instance?.EquipWeapon(weapon);
            return true;
        }

        public WeaponData[] GetWeaponOffers()
        {
            return catalog != null ? catalog.weapons : Array.Empty<WeaponData>();
        }

        public int GetHealCost() => catalog != null ? catalog.healCost : 0;
        public int GetHealAmount() => catalog != null ? catalog.healAmount : 0;

        public void CloseShop()
        {
            ShopClosed?.Invoke();
        }
    }
}