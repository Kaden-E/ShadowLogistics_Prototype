using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class RouteManager : MonoBehaviour
{
    private DeliveryOutcomeService deliveryOutcome;
    private bool wasInspectedThisRun;
    private bool illegalFoundThisRun;
    private bool bribeUsedThisRun;
    private bool runFailed;
    
    private float runStartTime;
    
    
    [Header("Debug")]
    public bool forceInspection = false;
    
    public static RouteManager Instance;

    public GameObject truckPrefab;
    
    private void Start()
    {
        deliveryOutcome = FindFirstObjectByType<DeliveryOutcomeService>();
        if (deliveryOutcome == null)
            Debug.LogError("No DeliveryOutcomeService found. Is Core in the scene?");
    }
    
    void Awake()
    {
        Instance = this;
    }
    
    public void DispatchRoute(List<Town> route)
    {
        List<Town> routeCopy = new List<Town>(route);
        StartCoroutine(MoveTruckAlongRoute(routeCopy));
        if (ConnectionManager.Instance != null)
            ConnectionManager.Instance.ShowRoute(routeCopy);
    }

    private IEnumerator MoveTruckAlongRoute(List<Town> route)
    {
        if (route == null || route.Count < 2) yield break;
        wasInspectedThisRun = false;
        illegalFoundThisRun = false;
        bribeUsedThisRun = false;
        runFailed = false;
        runStartTime = Time.time;

        GameObject truck = Instantiate(truckPrefab, route[0].transform.position, Quaternion.identity);

        float baseSpeed = 2f;

        for (int i = 0; i < route.Count - 1; i++)
        {
            Town a = route[i];
            Town b = route[i + 1];

            // Segment speed: if either endpoint is a BorderNode, apply its multiplier
            float multiplier = 1f;
            if (a is BorderNode ba) multiplier *= ba.speedMultiplier;
            if (b is BorderNode bb) multiplier *= bb.speedMultiplier;

            float speed = baseSpeed * multiplier;

            yield return StartCoroutine(MoveTruckTo(truck, b.transform.position, speed));

            if (b is BorderNode border)
            {
                yield return StartCoroutine(HandleBorderInspection(border));

                if (runFailed)
                {
                    Debug.Log("Run terminated due to inspection.");
                    break;
                }
            }
        }
        if (ConnectionManager.Instance != null)
            ConnectionManager.Instance.HideAll();

        bool success = !runFailed;

        if (deliveryOutcome != null)
        {
            var result = new DeliveryResult
            {
                contractId = "route_test", // later comes from contract scene
                origin = route[0].name,
                destination = route[route.Count - 1].name,
                tier = 1, // later comes from contract

                success = success,
                wasInspected = wasInspectedThisRun,
                illegalFound = illegalFoundThisRun,
                bribeUsed = bribeUsedThisRun,

                instabilityAtStart = 0,
                riskAtStart = 0,

                payout = 0,
                penalty = success ? 0 : 0,
                heatChange = illegalFoundThisRun ? 5 : (wasInspectedThisRun ? 1 : 0),

                timeTakenSeconds = Time.time - runStartTime
            };

            deliveryOutcome.CompleteDelivery(result);
        }

        Destroy(truck);
    }

    
    private IEnumerator MoveTruckTo(GameObject truck, Vector3 target, float speed)
    {
        while (Vector3.Distance(truck.transform.position, target) > 0.01f)
        {
            truck.transform.position = Vector3.MoveTowards(
                truck.transform.position,
                target,
                speed * Time.deltaTime
            );

            Vector3 dir = (target - truck.transform.position).normalized;
            if (dir.sqrMagnitude > 0.0001f)
                truck.transform.up = dir;

            yield return null;
        }

        truck.transform.position = target;
    }
    private IEnumerator FlashBorder(BorderNode border, float duration)
    {
        SpriteRenderer sr = border.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Color originalColor = sr.color;
        Vector3 originalScale = border.transform.localScale;

        float timer = 0f;

        while (timer < duration)
        {
            sr.color = Color.red;
            border.transform.localScale = originalScale * 1.15f;

            yield return new WaitForSeconds(0.15f);

            sr.color = originalColor;
            border.transform.localScale = originalScale;

            yield return new WaitForSeconds(0.15f);

            timer += 0.3f;
        }

        sr.color = originalColor;
        border.transform.localScale = originalScale;
    }
    private IEnumerator HandleBorderInspection(BorderNode border)
    {
        yield return new WaitForSeconds(0.25f);

        bool inspected = forceInspection || Random.value < border.inspectionChance;

        if (!inspected)
            yield break;

        wasInspectedThisRun = true;

        Debug.Log($"INSPECTION at {border.townName} ({border.type})");

        // Visual feedback
        yield return StartCoroutine(FlashBorder(border, border.inspectionDelay));

        if (border.inspectionDelay > 0f)
            yield return new WaitForSeconds(0.5f);

        // --- Illegal detection logic ---
        // For now, simple placeholder detection chance
        float illegalDetectionChance = 0.3f; // 30% chance if carrying illegal goods

        bool carryingIllegalGoods = false; 
        // Replace later with real cargo flag from contract system

        if (carryingIllegalGoods && Random.value < illegalDetectionChance)
        {
            illegalFoundThisRun = true;
            runFailed = true;

            Debug.Log("Illegal goods discovered! Delivery failed.");

            yield break;
        }

        // --- Bribe logic placeholder (future expansion) ---
        bool attemptBribe = false; // hook into UI later

        if (attemptBribe)
        {
            bribeUsedThisRun = true;

            bool bribeSuccess = Random.value < 0.7f; // 70% success chance

            if (!bribeSuccess)
            {
                illegalFoundThisRun = true;
                runFailed = true;

                Debug.Log("Bribe failed! Delivery failed.");
                yield break;
            }

            Debug.Log("Bribe succeeded.");
        }

        Debug.Log("Inspection cleared.");
    }
}