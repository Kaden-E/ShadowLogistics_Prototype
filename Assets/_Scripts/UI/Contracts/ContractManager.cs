using UnityEngine;

public class ContractManager : MonoBehaviour
{
    public ContractDefinition[] availableContracts;
    public TierMultipliers tierMultipliers;

    private void Start()
    {
        Debug.Log("=== CONTRACT BOARD ===");

        foreach (var contract in availableContracts)
        {
            float multiplier = tierMultipliers.GetPayoutMultiplier(contract.tier);
            int finalPayout = Mathf.RoundToInt(contract.basePayout * multiplier);

            Debug.Log(
                $"{contract.contractName} | Tier: {contract.tier} | Risk: {contract.baseRisk} | Payout: £{finalPayout}"
            );
        }
    }
}