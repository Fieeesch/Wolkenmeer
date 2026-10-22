using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemies;
using Spawning;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = System.Random;

public class WaveSpawnLogic : MonoBehaviour
{
    private WaveConfig _waveConfig;

    public List<GameObject> targets;

    private List<KeyValuePair<GameObject, int>> _enemiesWithWeights = new List<KeyValuePair<GameObject, int>>();

    [Range(10, 90)] public int spawnArcDegrees;
    [Range(10, 200)] public int distance;
    [Range(-25, 0)] public int maxNegativeYOffset;
    [Range(0, 25)] public int maxPositivYOffset;

    [Range(1, 2)] public float playerWeightScaling = 1f;
    private float _difficulty = 1f;

    public Vector3 centerOfShip = new Vector3(7f, 0f, 0f);

    public ParticleSystem particleSystem;
    public LineRenderer lineRenderer;

    private Random _random = new Random();

    [SerializeField]
    private BreakProgress breakProgression;

    void Start()
    {
        _waveConfig = GameManager.Instance.DifficultyConfig; //Instantiate(waveConfig);

        targets.RemoveAll(item => item == null);
        _waveConfig.enabledEnemies.RemoveAll(item => item == null);

        GetEnemiesWithWeights();

        PrepareSpawnArea();

        _difficulty = 1f * (float)Math.Pow(playerWeightScaling,
            GameManager.Instance.GetComponent<PlayerInputManager>().playerCount - 1);

        StartCoroutine(PreparationPhase()); // Start the preparation coroutine
    }

    private void Update()
    {
        // PrepareSpawnArea(); 
    }

    private void PrepareSpawnArea()
    {
        lineRenderer.positionCount = spawnArcDegrees + 1;
        float maxAngle = (spawnArcDegrees / 2f) * Mathf.Deg2Rad;

        Quaternion rotation = Quaternion.Euler(0f, -90f, 0f);

        for (int i = 0; i <= spawnArcDegrees; i++)
        {
            float t = (float)i / spawnArcDegrees;
            float angle = -maxAngle + maxAngle * 2 * t;

            float x = centerOfShip.x + Mathf.Cos(angle) * distance;
            float z = centerOfShip.z + Mathf.Sin(angle) * distance;

            Vector3 pos = new Vector3(centerOfShip.x + x, centerOfShip.y, centerOfShip.z + z);

            pos = rotation * pos;

            lineRenderer.SetPosition(i, pos);

            if (i % 5 == 0)
            {
                // ParticleSystem ps = Instantiate(particleSystem, pos, Quaternion.identity);
                // ps.transform.SetParent(transform);
            }
        }
    }

    private void GetEnemiesWithWeights()
    {
        foreach (GameObject enemy in _waveConfig.enabledEnemies)
        {
            EnemyClassifier classifier = enemy.GetComponent<EnemyClassifier>();
            if (classifier != null)
            {
                _enemiesWithWeights.Add(new KeyValuePair<GameObject, int>(enemy, classifier.GetWeight()));
            }
            else
            {
                Debug.Log("No classifier found!");
            }
        }
    }

    IEnumerator PreparationPhase()
    {
        Debug.Log("Preparation phase started. Waiting for " + _waveConfig.prepareTime + " seconds...");
        breakProgression.StartProgress(_waveConfig.prepareTime);
        yield return new WaitForSeconds(_waveConfig.prepareTime); // Wait for the specified time
        Debug.Log("Preparation complete. Starting wave spawning...");

        StartCoroutine(WaveStarter());
    }

    IEnumerator WaveStarter()
    {
        float waveWeight = _waveConfig.waveBaseWeight * _difficulty;

        for (int i = 0; i < _waveConfig.waveCount; i++)
        {
            Debug.Log("Start of wave " + (i + 1));

            yield return StartCoroutine(WaveSpawner(waveWeight));
            waveWeight *= _waveConfig.weightScaleFactorPerWave;

            Debug.Log("Wave " + (i + 1) + " complete!");

            if (i < _waveConfig.waveCount - 1)
            {
                Debug.Log("Waiting for " + _waveConfig.timeBetweenWaves + " seconds until the next wave...");
                breakProgression.StartProgress(_waveConfig.timeBetweenWaves);
                yield return new WaitForSeconds(_waveConfig.timeBetweenWaves);
            }
        }

        Debug.Log("All waves completed!");

        GameManager.Instance.WinGame();
    }

