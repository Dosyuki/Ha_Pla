using System;
using UnityEngine;

public class BoatPhysics : MonoBehaviour
{
    [Header("Boat Movement")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float turnSpeed = 1f;
    [SerializeField] private float angularDragWater = 2f;

    [Header("boatWheel Configs")]
    [SerializeField] private CanvasGroup wheelGroup;

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

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ |  RigidbodyConstraints.FreezePositionY;
        rb.isKinematic = true;

        wheelGroup.alpha = 0;
    }

    private void Update()
    {
        if (!playerInsideTrigger) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!inBoat)
                EnterBoat();
            else
                ExitBoat();
        }
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
    }

    private void ExitBoat()
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
    }

    void FixedUpdate()
    {
        if (!canMove) return;
        HandleMovement();
        rb.angularDamping = angularDragWater;
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        rb.AddForce(rb.transform.forward * (moveInput * moveSpeed), ForceMode.Force);

        if (Mathf.Abs(moveInput) > 0.01f)
        {
            float yaw = turnInput * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnOffset = Quaternion.Euler(0f, yaw, 0f);
            rb.MoveRotation(rb.rotation * turnOffset);
            
        }
        
        
    }

    // --- Trigger detection for player ---
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
