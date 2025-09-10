using UnityEngine;

public class TimeSystem : MonoBehaviour
{
    [SerializeField] private Light Sun;

    public float timeToRotate = 300f; 

    void Update()
    {
        Sun.transform.Rotate(Vector3.right, 360 / timeToRotate * Time.deltaTime);
    }
}