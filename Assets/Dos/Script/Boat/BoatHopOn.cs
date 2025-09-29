using System;
using TMPro;
using UnityEngine;

public class BoatHopOn : MonoBehaviour
{
    [SerializeField] private TMP_Text boatEnterText;
    [SerializeField] private string text;
    [SerializeField] private BoatPhysics boatPhysics;
    private bool isInRange = false;
    private Transform player;
    private bool isOnBoat;

    private void Update()
    {
        if (isInRange)
        {
            if (Input.GetKeyDown(KeyCode.F) && !isOnBoat)
            {
                var fpc = player.GetComponent<FirstPersonController>();
                fpc.SetCanMove(false); // stop movement updates
                fpc.GetComponent<CharacterController>().enabled = false;

                player.transform.position = boatPhysics.GetExitPoint().position;
                player.transform.rotation = boatPhysics.GetExitPoint().rotation;

                fpc.GetComponent<CharacterController>().enabled = true;
                fpc.SetCanMove(true); // resume movement updates
                
                boatEnterText.text = String.Empty;
                isOnBoat = true;
            }
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(!isInRange)
                boatEnterText.text = text;
            player = other.transform;
            isInRange =  true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            boatEnterText.text = String.Empty;
            isInRange = false;
            isOnBoat = false;
        }
    }
}
