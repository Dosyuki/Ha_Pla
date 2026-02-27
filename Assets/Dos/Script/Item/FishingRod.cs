using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FishingRod : BaseItem
{
    private static readonly int IsCharging = Animator.StringToHash("isCharging");
    private static readonly int IsThrown = Animator.StringToHash("isThrown");
    private static readonly int FishBite = Animator.StringToHash("fishBite");

    [Header("Fishing Charge")]
    [SerializeField] private Slider fishingSlider;
    public float chargeSpeed = 1f; 

    private int direction = 1; 
    private bool isStopped = false;
    private bool isCharging = false;

    [Header("Thrown Section")]
    [SerializeField] private float ThrowPower = 10f;
    [SerializeField] private float RecallSpeed = 5f;
    [SerializeField] private bool isThrown;
    [SerializeField] private LayerMask FishingLayer;
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private MouseLook mouseLook;
    [SerializeField] private Animator rodAnimator;
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
    
    // ตัวแปรสำหรับเก็บ Coroutine เพื่อสั่งหยุดเมื่อจำเป็น
    private Coroutine waitFishCoroutine;


    private void Start()
    {
        Prefab = this.gameObject;
        bait = Prefab.GetComponentInChildren<Rigidbody>();
        baitTransform = bait.transform;

        playerController = FindObjectOfType<FirstPersonController>();
        mouseLook = playerController.GetMouseLook();
        fishCollectUI = FindObjectOfType<FishCollectUI>(true);
        rodAnimator = GetComponent<Animator>();

        lineRenderer = bait.GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 20;
        lineRenderer.startWidth = 0.03f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        
        fishingSlider.minValue = 1;
        fishingSlider.maxValue = 2;
        fishingSlider.value = 1;
        
        HideSliderCanvas(true);
    }

    private void Update()
    {
        if(UIManager.Instance.GetCurrentState() == currentState.UI)
            return;

        // Start charging
        if (Input.GetMouseButtonDown(0) && !isThrown)
        {
            // 2. เช็คว่า "มีตัวเหยื่อถูกเลือกอยู่ไหม" (กัน Error NullReference)
            if (Inventory.Instance.currentBait != null)
            {
                // 3. ถ้ามีเหยื่อ -> เช็คจำนวนว่าเหลือพอมั้ย
                if (Inventory.Instance.currentBait.amount > 0)
                {
                    // มีของครบ -> เริ่มตกปลา
                    StartCharging();
                    Inventory.Instance.currentBait.amount--;
                }
                else
                {
                    // มีตัวเหยื่อ แต่จำนวนเป็น 0 -> แจ้งเตือน
                    UIAlert.Instance.FishWarning();
                }
            }
            else
            {
                // ไม่มีเหยื่อถูกเลือกเลย (currentBait เป็น null) -> แจ้งเตือน
                UIAlert.Instance.FishWarning();
            }
        }

        if (Input.GetMouseButton(0) && isCharging)
        {
            UpdateCharging();
        }

        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            ReleaseCharge();
        }

        // Right click recall (Cancel fishing)
        if (Input.GetMouseButtonDown(1) && isThrown && !isRecalling && !waitingForFish)
        {
            StartRecall();
            ObtainFish(); 
        }

        // --- แก้ไข: เช็ค FishingHook เฉพาะตอนที่เหวี่ยงเบ็ดไปแล้วเท่านั้น ---
        if (isThrown)
        {
            FishingHook();
        }
        
        rodAnimator.SetBool(IsCharging,isCharging);
        rodAnimator.SetBool(IsThrown,isThrown);
        rodAnimator.SetBool(FishBite,minigame != null); // เช็คว่ามี object minigame อยู่จริงไหม
    }

    private void LateUpdate()
    {
        if (isThrown || isRecalling)
        {
            UpdateLine();
        }
    }

    // ----------------- FISHING MINI-GAME -----------------

    private void StartCharging()
    {
        HideSliderCanvas(false);
        fishingSlider.value = 1.0f;
        direction = 1;
        isStopped = false;
        isCharging = true;
    }

    private void UpdateCharging()
    {
        if (!isStopped && !fishCollectUI.isOpen)
        {
            fishingSlider.value += direction * chargeSpeed * Time.deltaTime;

            if (direction == 1 && fishingSlider.value >= 2f)
            {
                fishingSlider.value = 2f;
                direction = -1;
            }
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
        HideSliderCanvas(true);
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
    }

    public void ObtainFish(Fish obtainedFish = null)
    {
        // --- แก้ไข: สั่งหยุดการรอทันที เพื่อป้องกันบั๊กเดินไม่ได้ ---
        if (waitFishCoroutine != null)
        {
            StopCoroutine(waitFishCoroutine);
            waitFishCoroutine = null;
        }
        waitingForFish = false;

        // Reset ค่าต่างๆ
        isRecalling = false;
        isThrown = false;
        lineRenderer.enabled = false;
        rodTip.gameObject.SetActive(true);

        SetBaitDefaultPosition();
        
        // คืนการควบคุมให้ผู้เล่น
        playerController.enabled = true;
        
        bait.GetComponent<MeshRenderer>().enabled = true;
        bait.isKinematic = true; 
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
        // เช็คว่ามีอะไรอยู่ในระยะไหม
        Collider[] hits = Physics.OverlapSphere(baitTransform.position, 0.6f, FishingLayer);

        if (!waitingForFish && !isRecalling && hits.Length > 0)
        {
            // ดึง Layer ของน้ำก้อนแรกที่เหยื่อชน
            int hitWaterLayer = hits[0].gameObject.layer; 

            // ส่ง Layer เข้าไปใน Coroutine
            waitFishCoroutine = StartCoroutine(WaitForFish(hitWaterLayer));
        }
    }

    // รับ parameter hitWaterLayer เข้ามา
    IEnumerator WaitForFish(int hitWaterLayer)
    {
        waitingForFish = true; 
        float randomTime = Random.Range(2f, 3f);
        yield return new WaitForSeconds(randomTime);
        
        if (!isThrown) 
        {
            waitingForFish = false;
            yield break;
        }

        bait.isKinematic = true;
        if(minigame == null)
        {
            var director = GameObject.Find("FishAlert")?.GetComponent<PlayableDirector>();
            if(director != null) director.Play();
        }
            
        yield return new WaitForSeconds(1.5f);
        
        if (minigame == null && !isRecalling && isDoneMinigame && isThrown)
        {
            // ส่ง hitWaterLayer ไปให้ FishManager
            Fish caughtFish = FishManager.Instance.RandomFish(hitWaterLayer, LuckMultiplier, WeightMultiplier, thrownMultipier, Inventory.Instance.currentBait);
            
            currentFish = caughtFish;
            minigame = Instantiate(newMinigame,GameObject.Find("MinigameCanvas").transform);
            minigame.GetComponent<MinigameFishReel>().SetupFish(currentFish);
            isDoneMinigame = false;
            
            Debug.Log("Start Playing Minigame");
            playerController.enabled = false;
        }
            
        waitingForFish = false; 
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

    // --- Recall Sequence ---
    public void BeginRecall()
    {
        isRecalling = true;
        
        // ลบ UI มินิเกมทันที (เผื่อไว้)
        if(minigame != null) Destroy(minigame); // ไม่หน่วงเวลา
        
        playerController.enabled = true;
        HideSliderCanvas(true);
        
        bait.isKinematic = false;

        if (currentFish.PrefabModel != null)
        {
            GameObject visualFish = Instantiate(currentFish.PrefabModel, baitTransform.position, Quaternion.identity, baitTransform);
            
            Vector3 playerPos = PlayerStats.Instance.transform.position;
            Vector3 directionToPlayer = (playerPos - baitTransform.position).normalized;
            Vector3 horizontalDir = new Vector3(directionToPlayer.x, 0, directionToPlayer.z).normalized;
            
            Vector3 launchDir = Vector3.Slerp(horizontalDir, Vector3.up, 0.5f).normalized;

            float launchForce = 50f; 
            bait.linearVelocity = Vector3.zero; 
            bait.AddForce(launchDir * launchForce, ForceMode.Impulse);

            visualFish.transform.rotation = Quaternion.LookRotation(launchDir);

            StartCoroutine(RecallSequence(visualFish));
        }
        else
        {
            ObtainFish();
        }

        Debug.Log($"Caught a {currentFish.Rarity} {currentFish.Name} weighing {currentFish.Weight:F2}kg!");
    }

    public void ClearMinigame()
    {
        minigame = null;
    }

    private IEnumerator RecallSequence(GameObject visualFish)
    {
        yield return new WaitForSeconds(1.5f);

        if(visualFish != null) Destroy(visualFish);

        if(currentFish != null) 
            fishCollectUI.UpdateFish(currentFish);

        ObtainFish();
    }

    // ----------------- GETTERS SETTER -----------------
    public bool getIsThrown() => isThrown;
    public LayerMask getFishingLayer() => FishingLayer;
    public void HideSliderCanvas(bool hide) => sliderCanvasGroup.alpha = hide ? 0 : 1;
    public Transform GetBaitTransform() => baitTransform;
    public FishCollectUI GetFishCollectUI() => fishCollectUI;
    public void SetIsRecalling(bool isRecall) => isRecalling = isRecall;
}