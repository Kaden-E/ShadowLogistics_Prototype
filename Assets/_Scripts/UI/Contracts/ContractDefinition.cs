using UnityEngine;
using ShadowLogistics.Cargo;

[CreateAssetMenu(menuName = "Shadow Logistics/Contract Definition")]
public class ContractDefinition : ScriptableObject
{
    public string contractName = "Local Drop";
    [TextArea] public string description;

    public ContractTier tier = ContractTier.Local;

    [Header("Rewards / Risk")]
    public int basePayout = 100;
    [Range(0f, 1f)] public float baseRisk = 0.1f;

    [Header("Route")]
    public float maxDistanceKm = 5f;
    public float timeLimitSeconds = 0f; // 0 = no timer yet (tier 1/2)
    
    [Header("Cargo Type")]
    public bool isIllegal = false;
    
    [Header("Cargo Size")]
    public CargoSize cargoSize = CargoSize.Medium;
    
}