using UnityEngine;


namespace Enemies
{
    public enum EnemyTier
    {
        Tier1,
        Tier2,
        Tier3,
        Tier4
    }
    public enum EnemyType
    {
        Leichti,
        Schweri,
        Gruppi
    }

    public class EnemyClassifier : MonoBehaviour
    {
        public EnemyTier tier;
        public EnemyType type;
        [Range(1,100)]
        public int spawnChance = 100;

        public bool willSpawn()
        {
            return Random.Range(0, 100) <= spawnChance;
        }
        
        public int GetWeight()
        {
            int tierBaseWeight = tier switch
            {
                EnemyTier.Tier1 => 1,
                EnemyTier.Tier2 => 2,
                EnemyTier.Tier3 => 4,
                EnemyTier.Tier4 => 8,
                _ => 1,
            };

            float typeMultiplier = type switch
            {
                EnemyType.Leichti => 2f,
                EnemyType.Gruppi => 3f,
                EnemyType.Schweri => 3f,
                _ => 1f
            };

            return Mathf.RoundToInt(tierBaseWeight * typeMultiplier);
        }
    }

}