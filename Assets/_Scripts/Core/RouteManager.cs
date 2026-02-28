using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class RouteManager : MonoBehaviour
{
    public static RouteManager Instance;

    public GameObject truckPrefab;
    
    void Awake()
    {
        Instance = this;
    }
    
    public void DispatchRoute(List<Town> route)
    {
        // Copy so TownManager can clear its list safely
        List<Town> routeCopy = new List<Town>(route);
        StartCoroutine(MoveTruckAlongRoute(routeCopy));
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

            // Visible stop at border nodes (optional but feels good)
            if (b is BorderNode)
                yield return new WaitForSeconds(0.25f);
        }

        Destroy(truck);
    }

    // Make sure you have this helper already; if not, paste it too:
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
}