using TMPro;
using UnityEngine;

public class ContractHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private ContractContextService contractContext;

    void Start()
    {
        contractContext = FindFirstObjectByType<ContractContextService>();
    }

    void Update()
    {
        if (contractContext == null || !contractContext.HasActive)
        {
            text.text = "No Active Contract";
            return;
        }

        var c = contractContext.Active;

        text.text =
            $"CONTRACT\n" +
            $"{c.origin} → {c.destination}\n\n" +
            $"Tier: {c.tier}\n" +
            $"Risk: {c.riskPercent}%\n" +
            $"Payout: £{c.payout}\n" +
            $"Cargo: {(c.hasIllegalGoods ? "Illegal" : "Legal")}";
    }
}