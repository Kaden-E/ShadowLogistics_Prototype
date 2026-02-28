using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContractRowUI : MonoBehaviour
{
    [SerializeField] private Image background;
    
    [SerializeField] private TMP_Text label;
    private Button button;

    private ContractDefinition contract;
    private System.Action<ContractDefinition> onClicked;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Setup(ContractDefinition contractDef, int finalPayout, System.Action<ContractDefinition> clickedCallback)
    {
        contract = contractDef;
        onClicked = clickedCallback;

        label.text =
            $"{contract.contractName} ({contract.tier})\n" +
            $"Risk {contract.baseRisk:0.00}   £{finalPayout}";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClicked?.Invoke(contract));
        
        switch (contract.tier)
        {
            case ContractTier.Local:
                background.color = new Color(0.8f, 1f, 0.8f);
                break;

            case ContractTier.Regional:
                background.color = new Color(1f, 0.95f, 0.7f);
                break;

            case ContractTier.HighRisk:
                background.color = new Color(1f, 0.7f, 0.7f);
                break;

            case ContractTier.BlackNetwork:
                background.color = new Color(0.6f, 0.6f, 0.6f);
                break;
        }
    }
}