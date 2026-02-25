using System;
using UnityEngine;

public class BoatPhysics : MonoBehaviour
{
    [Header("Boat Movement")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float turnSpeed = 1f;
    [SerializeField] private float angularDragWater = 2f;

    [Header("Visual Effects (เอียงเรือ)")]
    [SerializeField] private Transform boatVisualModel; 
    [SerializeField] private MeshRenderer mainBoatVisualModel;
    [SerializeField] private float maxLeanAngle = 15f;   // องศาที่เรือจะเอียงซ้าย/ขวามากที่สุด
    [SerializeField] private float maxPitchAngle = 5f;   // องศาที่หัวเรือจะเชิดขึ้นตอนเร่งความเร็ว
    [SerializeField] private float leanSmoothness = 5f;  // ความสมูทในการเอียง

    [Header("boatWheel Configs")]
    [SerializeField] private CanvasGroup wheelGroup;

    [Header("Camera Look Settings")]
    [SerializeField] private Transform cameraPivot; 
    [SerializeField] private float cameraDistance = 5f; 
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float maxYaw = 90f;   
    [SerializeField] private float maxPitch = 30f; 

    [Header("Other Settings")]
    [SerializeField] private Camera boatCamera;
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private GameObject playerModel;
    [SerializeField] private Transform exitPoint;
    public Transform GetExitPoint() => exitPoint;

    private Rigidbody rb;
    private bool canMove = false;
    private bool inBoat = false;
    private bool playerInsideTrigger = false;

    private float currentYaw = 0f;
    private float currentCamPitch = 0f;

    // เก็บค่าแกนบังคับเพื่อเอาไปคำนวณการเอียงเรือ
    private float currentMoveInput = 0f;
    private float currentTurnInput = 0f;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        rb.useGravity = true;
        // ล็อกแกน X และ Z ไว้เหมือนเดิม เพื่อไม่ให้ฟิสิกส์เรือคว่ำ
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ |  RigidbodyConstraints.FreezePositionY;
        rb.isKinematic = true;

        wheelGroup.alpha = 0;
    }

    private void Update()
    {
        if (inBoat)
        {
            HandleCameraLook();
            mainBoatVisualModel.enabled = false;
            boatVisualModel.GetComponent<MeshRenderer>().enabled = true;
            if (Input.GetKeyDown(KeyCode.F))
            {
                ExitBoat();
            }

            // คำนวณการเอียงโมเดลเรือใน Update เพื่อให้ภาพดูลื่นไหลที่สุด
            HandleBoatTilt();
        }
        else
        {
            // ถ้าไม่ได้ขับเรืออยู่ ให้โมเดลเรือค่อยๆ กลับมาตั้งตรงเหมือนเดิม
            currentMoveInput = Mathf.Lerp(currentMoveInput, 0f, Time.deltaTime * leanSmoothness);
            currentTurnInput = Mathf.Lerp(currentTurnInput, 0f, Time.deltaTime * leanSmoothness);
            HandleBoatTilt();

            if (!playerInsideTrigger) return;

            if (Input.GetKeyDown(KeyCode.F))
            {
                EnterBoat();
            }
        }
    }

    // ฟังก์ชันใหม่: จัดการการเอียงของเรือ
    private void HandleBoatTilt()
    {
        if (boatVisualModel == null) return;

        // คำนวณเป้าหมายองศาการเอียง
        // เชิดหัวขึ้นเมื่อเร่ง (Pitch) และเอียงซ้ายขวาตามการเลี้ยว (Roll)
        // หมายเหตุ: การหมุน (Roll) คือแกน Z, การเชิด (Pitch) คือแกน X
        float targetPitch = currentMoveInput * -maxPitchAngle; // เดินหน้าหัวเชิดขึ้น ถอยหลังหัวทิ่มลง
        float targetRoll = currentTurnInput * -maxLeanAngle;   // เลี้ยวขวาเอียงขวา เลี้ยวซ้ายเอียงซ้าย

        // ถ้าเรือไม่ได้ขยับเดินหน้าเลย จะไม่ให้มันเอียงเวลาเลี้ยวอยู่กับที่ (Optional)
        if (Mathf.Abs(currentMoveInput) < 0.1f)
        {
            targetRoll = 0f;
        }

        Quaternion targetRotation = Quaternion.Euler(targetPitch, 0f, targetRoll);

        // ค่อยๆ สมูทให้เอียงไปยังองศาเป้าหมาย
        boatVisualModel.localRotation = Quaternion.Lerp(boatVisualModel.localRotation, targetRotation, Time.deltaTime * leanSmoothness);
    }

    private void HandleCameraLook()
    {
        if (cameraPivot == null) return;

        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        currentYaw += mouseX;
        currentCamPitch -= mouseY; 

        currentYaw = Mathf.Clamp(currentYaw, -maxYaw, maxYaw);
        currentCamPitch = Mathf.Clamp(currentCamPitch, -maxPitch, maxPitch);

        Quaternion localRotation = Quaternion.Euler(currentCamPitch, currentYaw, 0f);
        boatCamera.transform.rotation = cameraPivot.rotation * localRotation;
        boatCamera.transform.position = cameraPivot.position - (boatCamera.transform.rotation * Vector3.forward * cameraDistance);
    }

    private void EnterBoat()
    {
        inBoat = true;
        canMove = true;
        rb.isKinematic = false;

        playerController.SetCanMove(false);
        playerModel.SetActive(false);

        playerController.GetComponentInChildren<Camera>().enabled = false;
        playerController.GetComponentInChildren<AudioListener>().enabled = false;
        
        boatCamera.enabled = true;
        boatCamera.GetComponent<AudioListener>().enabled = true;

        currentYaw = 0f;
        currentCamPitch = 0f;
        
        if (cameraPivot != null)
        {
            boatCamera.transform.rotation = cameraPivot.rotation;
            boatCamera.transform.position = cameraPivot.position - (boatCamera.transform.rotation * Vector3.forward * cameraDistance);
        }
    }

    public void ExitBoat()
    {
        inBoat = false;
        playerInsideTrigger = false;
        canMove = false;
        rb.isKinematic = true;

        playerController.transform.position = exitPoint.position;
        playerController.transform.rotation = exitPoint.rotation;

        playerController.SetCanMove(true);
        playerModel.SetActive(true);

        boatCamera.enabled = false;
        boatCamera.GetComponent<AudioListener>().enabled = false;
        playerController.GetComponentInChildren<Camera>().enabled = true;
        playerController.GetComponentInChildren<AudioListener>().enabled = true;
        mainBoatVisualModel.enabled = true;
        boatVisualModel.GetComponent<MeshRenderer>().enabled = false;
    }

    void FixedUpdate()
    {
        if (!canMove) return;
        HandleMovement();
        rb.angularDamping = angularDragWater;
    }

    private void HandleMovement()
    {
        // รับค่า input มาเก็บไว้ในตัวแปรคลาส เพื่อให้ HandleBoatTilt() เอาไปใช้งานต่อได้
        currentMoveInput = Input.GetAxis("Vertical");
        currentTurnInput = Input.GetAxis("Horizontal");

        rb.AddForce(rb.transform.forward * (currentMoveInput * moveSpeed), ForceMode.Force);

        if (Mathf.Abs(currentMoveInput) > 0.01f)
        {
            float yaw = currentTurnInput * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnOffset = Quaternion.Euler(0f, yaw, 0f);
            rb.MoveRotation(rb.rotation * turnOffset);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wheelGroup.alpha = 1;
            playerInsideTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wheelGroup.alpha = 0;
            playerInsideTrigger = false;
        }
    }
}