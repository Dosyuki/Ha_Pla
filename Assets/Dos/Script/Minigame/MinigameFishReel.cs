using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinigameFishReel : MonoBehaviour
{
    [Header("Fish Reel")]
    public Fish curFish;
    public Slider progressBar;
    public float timeBeforeYellow;

    [Header("ProgressBar Customize")] 
    [SerializeField] private float min = 0;
    [SerializeField] private float max = 100;
    [SerializeField] private float startValue = 30;
    [SerializeField] private float reelPower = 15f; 
    [SerializeField] private float tensionDropSpeed = 5f;
    [SerializeField] private float penaltyDropSpeed = 20f;

    [Header("Minigame State")] 
    [SerializeField] private bool isFishCalm = true;
    [SerializeField] private Image statusIndicator;
    [SerializeField] private Color statusIndicatorColor;

    [Header("UI Customization")] 
    [SerializeField] private Sprite fishCalmSprite;
    [SerializeField] private Sprite fishDashSprite;
    [SerializeField] private Image fishImage;
    [SerializeField] private GameObject NormalRod;
    [SerializeField] private GameObject ReelRod;
    
    
    private List<BaseFish.movementPattern> movementPatterns;
    private UIJiggle JiggleFish;
    private bool isGameActive = false;

    void Start()
    {
        progressBar.minValue = min;
        progressBar.maxValue = max;
        progressBar.value = startValue;
        JiggleFish = GetComponentInChildren<UIJiggle>();
    }

    public void SetupFish(Fish fish)
    {
        curFish = fish;
        movementPatterns = new List<BaseFish.movementPattern>(fish.GetBaseFish().movementPatterns);
        
        this.enabled = true;
        isGameActive = true;
        StartCoroutine(FishMovementSequence());
    }

    void Update()
    {
        if (!isGameActive) return;

        bool isPressing = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);

        if (isFishCalm) // --- ช่วงปลาสงบ (สีเขียว/เหลือง) ---
        {
            if (isPressing)
            {
                // ดึงเข้าหาตัว
                progressBar.value += reelPower * Time.deltaTime;
            }
            else
            {
                // ปล่อยไว้เฉยๆ ค่าลดลงนิดหน่อยตามแรงน้ำ
                progressBar.value -= tensionDropSpeed * Time.deltaTime;
            }
            JiggleFish.StopShake();
        }
        else // --- ช่วงปลาแง้น/ดิ้น (สีแดง) ---
        {
            // ปลาว่ายหนี "ตลอดเวลา" (ใช้ค่าความเร็วปลามาช่วยให้แต่ละตัวแง้นไวไม่เท่ากันได้)
            float fishRunSpeed = tensionDropSpeed * 2f; 
            
            if (isPressing)
            {
                // ถ้าฝืนดึงตอนปลาแง้น = แง้นออกไปไวขึ้นมหาศาล (Penalty)
                progressBar.value -= (fishRunSpeed + penaltyDropSpeed) * Time.deltaTime;
            }
            else
            {
                // ปล่อยมือเฉยๆ ปลาก็ยังแง้นหนีออกไปอยู่ดี
                progressBar.value -= fishRunSpeed * Time.deltaTime;
            }
            JiggleFish.StartShake();
        }
        
        // --- ส่วนที่เหลือเหมือนเดิม ---
        if (progressBar.value >= max) WinGame();
        else if (progressBar.value <= min) LoseGame();
        
        UpdateRodSprite(isPressing);
        UpdateFishSprite();
        Inventory.Instance.CurrentRod.GetBaitTransform().GetComponent<LineRenderer>().SetColors(statusIndicatorColor, statusIndicatorColor);
    }

    private IEnumerator FishMovementSequence()
    {
        int index = 0;
        int listLength = movementPatterns.Count;

        while (isGameActive && listLength > 0)
        {
            // ดึง Pattern ปัจจุบัน
            var pattern = movementPatterns[index % listLength];
            
            // --- Phase 1: ปลาเหนื่อย (ดึงได้) ---
            isFishCalm = true;
            UpdateIndicator(Color.green); 
            yield return new WaitForSeconds(pattern.timeLeft - timeBeforeYellow);
            
            UpdateIndicator(Color.yellow);
            yield return new WaitForSeconds(timeBeforeYellow);

            // --- Phase 2: ปลาดิ้น (ห้ามดึง) ---
            isFishCalm = false;
            UpdateIndicator(Color.red);
            yield return new WaitForSeconds(pattern.speedTime);

            index++;
        }
    }

    private void UpdateIndicator(Color color)
    {
        statusIndicatorColor = color;
        if (statusIndicator != null) statusIndicator.color = statusIndicatorColor;
    }

    private void WinGame()
    {
        isGameActive = false;
        StopAllCoroutines();
        Debug.Log("จับปลาสำเร็จ!");
        
        Inventory.Instance.CurrentRod.BeginRecall(); 
        Destroy(gameObject);
        Inventory.Instance.CurrentRod.GetBaitTransform().GetComponent<LineRenderer>().SetColors(statusIndicatorColor, statusIndicatorColor);
        
    }

    private void LoseGame()
    {
        isGameActive = false;
        StopAllCoroutines();
        Debug.Log("ปลาหลุด!");
        
        Inventory.Instance.CurrentRod.ObtainFish(null); 
        Destroy(gameObject);
        Inventory.Instance.CurrentRod.GetBaitTransform().GetComponent<LineRenderer>().SetColors(statusIndicatorColor, statusIndicatorColor);
        
    }

    private void UpdateFishSprite()
    {
        fishImage.sprite = isFishCalm ?  fishCalmSprite : fishDashSprite;
    }
    private void UpdateRodSprite(bool isPressing)
    {
        NormalRod.SetActive(!isPressing);
        ReelRod.SetActive(isPressing);
    }
}