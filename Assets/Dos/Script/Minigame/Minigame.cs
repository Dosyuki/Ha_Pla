using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections; // <- needed for coroutines

public class Minigame : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] private GameObject minigameUI;

    [SerializeField] private Slider sliderProgress;
    [SerializeField] private float progressBar;
    [SerializeField] private float baseRotationSpeed = 60f; // base speed
    private float rotationSpeed;
    private float rotationX = 0f;

    private Coroutine speedUpRoutine;

    void Start()
    {
        rotationSpeed = baseRotationSpeed;

        sliderProgress.maxValue = 100;
        sliderProgress.minValue = 0;
        sliderProgress.value = 0;
    }

    void Update()
    {
        bool isHit = Physics.Raycast(transform.position, Vector3.up * 5, layerMask);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isHit)
            {
                progressBar -= 10;
            }
            else
            {
                progressBar += 10;
                SpeedUpOnHit();
            }
            progressBar = Mathf.Clamp(progressBar, 0, 100);
            sliderProgress.value = progressBar;
            if (progressBar == 100)
            {
                Inventory.Instance.CurrentRod.BeginRecall();
                Debug.Log("you win");
            }
        }

        if (isHit)
            Debug.DrawRay(transform.position, Vector3.up * 5, Color.green);
        else
            Debug.DrawRay(transform.position, Vector3.up * 5, Color.red);

        rotationX += rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(rotationX, 90, 90);

        // apply only Z to UI
        minigameUI.transform.rotation = Quaternion.Euler(0, 0, rotationX);
    }

    private void SpeedUpOnHit()
    {
        // if a boost is already running, stop it and restart
        if (speedUpRoutine != null)
            StopCoroutine(speedUpRoutine);

        speedUpRoutine = StartCoroutine(SpeedUpCoroutine());
    }

    private IEnumerator SpeedUpCoroutine()
    {
        rotationSpeed = baseRotationSpeed * 1.75f;
        yield return new WaitForSeconds(0.25f);
        rotationSpeed = baseRotationSpeed;
        speedUpRoutine = null; // reset flag
    }
}
