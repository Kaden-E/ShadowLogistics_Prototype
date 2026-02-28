using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance;

    public Connection[] allConnections;

    void Start()
    {
        HideAll();
    }
    void Awake()
    {
        Instance = this;
    }

    public void HideAll()
    {
        foreach (var c in allConnections)
            if (c != null) c.SetVisible(false);
    }

    public void ShowRoute(List<Town> route)
    {
        HideAll();
        if (route == null || route.Count < 2) return;

        for (int i = 0; i < route.Count - 1; i++)
        {
            Town a = route[i];
            Town b = route[i + 1];

            Connection c = FindConnection(a, b);
            if (c != null) c.SetVisible(true);
        }
    }

    private Connection FindConnection(Town a, Town b)
    {
        foreach (var c in allConnections)
            if (c != null && c.Matches(a, b))
                return c;

        return null;
    }
}