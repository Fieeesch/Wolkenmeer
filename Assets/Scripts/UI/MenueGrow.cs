using System.Collections;
using UnityEngine;

public class MenueGrow : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] Vector3 maxScale;
    [SerializeField] Vector3 minScale;

    public void GrowSmoothly()
    {
        if (gameObject.TryGetComponent<RectTransform>(out RectTransform rt))
        {
            StartCoroutine(Grow(rt));
        }
    }

    public void ShrinkSmoothly()
    {
        if (gameObject.TryGetComponent<RectTransform>(out RectTransform rt))
        {
            StartCoroutine(Shrink(rt));
        }
    }

    public IEnumerator Grow(RectTransform rt) { 
        Vector2 initialScale = rt.localScale; 
        float elapsedTime = 0f; 
        while (elapsedTime < duration) {
            rt.localScale = Vector2.Lerp(initialScale, maxScale, elapsedTime / duration); 
            elapsedTime += Time.deltaTime; 
            yield return null; 
        } 
        rt.localScale = maxScale; 
    }

    public IEnumerator Shrink(RectTransform rt) { 
        Vector2 initialScale = rt.localScale;
        float elapsedTime = 0f; 
        while (elapsedTime < duration) {
            rt.localScale = Vector2.Lerp(initialScale, minScale, elapsedTime / duration); 
            elapsedTime += Time.deltaTime; 
            yield return null; 
        }
        rt.localScale = minScale; 
    }
}
