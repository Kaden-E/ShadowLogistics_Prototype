using UnityEngine;
using TMPro;

public class ActiveContractUI : MonoBehaviour
{
    public TMP_Text activeText;
    public GameObject activePanel;
    public GameObject contractBoardRoot; // your ContractBoardPanel

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        var holder = ActiveContractHolder.Instance;

        if (holder != null && holder.activeContract != null)
        {
            activePanel.SetActive(true);
            activeText.text =
                $"ACTIVE: {holder.activeContract.contractName} ({holder.activeContract.tier})\n" +
                $"Payout: £{holder.activePayout}   Risk: {holder.activeContract.baseRisk:0.00}";
        }
        else
        {
            activePanel.SetActive(false);
        }
    }

    public void CompleteContract()
    {
        Debug.Log("CONTRACT COMPLETE (prototype)");
        ActiveContractHolder.Instance.activeContract = null;
        ActiveContractHolder.Instance.activePayout = 0;

        contractBoardRoot.SetActive(true);
        Refresh();
    }

    public void FailContract()
    {
        Debug.Log("CONTRACT FAILED (prototype)");
        ActiveContractHolder.Instance.activeContract = null;
        ActiveContractHolder.Instance.activePayout = 0;

        contractBoardRoot.SetActive(true);
        Refresh();
    }
}