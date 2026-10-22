using System.Collections.Generic;
using UnityEngine;

namespace Spawning
{
    [CreateAssetMenu(menuName = "Spawning/Wave Config")]
    public class WaveConfig : ScriptableObject
    {
        public List<GameObject> enabledEnemies;

        [Min(1)]
        public int differentEnemiesPerWave;
        public int waveCount;
        public int waveBaseWeight;
        [Range(1,5)]
        public float weightScaleFactorPerWave;
        public int waveDuration;

        public int prepareTime;
        public int timeBetweenWaves;
    }
}