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
        if(UIManager.Instance.GetCurrentState() == currentState.UI)
            return;

        // Start charging
        if (Input.GetMouseButtonDown(0) && !isThrown && (Inventory.Instance.currentBait != null 
                                                         && Inventory.Instance.currentBait.amount > 0))
        {
            StartCharging();
            Inventory.Instance.currentBait.amount--;
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
            // ถ้ากดคลิกขวาเพื่อยกเลิก ให้ดึงกลับทันทีแบบไม่มีปลา
            StartRecall();
            ObtainFish(); 
        }
        

        FishingHook();
        rodAnimator.SetBool(IsCharging,isCharging);
        rodAnimator.SetBool(IsThrown,isThrown);
        rodAnimator.SetBool(FishBite,minigame);

    }

    // ใช้ LateUpdate วาดเส้น (ตามที่แก้ไขไปครั้งก่อน)
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
        // bait.isKinematic = true; // เอาออก เพื่อให้ Physics ทำงานตอนปลาลอยมา
    }

    public void ObtainFish(Fish obtainedFish = null)
    {
        // ฟังก์ชันนี้จะทำหน้าที่ "จบงาน" รีเซ็ตค่าและเก็บเหยื่อเข้าที่
        isRecalling = false;
        isThrown = false;
        lineRenderer.enabled = false;
        rodTip.gameObject.SetActive(true);

        SetBaitDefaultPosition();
        
        // สั่งให้ตัว Player กลับมาขยับได้
        playerController.enabled = true;
        
        bait.GetComponent<MeshRenderer>().enabled = true;
        bait.isKinematic = true; // ล็อกตำแหน่งเหยื่อไว้ที่ปลายเบ็ด
        isDoneMinigame = true;
        
        // (การโชว์ UI จะทำใน Coroutine แทน)
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
        waitingForFish = true; 
        float randomTime = Random.Range(2f, 3f);
        yield return new WaitForSeconds(randomTime);
        bait.isKinematic = true;
        if(minigame == null)
            GameObject.Find("FishAlert").GetComponent<PlayableDirector>().Play();
        yield return new WaitForSeconds(1.5f);
        
        if (minigame == null && !isRecalling && isDoneMinigame)
        {
            Fish caughtFish = FishManager.Instance.RandomFish(LuckMultiplier, WeightMultiplier,thrownMultipier,Inventory.Instance.currentBait);
            currentFish = caughtFish;
            minigame = Instantiate(newMinigame,GameObject.Find("UICanvas").transform);
            minigame.GetComponent<MinigameFishReel>().SetupFish(currentFish);
            isDoneMinigame = false;
            Debug.Log("Start Playing Minigame");
        }
            
        playerController.enabled = false;
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

    // --- ฟังก์ชันหลักที่แก้ไข ---
    public void BeginRecall()
    {
        isRecalling = true;
        Destroy(minigame,0.5f);
        playerController.enabled = true;
        HideSliderCanvas(true);
        
        // 1. เปิด Physics ให้เหยื่อเคลื่อนที่ได้
        bait.isKinematic = false;

        // Instantiate fish prefab
        if (currentFish.PrefabModel != null)
        {
            // สร้างโมเดลปลาติดกับเหยื่อ
            GameObject visualFish = Instantiate(currentFish.PrefabModel, baitTransform.position, Quaternion.identity, baitTransform);
            
            // คำนวณทิศทางเข้าหาตัวผู้เล่น (ตัดแกน Y ออกแล้วค่อยเงยขึ้น)
            Vector3 playerPos = PlayerStats.Instance.transform.position;
            Vector3 directionToPlayer = (playerPos - baitTransform.position).normalized;
            Vector3 horizontalDir = new Vector3(directionToPlayer.x, 0, directionToPlayer.z).normalized;
            
            // ผสมเวกเตอร์ให้พุ่งขึ้นฟ้า (เฉียงขึ้น 45-60 องศา)
            Vector3 launchDir = Vector3.Slerp(horizontalDir, Vector3.up, 0.5f).normalized;

            // ยิงปลาและเหยื่อขึ้นไป
            float launchForce = 25f; // ปรับความแรงตรงนี้
            bait.linearVelocity = Vector3.zero; // รีเซ็ตแรงเก่า
            bait.AddForce(launchDir * launchForce, ForceMode.Impulse);

            // หมุนปลาให้หันไปทางที่บินไป
            visualFish.transform.rotation = Quaternion.LookRotation(launchDir);

            // เริ่ม Coroutine เพื่อนับเวลาและจบงาน
            StartCoroutine(RecallSequence(visualFish));
        }
        else
        {
            // กรณีไม่มีโมเดล ให้จบเลย
            ObtainFish();
        }

        Debug.Log($"Caught a {currentFish.Rarity} {currentFish.Name} weighing {currentFish.Weight:F2}kg!");
    }

    public void ClearMinigame()
    {
        minigame = null;
    }

    // --- Coroutine ใหม่สำหรับลำดับการดึงปลา ---
    private IEnumerator RecallSequence(GameObject visualFish)
    {
        // รอเวลาให้ปลาลอย (เช่น 1.5 วินาที)
        yield return new WaitForSeconds(1.5f);

        // ทำลายโมเดลปลาทิ้ง
        if(visualFish != null) Destroy(visualFish);

        // โชว์ UI สรุปผล
        if(currentFish != null) 
            fishCollectUI.UpdateFish(currentFish);

        // รีเซ็ตตำแหน่งเหยื่อกลับที่เดิม
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