    IEnumerator WaveSpawner(float waveWeight)
    {
        List<KeyValuePair<GameObject, int>> selection = GetEnemySelection();

        List<GameObject> aliveEnemies = new List<GameObject>();

        float currentWeightBalance = 0;
        float currentWaveWeightPerSecond = waveWeight / _waveConfig.waveDuration;

        KeyValuePair<GameObject, int> nextEnemy = GetNextEnemy(selection);

        for (int i = 0; i < _waveConfig.waveDuration; i++)
        {
            currentWeightBalance += currentWaveWeightPerSecond;

            while (currentWeightBalance >= nextEnemy.Value)
            {
                currentWeightBalance -= nextEnemy.Value;
                aliveEnemies.Add(SpawnEnemy(nextEnemy.Key));
                nextEnemy = GetNextEnemy(selection);
            }

            yield return new WaitForSeconds(1);
        }

        if (currentWeightBalance > 0)
        {
            aliveEnemies.Add(SpawnEnemy(nextEnemy.Key));
        }

        while (aliveEnemies.Count > 0)
        {
            aliveEnemies.RemoveAll(item => item == null);
            yield return new WaitForSeconds(1);
        }
    }

    private GameObject SpawnEnemy(GameObject enemy)
    {
        Vector3 pos = GetRandomPointOnLine();

        // pos.y = _random.Next(maxNegativeYOffset, maxPositivYOffset);

        GameObject target = targets[_random.Next(targets.Count())];

        GameObject enemyInstance = Instantiate(enemy, pos, Quaternion.identity);

        if (enemyInstance.TryGetComponent(out EnemyMovement enemyMovement) && enemyMovement.isActiveAndEnabled)
        {
            enemyMovement.Target = target.transform;
            enemyInstance.transform.LookAt(target.transform);
        }
        else if (enemyInstance.TryGetComponent(out EnemyMovementBezier bezierMovement) &&
                 bezierMovement.isActiveAndEnabled)
        {
            bezierMovement.InitializeMovement(pos, target.transform);
        }
        else
        {
            Debug.LogWarning("No Movement found on the spawned enemy!");
            Destroy(enemyInstance);
            return null;
        }

        return enemyInstance;
    }

    private Vector3 GetRandomPointOnLine()
    {
        int index1 = _random.Next(0, lineRenderer.positionCount - 1);
        int index2 = index1 + 1;

        Vector3 point1 = lineRenderer.GetPosition(index1);
        Vector3 point2 = lineRenderer.GetPosition(index2);

        float t = (float)_random.NextDouble();
        Vector3 spawnPoint = Vector3.Lerp(point1, point2, t);

        return spawnPoint;
    }

    private KeyValuePair<GameObject, int> GetNextEnemy(List<KeyValuePair<GameObject, int>> selection)
    {
        KeyValuePair<GameObject, int> next = GetRandomEnemy(selection);
        EnemyClassifier classifier = next.Key.GetComponent<EnemyClassifier>();

        while (classifier != null && !classifier.willSpawn())
        {
            next = GetRandomEnemy(selection);
            classifier = next.Key.GetComponent<EnemyClassifier>();
        }

        return next;
    }

    private KeyValuePair<GameObject, int> GetRandomEnemy(List<KeyValuePair<GameObject, int>> selection)
    {
        return selection[_random.Next(selection.Count)];
    }

    private List<KeyValuePair<GameObject, int>> GetEnemySelection()
    {
        List<KeyValuePair<GameObject, int>> selection = new List<KeyValuePair<GameObject, int>>();

        if (_waveConfig.differentEnemiesPerWave >= _enemiesWithWeights.Count)
        {
            selection.AddRange(_enemiesWithWeights);
        }
        else
        {
            HashSet<int> selectedIndexes = new HashSet<int>();

            while (selectedIndexes.Count < _waveConfig.differentEnemiesPerWave)
            {
                int randomIndex = _random.Next(_enemiesWithWeights.Count);

                if (!selectedIndexes.Contains(randomIndex))
                {
                    selectedIndexes.Add(randomIndex);
                    selection.Add(_enemiesWithWeights[randomIndex]);
                }
            }
        }

        return selection;
    }
}