using UnityEngine;

public class RotateFan : MonoBehaviour
{
    public Vector3 vector;

    private void FixedUpdate()
    {
        gameObject.transform.Rotate(vector.x * Time.deltaTime, vector.y * Time.deltaTime,vector.z * Time.deltaTime);
    }
}
