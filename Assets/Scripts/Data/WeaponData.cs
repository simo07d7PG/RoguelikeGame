using UnityEngine;

namespace FinalRogue
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "FinalRogue/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        public string weaponName = "Pistol";
        public int damage = 10;
        public int magazineSize = 12;
        public float fireRate = 5f;
        public AttackType attackType = AttackType.Projectile;
        public float range = 10f;
        public float projectileSpeed = 12f;
        public GameObject projectilePrefab;
        public int shopPrice = 100;

        public int EffectiveMagazineSize
        {
            get
            {
                if (magazineSize > 0)
                    return magazineSize;

                return attackType == AttackType.Raycast ? 5 : 12;
            }
        }

        public bool IsValidForCombat()
        {
            if (attackType == AttackType.Projectile)
                return projectilePrefab != null;

            return range > 0f && damage > 0;
        }
    }
}