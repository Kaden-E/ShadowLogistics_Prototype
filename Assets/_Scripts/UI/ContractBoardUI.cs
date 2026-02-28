using UnityEngine;
using UnityEngine.SceneManagement;
public class ContractBoardUI : MonoBehaviour
{
    [SerializeField] private string mapSceneName = "MainMap";
    
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
        // Calculate final payout
        float mult = tierMultipliers.GetPayoutMultiplier(c.tier);
        int finalPayout = Mathf.RoundToInt(c.basePayout * mult);

        // Your existing active contract/UI flow
        ActiveContractHolder.Instance.SetActiveContract(c, finalPayout);
        FindObjectOfType<ActiveContractUI>()?.Refresh();

        Debug.Log("Active Contract showing");
        Debug.Log($"ACCEPTED: {c.contractName}");

        // Bridge into Core for the map/route scene
        var contractContext = FindFirstObjectByType<ContractContextService>();
        if (contractContext != null)
        {
            contractContext.SetActive(new ActiveContractContext
            {
                contractId = c.contractName,
                origin = "TBD_ORIGIN",
                destination = "TBD_DEST",
                tier = (int)c.tier,

                hasIllegalGoods = c.isIllegal,
                payout = finalPayout,
                penalty = 0,

                riskPercent = Mathf.RoundToInt(c.baseRisk * 100f)
            });
        }
        else
        {
            Debug.LogWarning("No ContractContextService found (Core missing). Contract won't carry into route scene yet.");
        }

        // Close board UI
        boardRoot.SetActive(false);


        SceneManager.LoadScene(mapSceneName);
    }
    
}