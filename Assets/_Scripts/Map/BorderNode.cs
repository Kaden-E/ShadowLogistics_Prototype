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
}