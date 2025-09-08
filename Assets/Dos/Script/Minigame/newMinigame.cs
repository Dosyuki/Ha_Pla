using System;
using UnityEngine;

public class newMinigame : MonoBehaviour
{
    [SerializeField] private RectTransform AreaRect;
    [SerializeField] private RectTransform checkRect;
    [SerializeField] private RectTransform fishPrefab;

    [SerializeField] private float offsetRadiusAreaRect;
    [SerializeField] private float offsetRadiusCheckRect;
    [SerializeField] private float offsetRadiusFishRect;

    [Header("Fish Movement")]
    [SerializeField] private float aiSpeed = 40f;        // normal AI wander speed 
    [SerializeField] private float playerPullSpeed = 50f; // Player control force 
    [SerializeField] private float targetReachThreshold = 10f;
    [SerializeField] private float pathTimeout = 3f;     // max time before switching target
    [SerializeField] private float boostMultiplier = 2f; // boost speed multiplier
    [SerializeField] private float boostDuration = 1f;   // how long the boost lasts

    [Header("Score")] 
    [SerializeField] private float progress;

    private RectTransform currentFish;
    private Vector2 fishTarget;

    // timers
    private float pathTimer;
    private float boostTimer;

    private void Start()
    {
        SpawnFish();
        PickNewTarget(); // set first target
        UIManager.Instance.ChangeState(currentState.UI);
    }

    private void Update()
    {
        if (currentFish == null) return;

        // --- AI wander movement ---
        Vector2 aiDir = (fishTarget - (Vector2)currentFish.position).normalized;

        float currentSpeed = aiSpeed;
        if (boostTimer > 0)
        {
            currentSpeed *= boostMultiplier;
            boostTimer -= Time.deltaTime;
        }

        // If close to target OR timed out → pick a new target
        pathTimer += Time.deltaTime;
        if (Vector2.Distance(currentFish.position, fishTarget) < targetReachThreshold || pathTimer > pathTimeout)
        {
            PickNewTarget();
        }

        // --- Player input movement ---
        Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        // Combine forces
        Vector2 finalDir = aiDir * currentSpeed + inputDir * playerPullSpeed;

        // Move fish
        currentFish.position += (Vector3)(finalDir * Time.deltaTime);

        // Keep inside area circle
        ClampInsideArea(currentFish, AreaRect);

        // Score when fish overlaps checkRect
        if (CircleOverlap(currentFish, checkRect))
        {
            progress += Time.deltaTime * 5f;
        }

        if (progress >= 50f)
        {
            Inventory.Instance.CurrentRod.BeginRecall();
            UIManager.Instance.ChangeState(currentState.None);
        }
    }

    private void SpawnFish()
    {
        // Destroy old fish if it exists
        if (currentFish != null)
        {
            Destroy(currentFish.gameObject);
        }

        for (int i = 0; i < 20; i++)
        {
            currentFish = Instantiate(fishPrefab, AreaRect.parent);

            Vector2 randomPos = GetRandomPositionInside(AreaRect);
            currentFish.position = randomPos;

            if (!CircleOverlap(currentFish, checkRect))
            {
                PickNewTarget();
                return;
            }

            Destroy(currentFish.gameObject);
        }

        Debug.LogWarning("Could not find valid spawn position for fish!");
    }

    private void PickNewTarget()
    {
        fishTarget = GetRandomPositionInside(AreaRect);
        pathTimer = 0f;
        boostTimer = boostDuration; // give boost for a short time
    }

    private Vector2 GetRandomPositionInside(RectTransform area)
    {
        Vector3 center = area.position;
        float radius = GetRadius(area);

        Vector2 randomInside = UnityEngine.Random.insideUnitCircle * (radius - offsetRadiusFishRect);
        return center + (Vector3)randomInside * 0.8f;
    }

    private void ClampInsideArea(RectTransform rect, RectTransform area)
    {
        Vector3 center = area.position;
        float areaRadius = GetRadius(area);
        float fishRadius = GetRadius(rect);

        Vector3 offset = rect.position - center;
        if (offset.magnitude > areaRadius - fishRadius)
        {
            rect.position = center + offset.normalized * (areaRadius - fishRadius);
        }
    }

    private void OnDrawGizmos()
    {
        if (AreaRect == null || checkRect == null) return;

        DrawCircleGizmo(AreaRect, Color.green, offsetRadiusAreaRect);
        DrawCircleGizmo(checkRect, Color.blue, offsetRadiusCheckRect);

        if (currentFish != null)
        {
            DrawCircleGizmo(currentFish, Color.red, offsetRadiusFishRect);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(currentFish.position, fishTarget);
        }
    }

    private void DrawCircleGizmo(RectTransform rect, Color color, float offset)
    {
        Vector3 pos = rect.position;
        pos.z -= 0.1f;
        float radius = rect.rect.width * 0.5f * rect.lossyScale.x + offset;

        Gizmos.color = color;
        Gizmos.DrawWireSphere(pos, radius);
    }

    private bool CircleOverlap(RectTransform r1, RectTransform r2)
    {
        Vector3 pos1 = r1.position;
        Vector3 pos2 = r2.position;

        float radius1 = GetRadius(r1);
        float radius2 = GetRadius(r2);

        float distance = Vector3.Distance(pos1, pos2);

        return distance < (radius1 + radius2);
    }

    private float GetRadius(RectTransform rect)
    {
        if (rect == AreaRect)
            return rect.rect.width * 0.5f * rect.lossyScale.x + offsetRadiusAreaRect;
        if (rect == checkRect)
            return rect.rect.width * 0.5f * rect.lossyScale.x + offsetRadiusCheckRect;
        if (rect == fishPrefab || rect == currentFish)
            return rect.rect.width * 0.5f * rect.lossyScale.x + offsetRadiusFishRect;

        return rect.rect.width * 0.5f * rect.lossyScale.x; // fallback
    }
}
