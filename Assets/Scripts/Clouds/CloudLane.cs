using UnityEngine;
using System.Collections;

public class CloudLane : MonoBehaviour
{
    // Ziehe hier im Inspector deine zwei (oder mehr) Wolkenprefabs hinein.
    [SerializeField] private GameObject[] cloudPrefabs;

    [Header("Spawning / Timing")]
    [SerializeField] private float minSpawnDelay = 2f;   // Minimale Zeit zwischen Spawns
    [SerializeField] private float maxSpawnDelay = 5f;   // Maximale Zeit zwischen Spawns

    [Header("Bewegung")]
    [SerializeField] private float laneSpeed = 1f;       // Bewegungsgeschwindigkeit (in -x)
    [SerializeField] private float spawnX = 20f;         // X-Position, an der Wolken spawnen
    [SerializeField] private float despawnX = -20f;      // X-Position, an der Wolken gelöscht werden
    
    [Header("Zufällige Skalierung")]
    [SerializeField] private float minScale = 0.8f;      // Kleinste Zufallsskalierung
    [SerializeField] private float maxScale = 1.2f;      // Größte Zufallsskalierung

    [Header("Zufälliger Z-Offset")]
    [SerializeField] private float minOffset = -1f;
    [SerializeField] private float maxOffset = 1f;
    
    private bool isSpawning = true;

    private void Start()
    {
        // Starte die Koroutine, die fortlaufend Wolken spawnt
        StartCoroutine(SpawnClouds());
    }

    private IEnumerator SpawnClouds()
    {
        while (isSpawning)
        {   
            // Warte eine zufällige Zeit, bevor die nächste Wolke spawnt
            float waitTime = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(waitTime);

            // Zufälliges Wolken-Prefab auswählen
            int randomIndex = Random.Range(0, cloudPrefabs.Length);
            GameObject prefab = cloudPrefabs[randomIndex];
            
            // Zufallsvariablen der Wolke berechnen
            float scale = Random.Range(minScale, maxScale);
            float scaleZ = Random.Range(1f, 2f);  
            float rotation = 90f * Random.Range(1, 5);
            float offset = Random.Range(minOffset, maxOffset);
            
            // Berechnung der Zeit die gewartet werden muss, bis der spawner wieder frei ist.
            float delayToNextCloud = scale / laneSpeed;
            const float epsilon = 0.0001f;
            float offsetZCorrecton = 1f;
            if (Mathf.Abs(rotation - 90f) < epsilon || Mathf.Abs(rotation - 270f) < epsilon)
            {
                offsetZCorrecton = scaleZ;
                delayToNextCloud *= scaleZ;
            }
            
            // Wolke instanzieren (Kind dieses Objekts, optional)
            GameObject newCloud = Instantiate(prefab, transform);

            // Die Wolke auf die richtige Position bringen (y und z sind die des Spawners)
            newCloud.transform.position = new Vector3(transform.position.x + scale * offsetZCorrecton / 2, transform.position.y, transform.position.z + offset);
            
            // Wolke skalieren
            newCloud.transform.localScale = new Vector3(scale, scale, scale * scaleZ);

            // Wolke rotieren (hier nur im Z, falls 2D-Sidescroller)
            newCloud.transform.rotation = Quaternion.Euler(0f, rotation, 0f);

            // Wolkenbewegung initialisieren (falls CloudMover genutzt wird)
            CloudMover mover = newCloud.GetComponent<CloudMover>();
            if (mover != null)
            {
                mover.Init(laneSpeed, despawnX);
            }
            
            // Warten bis die nächste Iteration beginnen kann
            yield return new WaitForSeconds(delayToNextCloud);
        }
    }
}
