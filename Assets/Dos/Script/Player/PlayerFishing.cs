using UnityEngine;

public class PlayerFishing : MonoBehaviour
{
    [SerializeField] private FishingRod currentRod;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public FishingRod GetCurrentRod() => currentRod;
}
