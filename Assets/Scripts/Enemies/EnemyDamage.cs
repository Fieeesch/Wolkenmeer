using UnityEngine;

namespace Enemies
{
    public class EnemyDamage : MonoBehaviour
    {
        [SerializeField] private float damageDealt = 10f;

        private void OnTriggerEnter(Collider other)
        {
            // Kollision mit dem Schiff
            if (other.gameObject.CompareTag("EnemyTarget"))
            {
                // Schaden verursachen
                GameManager.Instance.ReduceLife(damageDealt);
                
                // Wenn man Gruppi ist, das Parent Objekt zerstören
                var parentTransform = transform.parent;
                if (parentTransform != null)
                {
                    var parentClassifier = parentTransform.GetComponent<EnemyClassifier>();
                    if (parentClassifier != null && parentClassifier.type.Equals(EnemyType.Gruppi))
                    {
                        // aber erst nach einer Sekunde, sonst kommen die Geschwister nicht mehr am Schiff an
                        Destroy(parentTransform.gameObject, 1f);
                    }
                }

                Destroy(gameObject);
            }
        }
    }
}