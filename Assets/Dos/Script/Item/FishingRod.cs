using System;
using UnityEngine;
using UnityEngine.UI;

public class FishingRod : BaseItem
{
    [Header("Fishing Charge")]
    [SerializeField] private Slider fishingSlider;
    public float chargeSpeed = 1f; // speed of up/down movement

    private int direction = 1; // 1 = going up, -1 = going down
    private bool isStopped = false;
    private bool isCharging = false;

    [Header("Thrown Section")]
    [SerializeField] private float ThrowPower = 10f;
    [SerializeField] private float RecallSpeed = 5f;
    [SerializeField] private bool isThrown;
    [SerializeField] private LayerMask FishingLayer;
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private MouseLook mouseLook;
    [SerializeField] public Fish currentFish;

    [SerializeField] private Transform baitTransform;
    [SerializeField] private Transform rodTip;

    [SerializeField] private GameObject MinigameUI;
    

    private Rigidbody bait;
    private LineRenderer lineRenderer;
    private FishCollectUI fishCollectUI;
    private bool isRecalling = false;

    private void Start()
    {
        Prefab = this.gameObject;
        bait = Prefab.GetComponentInChildren<Rigidbody>();
        baitTransform = bait.transform;

        playerController = FindObjectOfType<FirstPersonController>();
        mouseLook = playerController.GetMouseLook();
        fishCollectUI = FindObjectOfType<FishCollectUI>(true);

        lineRenderer = bait.GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 20;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        
        fishingSlider.minValue = 0;
        fishingSlider.maxValue = 1;
        fishingSlider.value = 0;
    }

    private void Update()
    {
        // Start charging when press
        if (Input.GetMouseButtonDown(0) && !isThrown)
        {
            StartCharging();
        }

        // Keep updating while holding
        if (Input.GetMouseButton(0) && isCharging)
        {
            UpdateCharging();
        }

        // Release and throw
        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            ReleaseCharge();
        }

        // Right click recall
        if (Input.GetMouseButtonDown(1) && isThrown)
        {
            StartRecall();
        }

        if (isRecalling)
        {
            RecallBaitLerp();
        }

        FishingHook();
        if (isThrown || isRecalling)
        {
            UpdateLine();
        }
    }

    // ----------------- FISHING MINI-GAME -----------------

    private void StartCharging()
    {
        fishingSlider.value = 0.2f;   // always restart at 0.2
        direction = 1;
        isStopped = false;
        isCharging = true;
    }

    private void UpdateCharging()
    {
        if (!isStopped)
        {
            fishingSlider.value += direction * chargeSpeed * Time.deltaTime;

            // reached top → go down
            if (direction == 1 && fishingSlider.value >= 1f)
            {
                fishingSlider.value = 1f;
                direction = -1;
            }
            // reached bottom → stop completely
            else if (direction == -1 && fishingSlider.value <= 0f)
            {
                fishingSlider.value = 0f;
                isStopped = true; // stop here until release
            }
        }
    }

    private void ReleaseCharge()
    {
        // Example "sweet spots"
        if (fishingSlider.value >= 0.06 && fishingSlider.value < 0.1f)
        {
            StartFishing(1.25f);
            Debug.Log("Good");
        }
        else if (fishingSlider.value >= 0.1f && fishingSlider.value < 0.15f)
        {
            StartFishing(1.5f);
            Debug.Log("Perfect");
        }
        else if (fishingSlider.value >= 0.15f && fishingSlider.value < 0.18f)
        {
            StartFishing(1.25f);
            Debug.Log("Good");
        }
        else
        {
            Debug.Log("Bad");
        }

        isCharging = false;
    }

    // ----------------- FISHING ACTIONS -----------------

    private void StartFishing(float multiplier = 1)
    {
        isThrown = true;
        isRecalling = false;

        bait.isKinematic = false;
        lineRenderer.enabled = true;

        Vector3 throwDirection = (bait.transform.forward + Vector3.up).normalized;
        bait.AddForce(throwDirection * (ThrowPower * multiplier), ForceMode.Impulse);
        bait.transform.parent = null;
    }

    private void StartRecall()
    {
        isRecalling = true;
        bait.isKinematic = true;
    }

    private void RecallBaitLerp()
    {
        float step = RecallSpeed * Time.deltaTime;
        baitTransform.position = Vector3.MoveTowards(baitTransform.position, rodTip.position, step);

        if (Vector3.Distance(baitTransform.position, rodTip.position) < 0.1f)
        {
            isRecalling = false;
            isThrown = false;
            lineRenderer.enabled = false;

            bait.transform.parent = rodTip.transform;
            bait.transform.rotation = rodTip.rotation;
            foreach (Transform child in baitTransform)
            {
                Destroy(child.gameObject);
            }
            if(currentFish != null)
                fishCollectUI.UpdateFish(currentFish);
        }
    }

    private void FishingHook()
    {
        if (Physics.OverlapSphere(baitTransform.position, 0.6f, FishingLayer).Length != 0)
        {
            bait.isKinematic = true;
            MinigameUI.SetActive(true);
            MinigameUI.GetComponentInChildren<Minigame>().StartMinigame();
            
            playerController.enabled = false;
            Debug.Log("Start Playing Minigame");
        }
    }

    private void UpdateLine()
    {
        int segmentCount = lineRenderer.positionCount;
        Vector3 start = rodTip.position;
        Vector3 end = baitTransform.position;

        for (int i = 0; i < segmentCount; i++)
        {
            float t = (float)i / (segmentCount - 1);
            Vector3 point = Vector3.Lerp(start, end, t);

            float distance = Vector3.Distance(start, end);
            float curveAmount = Mathf.Clamp(distance * 0.1f, 0f, 2f);
            float sag = Mathf.Sin(t * Mathf.PI) * curveAmount;
            point.y -= sag;

            lineRenderer.SetPosition(i, point);
        }
    }

    // ----------------- GETTERS -----------------
    public bool getIsThrown() => isThrown;
    public LayerMask getFishingLayer() => FishingLayer;

    public void BeginRecall()
    {
        isRecalling = true;
        MinigameUI.SetActive(false);
        playerController.enabled = true;
        bait.isKinematic = true;

        // Example: player has 1.1x luck, 1.2x weight multiplier
        Fish caughtFish = FishManager.Instance.RandomFish(LuckMultiplier, WeightMultiplier);
        currentFish = caughtFish;
        // Instantiate fish prefab
        if (caughtFish.PrefabModel != null)
        {
            Instantiate(caughtFish.PrefabModel, baitTransform.position, Quaternion.identity, baitTransform);
        }

        Debug.Log($"Caught a {caughtFish.Rarity} {caughtFish.Name} weighing {caughtFish.Weight:F2}kg!");
    }

}
