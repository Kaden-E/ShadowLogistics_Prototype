using TMPro;
using UnityEngine;
using ShadowLogistics.Heat;

public class HeatHUD : MonoBehaviour
{
    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (_text == null || HeatService.Instance == null)
            return;

        // Pull current region from HeatService OR from a static run tracker
        string regionId = GetCurrentRegion();

        if (string.IsNullOrEmpty(regionId))
        {
            _text.text = "HEAT: -";
            return;
        }

        float heat = HeatService.Instance.GetHeat(regionId);
        _text.text = $"HEAT: {heat:0}";
    }

    private string GetCurrentRegion()
    {
        // Easiest option:
        return RouteManager.CurrentHeatRegionId; 
        // (we’ll add this in a second)
    }
}