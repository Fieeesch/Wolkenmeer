using UnityEngine;

public class CloudMover : MonoBehaviour
{
    public float speed;
    public float despawnX;

    // Wird nach dem Instantiieren durch den Spawner aufgerufen
    public void Init(float laneSpeed, float xLimit)
    {
        speed = laneSpeed;
        despawnX = xLimit;
    }
    
    private void Update()
    {
        // Variante 1: via Translate + Space.World
        transform.Translate(speed * Time.deltaTime * Vector3.left, Space.World);

        // Variante 2 (alternativ): transform.position += Vector3.left * speed * Time.deltaTime;

        // Lösche das Objekt, wenn es zu weit links ist
        if (transform.position.x < despawnX)
        {
            Destroy(gameObject);
        }
    }
}