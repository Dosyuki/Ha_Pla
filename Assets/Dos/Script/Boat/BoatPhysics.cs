using System;
using UnityEngine;

public class BoatPhysics : MonoBehaviour
{
    [Header("Boat Movement")]
    [SerializeField] private float moveSpeed = 15f;     // Forward/Backward speed
    [SerializeField] private float turnSpeed = 1f;     // Turning speed
    
    
    [SerializeField] private float angularDragWater = 2f;

    
    [Header("boatWheel Configs")]
    [SerializeField] private GameObject boatWheel;
    [SerializeField] private Vector3 hitboxWheelSize;
    [SerializeField] private Vector3 offsetWheel;
    [SerializeField] private LayerMask playerMask;
    private Collider[] hitColliders;

    [Header("Other Settings")]
    [SerializeField] private Camera boatCamera;    // assign in Inspector
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private GameObject playerModel; // so you can hide/show player
    [SerializeField] private Transform exitPoint;
    
    private Rigidbody rb;
    private bool canMove = false;
    private bool inBoat = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;         // still falls normally
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; 
    }

    private void Update()
    {
        hitColliders = Physics.OverlapBox(
            boatWheel.transform.position + offsetWheel,
            hitboxWheelSize,
            Quaternion.identity,
            playerMask
        );

        if (hitColliders.Length > 0 && Input.GetKeyDown(KeyCode.F))
        {
            if (!inBoat)
                EnterBoat();
        }
        else if (Input.GetKeyDown(KeyCode.F) && inBoat)
        {
            ExitBoat();
        }
    }
    private void EnterBoat()
    {
        inBoat = true;
        canMove = true;

        // Disable player controller and hide body
        playerController.SetCanMove(false);
        playerModel.SetActive(false);

        // Switch cameras
        playerController.GetComponentInChildren<Camera>().enabled = false;
        playerController.GetComponentInChildren<AudioListener>().enabled = false;
        boatCamera.enabled = true;
        GetComponentInChildren<AudioListener>().enabled = true;
    }

    private void ExitBoat()
    {
        inBoat = false;
        canMove = false;

        // Place player at exit point
        playerController.transform.position = exitPoint.position;
        playerController.transform.rotation = exitPoint.rotation;

        // Enable movement + show player
        playerController.SetCanMove(true);
        playerModel.SetActive(true);

        // Switch cameras
        boatCamera.enabled = false;
        GetComponentInChildren<AudioListener>().enabled = false;
        
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
        float moveInput = Input.GetAxis("Vertical");   // W/S
        float turnInput = Input.GetAxis("Horizontal"); // A/D

        // Forward/backward
        rb.AddForce(transform.right * moveInput * moveSpeed, ForceMode.Force);

        if (Mathf.Abs(moveInput) > 0.01f)
        {
            float yaw = turnInput * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnOffset = Quaternion.Euler(0f, yaw, 0f);
            rb.MoveRotation(rb.rotation * turnOffset);
        }
    }

    private void OnDrawGizmos()
    {
        ShowWireCube(boatWheel,offsetWheel);
    }

    private void ShowWireCube(GameObject obj,Vector3 offset)
    {
        Gizmos.color = Color.green;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(obj.transform.position + offset, obj.transform.rotation, hitboxWheelSize);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = Matrix4x4.identity;
    }
}
