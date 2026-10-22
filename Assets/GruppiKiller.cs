using System.Collections;
using UnityEngine;

/// <summary>
/// Checks if children still exist. If not, kys
/// </summary>
public class GruppiKiller : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(CheckAndDestroy());
    }

    IEnumerator CheckAndDestroy()
    {
        while (true)
        {
            if (transform.childCount == 0)
            {
                Destroy(gameObject);
                yield break; // Beende die Coroutine, wenn das Objekt zerstört wird
            }

            // Warte eine Sekunde, bevor die Prüfung erneut erfolgt
            yield return new WaitForSeconds(1f);
        }
    }
}
