using System;
using UnityEngine;

public class FishingRod : BaseItem
{
    [SerializeField] private float ThrowPower = 10f;
    [SerializeField] private float RecallSpeed = 5f;
    [SerializeField] private bool isThrown;
    [SerializeField] private LayerMask FishingLayer;
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private MouseLook mouseLook;

    [SerializeField] private Transform baitTransform;
    [SerializeField] Transform rodTip;
    private Rigidbody bait;
    private TrailRenderer trail;
    private bool isRecalling = false;
    private void Start()
    {
        Prefab = this.gameObject;
        bait = Prefab.GetComponentInChildren<Rigidbody>();
        trail = bait.GetComponent<TrailRenderer>();
        trail.enabled = false;
        playerController = FindObjectOfType<FirstPersonController>();
        mouseLook = playerController.GetMouseLook();
        
        baitTransform = bait.transform;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isThrown)
        {
            StartFishing();
        }

        if (Input.GetMouseButtonDown(1) && isThrown)
        {
            StartRecall();
        }

        if (isRecalling)
        {
            RecallBaitLerp();
        }
        FishingHook();
    }

    private void FishingHook()
    {
        if (Physics.OverlapSphere(baitTransform.position, 0.6f, FishingLayer).Length != 0)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            Debug.Log("Start Playing Minigame");
        }
    }

    private void StartFishing()
    {
        isThrown = true;
        isRecalling = false;
        
        bait.isKinematic = false;
        trail.enabled = true;
        Vector3 throwDirection = (bait.transform.forward + Vector3.up).normalized;
        bait.AddForce(throwDirection * ThrowPower, ForceMode.Impulse);
        playerController.enabled = false;
        mouseLook.SetCursorLock(false);
    }

    private void StartRecall()
    {
        isRecalling = true;
        bait.isKinematic = true; // stop physics so we can move it manually
    }

    private void RecallBaitLerp()
    {
        float step = RecallSpeed * Time.deltaTime;
        baitTransform.position = Vector3.MoveTowards(baitTransform.position, rodTip.position, step);

        // Stop recalling when it's close enough
        if (Vector3.Distance(baitTransform.position, rodTip.position) < 0.1f)
        {
            isRecalling = false;
            isThrown = false;
            trail.enabled = false;
            
            playerController.enabled = true;
            mouseLook.SetCursorLock(true);
        }
    }
    public bool getIsThrown() => isThrown;
    public LayerMask getFishingLayer() => FishingLayer;
}