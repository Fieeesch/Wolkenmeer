using UnityEngine;

public class BreakProgress : MonoBehaviour
{
    [SerializeField] GameObject Flock;
    [SerializeField] Transform PointA;
    [SerializeField] Transform PointB;

    private float max_distance;
    private float elapsed;
    private bool started;
    private float duration = 10f;

    public void StartProgress(float duration)
    {
        this.duration = duration;
        Flock.SetActive(true);
        started = true;
    }

    public void StopProgress()
    {
        Flock.SetActive(false);
        started = false;
        elapsed = 0.0f;
    }

    private void FixedUpdate()
    {
        if(started)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration; 
            Flock.transform.position = Vector3.Lerp(PointA.position, PointB.position, t); 
            if (elapsed >= duration) {
                StopProgress();
            }
        }
    }
}
