using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class RouteManager : MonoBehaviour
{
    [Header("Debug")]
    public bool forceInspection = false;
    
    public static RouteManager Instance;

    public GameObject truckPrefab;
    
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
            }
        }
        if (ConnectionManager.Instance != null)
            ConnectionManager.Instance.HideAll();
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
        if (!inspected) yield break;

        Debug.Log($"INSPECTION at {border.townName} ({border.type})");

        // Flash border visually
        yield return StartCoroutine(FlashBorder(border, border.inspectionDelay));

        // Extra wait 
        if (border.inspectionDelay > 0f)
            yield return new WaitForSeconds(0.5f);
    }
}