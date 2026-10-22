using System.Collections;
using UnityEngine;

public class AnimateCanonStack : MonoBehaviour
{
    [SerializeField] Transform DisplayCanonballParent;
    private float refillSpeed;

    private void Start()
    {
        if(TryGetComponent<ItemSpawner>(out ItemSpawner spawner))
        {
            refillSpeed = spawner.refillDuration / DisplayCanonballParent.childCount;
        } else
        {
            refillSpeed = 0.2f;
        }
    }

    public void RemoveFromStack()
    {
        foreach (Transform child in DisplayCanonballParent.transform)
        {
            child.gameObject.SetActive(false);
        }
        StartCoroutine(SetActiveCanonball());
    }

    IEnumerator SetActiveCanonball()
    {
        foreach (Transform child in DisplayCanonballParent.transform)
        {
            child.gameObject.SetActive(true);
            yield return new WaitForSeconds(refillSpeed);
        }
    }
}
