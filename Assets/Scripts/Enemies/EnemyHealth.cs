namespace Enemies
{
    using System.Collections;
    using System.Collections.Generic;
    using Effects;
    using UnityEngine;

    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private float health = 100f;
        public float damageMultiplier = 1f;

        // returns health left
        public float TakeDamage(float damage)
        {
            float finalDamage = damage * damageMultiplier;
            health -= finalDamage;
            StartCoroutine(DamageAnimation());

            if (health <= 0)
            {
                // Wenn man der letzte Gruppi einer Gruppe ist, das Parent Objekt auch zerstören
                var parentTransform = transform.parent;
                if (parentTransform != null)
                {
                    var parentClassifier = parentTransform.GetComponent<EnemyClassifier>();
                    if (parentClassifier != null && parentClassifier.type.Equals(EnemyType.Gruppi))
                    {
                        int siblingCount = 0;
                        foreach (Transform sibling in parentTransform)
                        {
                            siblingCount++;
                        }

                        if (siblingCount <= 1 && parentTransform != null)
                        {
                            Destroy(parentTransform.gameObject);
                        }
                    }
                }

                Destroy(gameObject, 0.04f);
            }

            return health;
        }

        private IEnumerator DamageAnimation()
        {
            var originalRotation = transform.rotation;
            var damagedRotation = Quaternion.Euler(originalRotation.eulerAngles + new Vector3(0, 0, 15f));

            const float animationDuration = 0.07f;
            var elapsedTime = 0f;

            while (elapsedTime < animationDuration)
            {
                transform.rotation =
                    Quaternion.Slerp(originalRotation, damagedRotation, elapsedTime / animationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(0.05f);

            elapsedTime = 0f;
            while (elapsedTime < animationDuration)
            {
                transform.rotation =
                    Quaternion.Slerp(damagedRotation, originalRotation, elapsedTime / animationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.rotation = originalRotation;
        }
    }
}