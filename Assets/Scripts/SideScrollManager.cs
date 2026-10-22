using UnityEngine;

public class SideScrollManager : MonoBehaviour
{
    public float initialRotation = 150f;
    public float speed = 0.05f;
    
    private float _initialTime;

    void Start()
    {
        _initialTime = Time.time;
    }
    
    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", (Time.time - _initialTime) * speed + initialRotation);
    }
}
