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
    [SerializeField] private Transform rodTip;

    private Rigidbody bait;
    private LineRenderer lineRenderer;
    private bool isRecalling = false;

    private void Start()
    {
        Prefab = this.gameObject;
        bait = Prefab.GetComponentInChildren<Rigidbody>();
        baitTransform = bait.transform;

        playerController = FindObjectOfType<FirstPersonController>();
        mouseLook = playerController.GetMouseLook();

        lineRenderer = bait.GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 20;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
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
        if (isThrown || isRecalling)
        {
            UpdateLine();
        }
    }

    private void FishingHook()
    {
        if (Physics.OverlapSphere(baitTransform.position, 0.6f, FishingLayer).Length != 0)
        {
            bait.isKinematic = true;
            Debug.Log("Start Playing Minigame");
        }
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

    private void StartFishing()
    {
        isThrown = true;
        isRecalling = false;

        bait.isKinematic = false;
        lineRenderer.enabled = true;

        Vector3 throwDirection = (bait.transform.forward + Vector3.up).normalized;
        bait.AddForce(throwDirection * ThrowPower, ForceMode.Impulse);
        bait.transform.parent = null;
    }

    private void StartRecall()
    {
        isRecalling = true;
        bait.isKinematic = true;
    }

    private void RecallBaitLerp()
    {
        float step = RecallSpeed * Time.deltaTime;
        baitTransform.position = Vector3.MoveTowards(baitTransform.position, rodTip.position, step);

        if (Vector3.Distance(baitTransform.position, rodTip.position) < 0.1f)
        {
            isRecalling = false;
            isThrown = false;
            lineRenderer.enabled = false;

            bait.transform.parent = rodTip.transform;
        }
    }

    public bool getIsThrown() => isThrown;
    public LayerMask getFishingLayer() => FishingLayer;
}
