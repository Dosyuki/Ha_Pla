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
    
    private List<BaseFish.movementPattern> movementPatterns;
    private bool isGameActive = false;

    void Start()
    {
        progressBar.minValue = min;
        progressBar.maxValue = max;
        progressBar.value = startValue;
        

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

        if (isPressing)
        {
            if (isFishCalm)
            {
                // ปลาเหนื่อย + กดดึง = ได้แต้ม
                progressBar.value += reelPower * Time.deltaTime;
            }
            else
            {
                // ปลาดิ้น + ดันทุรังกด = เสียแต้มเยอะ (สายจะขาด)
                progressBar.value -= penaltyDropSpeed * Time.deltaTime;
            }
        }
        else
        {
            // ไม่กดอะไรเลย = แต้มลดลงเรื่อยๆ ตามธรรมชาติ
            progressBar.value -= tensionDropSpeed * Time.deltaTime;
        }

        // 3. เช็คชนะ / แพ้
        if (progressBar.value >= max)
        {
            WinGame();
        }
        else if (progressBar.value <= min)
        {
            LoseGame();
        }
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
        if (statusIndicator != null) statusIndicator.color = color;
    }

    private void WinGame()
    {
        isGameActive = false;
        StopAllCoroutines();
        Debug.Log("จับปลาสำเร็จ!");
        
        Inventory.Instance.CurrentRod.BeginRecall(); 
        Destroy(gameObject);
    }

    private void LoseGame()
    {
        isGameActive = false;
        StopAllCoroutines();
        Debug.Log("ปลาหลุด!");
        
        Inventory.Instance.CurrentRod.ObtainFish(null); 
        Destroy(gameObject);
    }
}