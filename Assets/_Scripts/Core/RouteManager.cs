using UnityEngine;
using System.Collections;

public class RouteManager : MonoBehaviour
{
    public static RouteManager Instance;
    public BorderNode[] borderNodes;

    public GameObject truckPrefab;

    void Awake()
    {
        Instance = this;
    }

    public void CreateRoute(Town from, Town to)
    {
        // Different countries? go via border
        if (from.country != to.country)
        {
            BorderNode crossing = FindBestCrossing(from, to);
            if (crossing == null)
            {
                Debug.Log("No border crossing set up!");
                return;
            }

            StartCoroutine(MoveTruckViaBorder(from, to, crossing));
            return;
        }

        // Same country: your normal logic
        if (!from.connectedTowns.Contains(to))
        {
            Debug.Log("No connection between " + from.townName + " and " + to.townName);
            return;
        }

        StartCoroutine(MoveTruck(from, to));
    }

    private BorderNode FindBestCrossing(Town from, Town to)
    {
        BorderNode best = null;
        float bestScore = float.MaxValue;

        foreach (BorderNode b in borderNodes)
        {
            if (b == null)
                continue;

            // Ensure this crossing supports these two countries
            bool supportsCountries =
                (b.sideA == from.country && b.sideB == to.country) ||
                (b.sideB == from.country && b.sideA == to.country);

            if (!supportsCountries)
                continue;

            // Distance from town -> border -> town
            float d1 = Vector3.Distance(from.transform.position, b.transform.position);
            float d2 = Vector3.Distance(b.transform.position, to.transform.position);

            // Base truck speed (must match your movement speed)
            float baseSpeed = 2f;

            // Higher multiplier = faster crossing
            float totalTime = (d1 + d2) / (baseSpeed * b.speedMultiplier);

            if (totalTime < bestScore)
            {
                bestScore = totalTime;
                best = b;
            }
        }

        return best;
    }

    private IEnumerator MoveTruck(Town from, Town to)
    {
        GameObject truck = Instantiate(truckPrefab, from.transform.position, Quaternion.identity);

        float baseSpeed = 2f; // tweak this
        yield return StartCoroutine(MoveTruckTo(truck, to.transform.position, baseSpeed));

        Destroy(truck);
    }
    private IEnumerator MoveTruckViaBorder(Town from, Town to, BorderNode border)
    {
        GameObject truck = Instantiate(truckPrefab, from.transform.position, Quaternion.identity);

        float baseSpeed = 2f; // MUST match your FindBestCrossing baseSpeed
        float speed = baseSpeed * border.speedMultiplier;

        // Leg 1: town -> border
        yield return StartCoroutine(MoveTruckTo(truck, border.transform.position, speed));

        // Visible stop at border
        yield return new WaitForSeconds(0.25f);

        // Leg 2: border -> town
        yield return StartCoroutine(MoveTruckTo(truck, to.transform.position, speed));

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

        // Exact stop
        truck.transform.position = target;
    }
}