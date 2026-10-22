using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    private GameObject previous;

    void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected != null && previous != selected)
        {
            if (previous != null)
            {
                AudioManager.Instance.PlayUI_MOVE(audioSource);
            }
            previous = selected;
        }
    }

    public void PlayUI_MOVE()
    {
        AudioManager.Instance.PlayUI_MOVE(audioSource);
    }

    public void PlayAndDestroyUI_CLICK()
    {
        DontDestroyOnLoad(this);
        AudioManager.Instance.PlayUI_CLICK(audioSource);
        StartCoroutine(WaitForSoundAndDestroy(audioSource));
    }

    public void PlayAndDestroyUI_BACK()
    {
        DontDestroyOnLoad(this);
        AudioManager.Instance.PlayUI_BACK(audioSource);
        StartCoroutine(WaitForSoundAndDestroy(audioSource));
    }

    public void PlayAndDestroyUI_START() {
        DontDestroyOnLoad(this);
        AudioManager.Instance.PlayUI_START(audioSource);
        StartCoroutine(WaitForSoundAndDestroy(audioSource));
    }

    private IEnumerator WaitForSoundAndDestroy(AudioSource src)
    {
        while (src.isPlaying)
        {
            yield return null; // Warte, bis der Sound fertig ist
        }
        Destroy(gameObject);
    }
}
