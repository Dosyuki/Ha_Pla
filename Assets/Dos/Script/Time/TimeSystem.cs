using UnityEngine;

public class TimeSystem : MonoBehaviour
{
    [SerializeField] private Light Sun; // Assign your Directional Light here

    public float timeToRotate = 300f; // 360 degrees over 300 seconds (5 minutes)

    void Update()
    {
        // Rotate around X axis only
        Sun.transform.Rotate(Vector3.right, 360 / timeToRotate * Time.deltaTime);
    }
}