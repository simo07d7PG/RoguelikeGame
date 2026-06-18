using UnityEngine;

namespace FinalRogue
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "FinalRogue/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public string enemyName = "Slime";
        public int maxHealth = 30;
        public int damage = 5;
        public Sprite sprite;
        public float fireInterval = 2f;
        public float moveSpeed = 2f;
        public float projectileSpeed = 6f;
        public GameObject projectilePrefab;
        public int coinReward = 10;
    }
}