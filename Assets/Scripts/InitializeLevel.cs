using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class InitializeLevel : MonoBehaviour
{
    [SerializeField] private Transform[] playerSpawns;
    [SerializeField] private GameObject playerPrefab;
    void Start()
    {
        Debug.Log("trying to init level");
        var playerConfigsUnsorted = PlayerConfigurationManager.Instance.GetPlayerConfigs();
        
        // Input-Devices mit niedrigerer ID müssen zuerst erstellt werden
        // playerConfigsUnsorted.Sort((prev, next) => prev.ControllerId.CompareTo(next.ControllerId));
        var playerConfigs = playerConfigsUnsorted.ToArray();
        
        for (var i = 0; i < playerConfigs.Length; i++)
        {
            var config = playerConfigs[i];
            var player = Instantiate(playerPrefab, playerSpawns[i].position, playerSpawns[i].rotation, gameObject.transform);
            
            var playerController = player.GetComponent<PlayerController>();
            playerController.Spawn(config);
            GameManager.Instance.BindPlayerToInput(playerController, config.PlayerIndex);
            
            Debug.Log("Player spawned: " + i);
        }

        GameManager.Instance.StartLevel();
    }
}
