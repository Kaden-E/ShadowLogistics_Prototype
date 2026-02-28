using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour
{
    public static TownManager Instance;

    [Header("Route Planning")]
    public LineRenderer plannedRouteLine; // assign in Inspector
    public bool routePlanningEnabled = true;

    private List<Town> plannedRoute = new List<Town>();

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!routePlanningEnabled) return;

        // Dispatch route
        if (Input.GetKeyDown(KeyCode.Space))
            DispatchPlannedRoute();

        // Cancel route
        if (Input.GetKeyDown(KeyCode.Escape))
            CancelRoute();

        // Undo last segment
        if (Input.GetMouseButtonDown(1))
            UndoLast();
    }

    public void SelectTown(Town town)
    {
        if (!routePlanningEnabled)
        {
            // (optional) keep your old 2-town selection mode here if you want later
            return;
        }

        // First node
        if (plannedRoute.Count == 0)
        {
            plannedRoute.Add(town);
            town.SetSelected(true);
            UpdatePlannedLine();
            Debug.Log("Route start: " + town.townName);
            return;
        }

        Town last = plannedRoute[plannedRoute.Count - 1];

        // Clicking the same town again cancels
        if (town == plannedRoute[0])
        {
            CancelRoute();
            return;
        }

        // Prevent double-click duplicates
        if (town == last) return;

        // Validate adjacency (THIS fixes the illegal-route bug)
        if (!last.connectedTowns.Contains(town))
        {
            Debug.Log("Invalid segment: " + last.townName + " -> " + town.townName);
            return;
        }

        plannedRoute.Add(town);
        town.SetSelected(true);
        UpdatePlannedLine();

        Debug.Log("Added segment: " + last.townName + " -> " + town.townName);
    }
    
    private void DispatchPlannedRoute()
    {
        if (plannedRoute.Count < 2)
        {
            Debug.Log("Route too short. Add at least 2 nodes.");
            return;
        }

        // Final legality check 
        for (int i = 0; i < plannedRoute.Count - 1; i++)
        {
            if (!plannedRoute[i].connectedTowns.Contains(plannedRoute[i + 1]))
            {
                Debug.Log("Route contains illegal hop at index " + i);
                return;
            }
        }

        RouteManager.Instance.DispatchRoute(plannedRoute);

        // Clear selection visuals + route line
        ClearSelectedVisuals();
        plannedRoute.Clear();
        UpdatePlannedLine();
    }

    private void CancelRoute()
    {
        if (plannedRoute.Count == 0) return;

        Debug.Log("Route cancelled.");
        ClearSelectedVisuals();
        plannedRoute.Clear();
        UpdatePlannedLine();
    }

    private void UndoLast()
    {
        if (plannedRoute.Count <= 1) return;

        Town last = plannedRoute[plannedRoute.Count - 1];
        last.SetSelected(false);
        plannedRoute.RemoveAt(plannedRoute.Count - 1);
        UpdatePlannedLine();

        Debug.Log("Undo last segment.");
    }

    private void ClearSelectedVisuals()
    {
        foreach (var t in plannedRoute)
            if (t != null) t.SetSelected(false);
    }

    private void UpdatePlannedLine()
    {
        if (plannedRouteLine == null) return;

        plannedRouteLine.positionCount = plannedRoute.Count;

        for (int i = 0; i < plannedRoute.Count; i++)
            plannedRouteLine.SetPosition(i, plannedRoute[i].transform.position);
    }
}