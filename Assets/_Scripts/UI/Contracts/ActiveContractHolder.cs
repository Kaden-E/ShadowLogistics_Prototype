using UnityEngine;

public class ActiveContractHolder : MonoBehaviour
{
    public static ActiveContractHolder Instance;

    public ContractDefinition activeContract;
    public int activePayout;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetActiveContract(ContractDefinition contract, int payout)
    {
        if (activeContract != null)
        {
            Debug.LogWarning("Already have an active contract!");
            return;
        }
        
        activeContract = contract;
        activePayout = payout;

        Debug.Log($"ACTIVE CONTRACT SET: {contract.contractName}");
    }
    
    
}