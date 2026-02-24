using UnityEngine;

public class TownManager : MonoBehaviour
{
    public static TownManager Instance;

    private Town selectedTown;

    void Awake()
    {
        Instance = this;
    }

    public void SelectTown(Town town)
    {
        if (selectedTown == null)
        {
            selectedTown = town;
            selectedTown.SetSelected(true);
        }
        else
        {
            if (selectedTown != town)
            {
                RouteManager.Instance.CreateRoute(selectedTown, town);
            }

            selectedTown.SetSelected(false);
            selectedTown = null;
        }
    }
}