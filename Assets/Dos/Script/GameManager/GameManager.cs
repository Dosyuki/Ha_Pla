using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [Header("Detect Out of Bounds")]
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform playerSpawnPos, boatSpawnPos;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask waterMask;
    [SerializeField] private TMP_Text countdownText, fadeText;

    [Header("Fade Timeline")]
    [SerializeField] private PlayableDirector fadeScreen;

    [Header("Player and Boat")]
    [SerializeField] private FirstPersonController player;
    [SerializeField] private BoatPhysics boat;

    private bool playerInBoundsLastFrame = true;
    private Coroutine fadeCoroutine;
    private Collider playerCollider;

    // ตัวแปรสำคัญ: ป้องกันการทำงานซ้ำระหว่างวาร์ป
    private bool isRespawning = false;

    private void Start()
    {
        // --- ส่วนของการโหลดเซฟ ---
        if (GameSession.Instance.CurrentSlotId > 0)
        {
            Debug.Log($"GameManager: กำลังโหลดข้อมูลจาก Slot {GameSession.Instance.CurrentSlotId}");
            bool loadSuccess = SaveManager.Instance.LoadGame(GameSession.Instance.CurrentSlotId);

            if (!loadSuccess)
            {
                Debug.LogError($"Load Slot {GameSession.Instance.CurrentSlotId} ล้มเหลว! (ไฟล์อาจจะไม่มี)");
            }
        }
        else
        {
            Debug.Log("GameManager: เริ่มเกมใหม่ (New Game).");
        }

        // หา Collider ของ Player ที่เอาไว้ชนน้ำ
        var colliderObj = GameObject.Find("PlayerWaterCollider");
        if (colliderObj != null)
            playerCollider = colliderObj.GetComponent<Collider>();
        else
            Debug.LogError("หา 'PlayerWaterCollider' ไม่เจอ! กรุณาเช็คชื่อ GameObject");
    }

    private void Update()
    {
        // 1. ถ้ากำลัง Respawn อยู่ (จอมืด/กำลังวาร์ป) ให้หยุดการทำงานทุกอย่างใน Update
        if (isRespawning) return;

        OutofBoundDetection();
        PlayerCollideWater();

        // Cheat Code
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerStats.Instance.AddMoney(10000);
            InventoryUI.Instance.UpdateText();
        }
    }

    private void PlayerCollideWater()
    {
        if (playerCollider == null) return;

        bool playerCollide = Physics.CheckSphere(playerCollider.transform.position, 0.5f, waterMask);
        
        // ชนน้ำ และ ต้องไม่อยู่ระหว่างการ Respawn
        if (playerCollide && !isRespawning)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            // ส่งค่า 4 ไปเพื่อให้ Fade เร็วขึ้นตอนตกน้ำ
            fadeCoroutine = StartCoroutine(PlayForwardAndMaybeTeleport(4));
        }
    }

    private void OutofBoundDetection()
    {
        bool playerInRange = Physics.OverlapSphere(startPos.position, radius, playerMask).Length > 0;

        // Show countdown if outside
        if (!playerInRange)
        {
            // ป้องกันค่า time ติดลบหรือเกิน
            float timeLeft = Mathf.Max(0, 10 - (float)fadeScreen.time);
            countdownText.text = $"{(int)timeLeft}s";
            countdownText.alpha = 1;
            fadeText.alpha = 1;
        }
        else
        {
            countdownText.alpha = 0;
            fadeText.alpha = 0;
        }

        // เพิ่งออกนอกเขต
        if (!playerInRange && playerInBoundsLastFrame)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(PlayForwardAndMaybeTeleport());
        }
        // เพิ่งกลับเข้ามาในเขต
        else if (playerInRange && !playerInBoundsLastFrame)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(PlayReverse());
        }

        playerInBoundsLastFrame = playerInRange;
    }

    // ฟังก์ชันหลักสำหรับการ Fade -> Warp -> Fade Back
    private IEnumerator PlayForwardAndMaybeTeleport(float playSpeed = 1)
    {
        // 2. ล็อกสถานะทันที เพื่อไม่ให้ Update เรียกซ้ำ
        isRespawning = true;

        fadeScreen.gameObject.SetActive(true);
        fadeScreen.Stop();
        fadeScreen.time = 0;
        fadeScreen.Evaluate();
        fadeScreen.Play();
        
        // ตั้งความเร็ว (ถ้าตกน้ำจะเร็ว, ถ้าออกนอกแมพจะช้า)
        fadeScreen.playableGraph.GetRootPlayable(0).SetSpeed(playSpeed);

        // รอจนกว่า Timeline จะเล่นจบ (จอมืดสนิท)
        while (fadeScreen.time < fadeScreen.duration)
        {
            yield return null;
        }

        // --- เริ่มขั้นตอนการย้ายตำแหน่ง (Teleport) ---
        
        // ปิดการควบคุม Player
        player.SetCanMove(false);
        player.GetComponent<CharacterController>().enabled = false;

        // บังคับให้ออกจากเรือก่อน (ถ้ามี)
        if (boat != null) boat.ExitBoat();

        // ย้าย Player
        player.transform.position = playerSpawnPos.position;
        player.transform.rotation = playerSpawnPos.rotation;

        // ย้ายเรือ (และรีเซ็ตฟิสิกส์เรือ)
        if (boat != null)
        {
            Rigidbody boatRb = boat.GetComponentInParent<Rigidbody>();
            if (boatRb != null)
            {
                boatRb.transform.position = boatSpawnPos.position;
                boatRb.transform.rotation = boatSpawnPos.rotation;
                boatRb.linearVelocity = Vector3.zero; // หยุดเรือไม่ให้พุ่งต่อ
                boatRb.angularVelocity = Vector3.zero;
            }
        }

        // รีเซ็ตสถานะคนบนเรือ (ถ้ามี script BoatHopOn)
        var boatHop = FindAnyObjectByType<BoatHopOn>();
        if (boatHop != null)
        {
            boatHop.SetisOnBoat(false);
            boatHop.SetisInRange(false);
        }

        // เปิดการควบคุม Player
        player.GetComponent<CharacterController>().enabled = true;
        player.SetCanMove(true);

        // 3. *** สั่งให้จอค่อยๆ สว่างกลับมา (สำคัญมาก) ***
        // รอให้ PlayReverse ทำงานจนเสร็จ
        yield return StartCoroutine(PlayReverse());

        // 4. ปลดล็อกสถานะเมื่อทุกอย่างเสร็จสิ้น
        isRespawning = false;
    }

    private IEnumerator PlayReverse()
    {
        fadeScreen.gameObject.SetActive(true);
        fadeScreen.Stop();

        // กระโดดไปที่เฟรมสุดท้าย (จอมืด)
        fadeScreen.time = fadeScreen.duration;
        fadeScreen.Evaluate();

        // ปรับโหมดเป็น Manual เพื่อคุมเวลาถอยหลังเอง
        fadeScreen.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

        double t = fadeScreen.duration;
        float reverseSpeed = 2.0f; // ความเร็วตอนสว่างกลับ (ปรับได้)

        while (t > 0)
        {
            t -= Time.deltaTime * reverseSpeed;
            fadeScreen.time = t;
            fadeScreen.Evaluate();
            yield return null;
        }

        // คืนค่าโหมดและหยุด
        fadeScreen.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        fadeScreen.Stop();
        
        // ซ่อน Timeline object เมื่อเสร็จงาน
        fadeScreen.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (startPos != null)
        {
            Gizmos.DrawWireSphere(startPos.position, radius);
        }
    }
}