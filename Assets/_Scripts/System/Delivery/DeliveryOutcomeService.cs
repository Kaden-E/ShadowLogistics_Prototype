using UnityEngine;

public class DeliveryOutcomeService : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private DeliveryHistory history;

    public DeliveryResult LastResult => history != null ? history.GetLast() : null;

    public string CompleteDelivery(DeliveryResult result)
    {
        if (history == null)
        {
            Debug.LogWarning("[DeliveryOutcomeService] No DeliveryHistory assigned. Result won't be stored.");
        }
        else
        {
            history.Add(result);
        }

        string summary = FormatSummary(result);
        Debug.Log(summary);
        return summary;
    }

    private string FormatSummary(DeliveryResult r)
    {
        string status = r.success ? "SUCCESS" : "FAIL";
        string inspected = r.wasInspected ? "yes" : "no";
        string found = r.illegalFound ? "yes" : "no";
        string bribe = r.bribeUsed ? "yes" : "no";

        return $"[Delivery] {status} T{r.tier} {r.origin}→{r.destination} | inspected:{inspected} | illegalFound:{found} | bribe:{bribe} | payout:{r.payout} | penalty:{r.penalty} | heatΔ:{r.heatChange}";
    }
}