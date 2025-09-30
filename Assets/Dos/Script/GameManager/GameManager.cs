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
    [SerializeField] private TMP_Text countdownText, fadeText;

    [Header("Fade Timeline")]
    [SerializeField] private PlayableDirector fadeScreen;

    [Header("Player and Boat")]
    [SerializeField] private FirstPersonController player;
    [SerializeField] private BoatPhysics boat;

    private bool playerInBoundsLastFrame = true;
    private Coroutine fadeCoroutine;

    private void Update()
    {
        OutofBoundDetection();
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerStats.Instance.AddMoney(10000);
            InventoryUI.Instance.UpdateText();
        }
    }

    private void OutofBoundDetection()
    {
        bool playerInRange = Physics.OverlapSphere(startPos.position, radius, playerMask).Length > 0;

        // Show countdown if outside
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

        // Just went out
        if (!playerInRange && playerInBoundsLastFrame)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(PlayForwardAndMaybeTeleport());
        }
        // Just came back in
        else if (playerInRange && !playerInBoundsLastFrame)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(PlayReverse());
        }

        playerInBoundsLastFrame = playerInRange;
    }

    private IEnumerator PlayForwardAndMaybeTeleport()
    {
        fadeScreen.gameObject.SetActive(true);
        fadeScreen.Stop();
        fadeScreen.time = 0;
        fadeScreen.Evaluate();
        fadeScreen.Play();
        fadeScreen.playableGraph.GetRootPlayable(0).SetSpeed(1);

        // Wait until timeline completes (10s)
        while (fadeScreen.time < fadeScreen.duration)
        {
            yield return null;
        }

        // Only teleport if it actually reached the end
        player.SetCanMove(false);
        player.GetComponent<CharacterController>().enabled = false;
        boat.ExitBoat();
        player.transform.position = playerSpawnPos.position;
        boat.GetComponentInParent<Rigidbody>().transform.position = boatSpawnPos.position;
        boat.GetComponentInParent<Rigidbody>().transform.rotation = boatSpawnPos.rotation;
        FindAnyObjectByType<BoatHopOn>().SetisOnBoat(false);
        FindAnyObjectByType<BoatHopOn>().SetisInRange(false);
        player.GetComponent<CharacterController>().enabled = true;
        player.SetCanMove(true);
    }

    private IEnumerator PlayReverse()
    {
        fadeScreen.gameObject.SetActive(true);
        fadeScreen.Stop();

        // Jump to end
        fadeScreen.time = fadeScreen.duration;
        fadeScreen.Evaluate();

        // Manual reverse
        fadeScreen.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

        double t = fadeScreen.duration;
        while (t > 0)
        {
            t -= Time.deltaTime;
            fadeScreen.time = t;
            fadeScreen.Evaluate();
            yield return null;
        }

        // Reset to normal
        fadeScreen.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        fadeScreen.Stop();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPos.position, radius);
    }
}
