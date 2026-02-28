using UnityEngine;

public class ContractBoardUI : MonoBehaviour
{
    [Header("Data")]
    public ContractDefinition[] availableContracts;
    public TierMultipliers tierMultipliers;

    [Header("UI")]
    public Transform contentParent;
    public ContractRowUI rowPrefab;
    public GameObject boardRoot; 

    private void Start()
    {
        Populate();
    }

    public void Populate()
    {
        // Clear old rows
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        // Spawn rows
        foreach (var c in availableContracts)
        {
            float mult = tierMultipliers.GetPayoutMultiplier(c.tier);
            int finalPayout = Mathf.RoundToInt(c.basePayout * mult);

            var row = Instantiate(rowPrefab, contentParent);
            row.Setup(c, finalPayout, OnContractClicked);
        }
    }

    private void OnContractClicked(ContractDefinition c)
    {
        float mult = tierMultipliers.GetPayoutMultiplier(c.tier);
        int finalPayout = Mathf.RoundToInt(c.basePayout * mult);

        ActiveContractHolder.Instance.SetActiveContract(c, finalPayout);
        FindObjectOfType<ActiveContractUI>()?.Refresh();
        Debug.Log("Active Contract showing");

        Debug.Log($"ACCEPTED: {c.contractName}");

        boardRoot.SetActive(false);
    }
    
}