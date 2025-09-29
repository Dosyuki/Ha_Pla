using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class Bed : MonoBehaviour
{
    [SerializeField] private float hitboxSize;
    [SerializeField] private Vector3 offset;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private PlayableDirector sleep; // use only one timeline

    private Collider[] hitColliders;
    private CanvasGroup BedWorldUI;
    private bool isSleeping = false;

    void Start()
    {
        BedWorldUI = GetComponentInChildren<CanvasGroup>();
    }

    void Update()
    {
        hitColliders = Physics.OverlapBox(
            transform.position + offset,
            Vector3.one * hitboxSize,
            Quaternion.identity,
            playerMask
        );

        if (hitColliders.Length > 0)
        {
            BedWorldUI.alpha = 1;

            // Start sleeping (play forward)
            if (Input.GetKeyDown(KeyCode.F) && !isSleeping)
            {
                TimeSystem.Instance.Sleep();
                UIManager.Instance.ChangeState(currentState.UI);
                isSleeping = true;

                StartCoroutine(PlayForwardNextFrame());
            }
            // Wake up (play reverse)
            else if ((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Escape)) && isSleeping)
            {
                UIManager.Instance.ChangeState(currentState.None);
                TimeSystem.Instance.Sleep();
                isSleeping = false;

                PlayReverse();
            }
        }
        else
        {
            BedWorldUI.alpha = 0;
        }
    }

    private IEnumerator PlayForwardNextFrame()
    {
        sleep.gameObject.SetActive(true);
        yield return null; // wait 1 frame
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

        // Set to the end
        sleep.time = sleep.duration;
        sleep.Evaluate();

        // Switch to manual update
        sleep.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

        StartCoroutine(PlayReverseCoroutine());
    }

    private IEnumerator PlayReverseCoroutine()
    {
        double t = sleep.duration;

        while (t > 0)
        {
            t -= Time.deltaTime;   // step backwards
            sleep.time = t;
            sleep.Evaluate();
            yield return null;
        }

        // Reset update mode to normal so future plays work
        sleep.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(
            transform.position + offset,
            transform.rotation,
            transform.lossyScale * hitboxSize
        );
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = Matrix4x4.identity;
    }
}
