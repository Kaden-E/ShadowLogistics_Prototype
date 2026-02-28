using UnityEngine;

[CreateAssetMenu(menuName = "Shadow Logistics/Tier Multipliers")]
public class TierMultipliers : ScriptableObject
{
    public float localPayout = 1f;
    public float regionalPayout = 1.8f;
    public float highRiskPayout = 3f;
    public float blackNetworkPayout = 5f;

    public float GetPayoutMultiplier(ContractTier tier)
    {
        return tier switch
        {
            ContractTier.Local => localPayout,
            ContractTier.Regional => regionalPayout,
            ContractTier.HighRisk => highRiskPayout,
            ContractTier.BlackNetwork => blackNetworkPayout,
            _ => 1f
        };
    }
}