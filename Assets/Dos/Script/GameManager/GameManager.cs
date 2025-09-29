using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [Header("Detect Out of Bounds")]
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform playerSpawnPos,boatSpawnPos;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private TMP_Text countdownText, fadeText;
    [Header("Fade Timeline")]
    [SerializeField] private PlayableDirector fadeScreen;

    [Header("Player and Boat")]
    [SerializeField] private FirstPersonController player;
    [SerializeField] BoatPhysics boat;
    private bool isOutOfBounds = false;
    private bool playerInBoundsLastFrame = true;
    private IEnumerator fadeOutCoroutine;
    
    private void Update()
    {
        // Check if player is inside the radius
        bool playerInRange = Physics.OverlapSphere(startPos.position, radius, playerMask).Length > 0;

        if (!playerInRange)
        {
            countdownText.text = $"{(int)(10 - fadeScreen.time)}s";
            countdownText.alpha = 1;
            fadeText.alpha = 1;
        }
        else
        {
            countdownText.alpha = 0;
            fadeText.alpha = 0;
        }
        
        // Player just went out of bounds
        if (!playerInRange && playerInBoundsLastFrame)
        {
            isOutOfBounds = true;
            if (fadeOutCoroutine != null)
                StopCoroutine(fadeOutCoroutine);
            
            fadeOutCoroutine = PlayForward();
            StartCoroutine(fadeOutCoroutine);
        }
        // Player just came back in bounds
        else if (playerInRange && !playerInBoundsLastFrame)
        {
            isOutOfBounds = false;
            if (fadeOutCoroutine != null)
                StopCoroutine(fadeOutCoroutine);
            
            fadeOutCoroutine = PlayReverse();
            StartCoroutine(fadeOutCoroutine);
        }

        
        if (fadeScreen.time >= 9.5)
        {
                player.SetCanMove(false);
                player.GetComponent<CharacterController>().enabled = false;
                player.gameObject.transform.position = playerSpawnPos.position;
                boat.GetComponentInParent<Rigidbody>().transform.position = boatSpawnPos.position;
                player.GetComponent<CharacterController>().enabled = true;
                player.SetCanMove(true);
        }
        

        playerInBoundsLastFrame = playerInRange;
    }

    private IEnumerator PlayForward()
    {
        fadeScreen.gameObject.SetActive(true);
        fadeScreen.Stop();
        fadeScreen.time = 0;
        fadeScreen.Evaluate();
        fadeScreen.Play();
        fadeScreen.playableGraph.GetRootPlayable(0).SetSpeed(1);
        yield break; // Let timeline play normally
    }

    private IEnumerator PlayReverse()
    {
        fadeScreen.gameObject.SetActive(true);
        
        // Switch to manual update
        fadeScreen.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

        double t = fadeScreen.duration;
        while (t > 0)
        {
            t -= Time.deltaTime;
            fadeScreen.time = t;
            fadeScreen.Evaluate();
            yield return null;
        }

        // Reset timeline to normal update mode
        fadeScreen.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        fadeScreen.Stop();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPos.position, radius);
    }
}
