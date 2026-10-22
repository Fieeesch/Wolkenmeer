using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies
{
    public class GruppiConstellation : MonoBehaviour
    {
        [SerializeField] private GameObject center;
        [SerializeField] private GameObject gruppi; 
        [SerializeField] private int gruppisPerRing;
        [SerializeField] private float ringAmount;
        [SerializeField] private float spawnRadius;
        [SerializeField] private float rotateSpeed;

        private List<GameObject> _gruppiGroup = new List<GameObject>();
        private List<float> _angles = new List<float>();


        void Start()
        {
            var angleStep = 360f / gruppisPerRing;

            var centerGruppi = Instantiate(center, transform.position, Quaternion.identity, transform);
            var centerMovement = centerGruppi.GetComponent<EnemyMovement>();
            if (centerMovement != null)
            {
                centerMovement.enabled = false;
            }

            for (float j = 1f; j <= ringAmount; j++)
            {
                for (int i = 0; i < gruppisPerRing; i++)
                {
                    float angle = angleStep * (i + j/ringAmount);
                    float rad = spawnRadius * j;

                    float x = transform.position.x + rad * Mathf.Cos(angle * Mathf.Deg2Rad);
                    float y = transform.position.y + rad * Mathf.Sin(angle * Mathf.Deg2Rad);

                    var spawnPosition = new Vector3(x, y, transform.position.z);
                    var newGruppi = Instantiate(gruppi, spawnPosition, Quaternion.identity, transform);
                    _gruppiGroup.Add(newGruppi);
                    _angles.Add(angle);
                }
            }
        }

        void Update()
        {
            var rad = 0f;
            var speed = 0.5f;
            
            for (int i = 0; i < _gruppiGroup.Count; i++)
            {
                if (i % gruppisPerRing == 0)
                {
                    rad += spawnRadius;
                    speed += 0.5f;
                }

                if (_gruppiGroup[i] == null)
                {
                    continue;
                }
                _angles[i] += (rotateSpeed * speed * Time.deltaTime) % 360f;
                _angles[i] %= 360f;

                float x = transform.position.x + rad * Mathf.Cos(_angles[i] * Mathf.Deg2Rad);
                float y = transform.position.y + rad * Mathf.Sin(_angles[i] * Mathf.Deg2Rad);

                _gruppiGroup[i].transform.position = new Vector3(x, y, transform.position.z);
            }
        }
    }
}