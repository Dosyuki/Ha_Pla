using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

    [SerializeField] private GameObject newMinigame;
    [SerializeField] private CanvasGroup sliderCanvasGroup;

    private float thrownMultipier = 1f;
    private Rigidbody bait;
    private LineRenderer lineRenderer;
    private FishCollectUI fishCollectUI;
    private bool isRecalling = false;
    private bool isDoneMinigame = true;
    private GameObject minigame;
    private bool waitingForFish = false;


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
        
        fishingSlider.minValue = 1;
        fishingSlider.maxValue = 2;
        fishingSlider.value = 1;
        
        HideSliderCanvas(true);
    }

    private void Update()
    {
        // Start charging when press
        if(UIManager.Instance.GetCurrentState() == currentState.UI)
            return;
        if (Input.GetMouseButtonDown(0) && !isThrown && (Inventory.Instance.currentBait != null 
                                                         && Inventory.Instance.currentBait.amount > 0))
        {
            StartCharging();
            Inventory.Instance.currentBait.amount--;
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
            ObtainFish();
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
        HideSliderCanvas(true);
        fishingSlider.value = 1.0f;   // always restart at 0.2
        direction = 1;
        isStopped = false;
        isCharging = true;
    }

    private void UpdateCharging()
    {
        if (!isStopped && !fishCollectUI.isOpen)
        {
            fishingSlider.value += direction * chargeSpeed * Time.deltaTime;

            // reached top → go down
            if (direction == 1 && fishingSlider.value >= 2f)
            {
                fishingSlider.value = 2f;
                direction = -1;
            }
            // reached bottom → stop completely
            else if (direction == -1 && fishingSlider.value <= 1f)
            {
                fishingSlider.value = 1f;
                direction = 1;
            }
        }
    }

    private void ReleaseCharge()
    {
        StartFishing(1.25f);
        if (fishingSlider.value >= 1.90f)
        {
            fishingSlider.value = 2f;
        }
        thrownMultipier = fishingSlider.value;
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

    public void ObtainFish(Fish obtainedFish = null)
    {
        isRecalling = false;
        isThrown = false;
        lineRenderer.enabled = false;
        rodTip.gameObject.SetActive(true);

        SetBaitDefaultPosition();
        if(currentFish.Value != 0 && obtainedFish != null)
            fishCollectUI.UpdateFish(obtainedFish);
        playerController.enabled = true;
        bait.GetComponent<MeshRenderer>().enabled = true;
        isDoneMinigame = true;
    }

    public void SetBaitDefaultPosition()
    {
        bait.transform.parent = rodTip.transform;
        bait.transform.position = rodTip.position;
        bait.transform.rotation = rodTip.rotation;
    }

    private void FishingHook()
    {
        if (!waitingForFish && !isRecalling && Physics.OverlapSphere(baitTransform.position, 0.6f, FishingLayer).Length > 0)
        {
            StartCoroutine(WaitForFish());
        }
    }

    IEnumerator WaitForFish()
    {
        waitingForFish = true; // lock
        float random = Random.Range(1f, 2f);
        yield return new WaitForSeconds(random);
        bait.isKinematic = true;
        if (minigame == null && !isRecalling && isDoneMinigame)
        {
            Fish caughtFish = FishManager.Instance.RandomFish(LuckMultiplier, WeightMultiplier,thrownMultipier,Inventory.Instance.currentBait);
            currentFish = caughtFish;
            minigame = Instantiate(newMinigame,GameObject.Find("UICanvas").transform);
            minigame.GetComponent<newMinigame>().AssignFish(currentFish);
            isDoneMinigame = false;
            Debug.Log("Start Playing Minigame");
        }
            
        playerController.enabled = false;
        waitingForFish = false; // unlock

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
    public void BeginRecall()
    {
        isRecalling = true;
        Destroy(minigame);
        playerController.enabled = true;
        bait.isKinematic = true;
        HideSliderCanvas(true);
        // Example: player has 1.1x luck, 1.2x weight multiplier
        
        // Instantiate fish prefab
        if (currentFish.PrefabModel != null)
        {
            Instantiate(currentFish.PrefabModel, baitTransform.position, Quaternion.identity, baitTransform);
        }

        Debug.Log($"Caught a {currentFish.Rarity} {currentFish.Name} weighing {currentFish.Weight:F2}kg!");
    }
    public void ClearMinigame()
    {
        minigame = null;
    }


    // ----------------- GETTERS SETTER -----------------
    public bool getIsThrown() => isThrown;
    public LayerMask getFishingLayer() => FishingLayer;
    public void HideSliderCanvas(bool hide) => sliderCanvasGroup.alpha = hide ? 0 : 1;
    public Transform GetBaitTransform() => baitTransform;
    public FishCollectUI GetFishCollectUI() => fishCollectUI;
    public void SetIsRecalling(bool isRecall) => isRecalling = isRecall;


}
