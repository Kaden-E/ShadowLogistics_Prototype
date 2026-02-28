using UnityEngine;

public class BorderNode : Town
{
    public enum CrossingType { Motorway, Backroad }

    public string borderName = "Crossing";
    public Town.Country sideA = Town.Country.A;
    public Town.Country sideB = Town.Country.B;

    public CrossingType type = CrossingType.Backroad;

    [Header("Tuning")]
    public float speedMultiplier = 1f; // Motorway e.g. 1.5, Backroad e.g. 0.9
    
    [Header("Inspection")]
    [Range(0f, 1f)] public float inspectionChance = 0.15f; // 15% default
    public float inspectionDelay = 2.0f;                   // extra wait if inspected
    public bool canFailDelivery = false;                   // v1 optional
    [Range(0f, 1f)] public float failChanceIfInspected = 0.25f;
}