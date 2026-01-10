using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class Bed : MonoBehaviour
{
    [SerializeField] private PlayableDirector sleep; // one timeline (used forward + reverse)
    [SerializeField] private CanvasGroup BedWorldUI;

    private bool isSleeping = false;
    private bool playerInside = false;

    private void Start()
    {
        if (BedWorldUI == null)
            BedWorldUI = GetComponentInChildren<CanvasGroup>();

        BedWorldUI.alpha = 0;
    }

    private void Update()
    {
        if (!playerInside) return;

        // Sleep
        if (Input.GetKeyDown(KeyCode.F) && !isSleeping)
        {
            TimeSystem.Instance.Sleep();
            UIManager.Instance.ChangeState(currentState.UI);
            isSleeping = true;
            StartCoroutine(PlayForwardNextFrame());
        }
        // Wake up
        else if ((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Escape)) && isSleeping)
        {
            UIManager.Instance.ChangeState(currentState.None);
            TimeSystem.Instance.Sleep();
            isSleeping = false;
            PlayReverse();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Must have Player layer
        if (other.CompareTag("Player")) 
        {
            BedWorldUI.alpha = 1;
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            BedWorldUI.alpha = 0;
            playerInside = false;
        }
    }

    private IEnumerator PlayForwardNextFrame()
    {
        sleep.gameObject.SetActive(true);
        yield return null;
        sleep.Stop();
        sleep.time = 0;
        sleep.Evaluate();
        sleep.Play();
        sleep.playableGraph.GetRootPlayable(0).SetSpeed(1); // forward
    }

    private void PlayReverse()
    {
        sleep.gameObject.SetActive(true);
        sleep.Stop();

        // Jump to end
        sleep.time = sleep.duration;
        sleep.Evaluate();

        // Manual reverse update
        sleep.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
        StartCoroutine(PlayReverseCoroutine());
    }

    private IEnumerator PlayReverseCoroutine()
    {
        double t = sleep.duration;

        while (t > 0)
        {
            t -= Time.deltaTime;
            sleep.time = t;
            sleep.Evaluate();
            yield return null;
        }

        // Reset to normal
        sleep.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
    }
}
