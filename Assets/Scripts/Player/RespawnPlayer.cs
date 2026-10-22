using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

/**
 * This script is responsible for respawning the player on an empty grid tile when the player collides with the killbox.
 * The script searches for the grid GameObject and then checks for empty grid tiles. If an empty grid tile is found, the player is respawned on that tile.
 * If no empty grid tiles are found, the player will not respawn and a warning in the log will appear.
 */
public class RespawnPlayer : MonoBehaviour
{
    private GameObject _killBox;
    private GameObject _spawnZone;
    
    private void Start()
    {
        if (_killBox == null)
        {
            _killBox = GameObject.Find("KillBox");
            _spawnZone = GameObject.Find("SpawnZone");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == _killBox)
        {
            //Debug.Log(".Player has fallen off the edge and will now attempt to respawn.");
            Respawn();
        }
    }


    /**
     * Checks for empty spawnpoints within the spawnzone and if an empty spawnpoint is found, the player will respawn on that spawnpoint.
     * "SpawnZone" is the parent GameObject of all spawnpoints within the scene. Spawnpoints are found by checking for empty child GameObjects within the "SpawnZone" GameObject.
     */
    void Respawn(){
        if (_spawnZone != null)
        {
            List<GameObject> spawnpoints = new List<GameObject>();
            foreach (Transform spawnpoint in _spawnZone.transform)
            {
                if (spawnpoint.childCount == 0)
                {
                    spawnpoints.Add(spawnpoint.gameObject);
                }
            }

            if (spawnpoints.Count == 0)
            {
                Debug.LogWarning("No empty grid tiles in spawn zone found, Player could not respawn");
                return;
            }
            else
            {
                foreach (GameObject spawnpoint in spawnpoints)
                {
                    BoxCollider boxCollider = spawnpoint.GetComponent<BoxCollider>();
                    if (boxCollider != null)
                    {   
                        Debug.Log("boxCollider not null");
                        Collider[] colliders = Physics.OverlapBox(boxCollider.bounds.center, boxCollider.bounds.extents, boxCollider.transform.rotation);
                        //check for amount of colliders that are intersecting with each other at the current spawnpoint.
                        //if there are 2 colliders, the player can respawn on that spawnpoint.
                        //2 colliders consist of ship-bug and the spawnpoints collider itself -> meaning there is no additional object at that point.
                        if (colliders.Length == 2 )
                        {
                            transform.position = spawnpoint.transform.position;
                            transform.position = new Vector3(transform.position.x, 2, transform.position.z);
                            return;
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("No spawnpoint found, player could not respawn");
            return;
        }
    }
}