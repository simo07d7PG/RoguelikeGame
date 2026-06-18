using UnityEngine;

namespace FinalRogue
{
    [CreateAssetMenu(fileName = "ShopCatalog", menuName = "FinalRogue/Shop Catalog")]
    public class ShopCatalog : ScriptableObject
    {
        public int healCost = 50;
        public int healAmount = 30;
        public WeaponData[] weapons;
    }
}