using UnityEngine;
using UnityEngine.Playables;

public class TransitionFish : Singleton<TransitionFish>
{
    [SerializeField] private Fish currentFish;
    [SerializeField] private PlayableDirector director;
    [SerializeField] private Transform fishSpawnPos;
    
    private Rigidbody rb;
    
    public void StartTransition(Fish fish)
    {
        director.gameObject.transform.SetParent(null);
        
        director.time = 0;
        director.Evaluate();
        director.Play();
        
        UIManager.Instance.ChangeState(currentState.UI);
        
        currentFish = fish;
        
    }
    
    public void SpawnFish()
    {
        GameObject fish = Instantiate(currentFish.PrefabModel, fishSpawnPos.position, fishSpawnPos.rotation);
        rb = fish.AddComponent<Rigidbody>();


        // Direction toward player (world space)
        Vector3 toPlayer = (PlayerStats.Instance.transform.position - fishSpawnPos.position).normalized;

        // Flatten to horizontal plane first
        toPlayer.y = 0;
        toPlayer.Normalize();

        // Axis to tilt around (like "right-hand rule")
        Vector3 tiltAxis = Vector3.Cross(toPlayer, Vector3.up);

        // Rotate that flat direction upwards by 75 degrees
        Quaternion tilt = Quaternion.AngleAxis(75f, tiltAxis);
        Vector3 launchDir = tilt * toPlayer;

        // Apply impulse
        float launchForce = 100f; // tweak strength
        rb.AddForce(launchDir * launchForce, ForceMode.Impulse);

        // OPTIONAL: orient the fish so its local Z+ matches the launch direction
        fish.transform.rotation = Quaternion.LookRotation(launchDir, Vector3.up);
    }

    public void EndTransition()
    {
        Inventory.Instance.CurrentRod.ObtainFish(currentFish);
        Inventory.Instance.CurrentRod.SetBaitDefaultPosition();
        
        director.transform.position = Inventory.Instance.CurrentRod.GetBaitTransform().position;
        director.transform.SetParent(Inventory.Instance.CurrentRod.GetBaitTransform().transform);
        
        Destroy(rb.gameObject,1f);
    }

}
