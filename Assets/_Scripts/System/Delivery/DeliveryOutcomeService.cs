using System;
using UnityEngine;
using ShadowLogistics.Economy;

public class DeliveryOutcomeService : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private DeliveryHistory history;

    public DeliveryResult LastResult => history != null ? history.GetLast() : null;
    private WalletService _wallet;

    private void Start()
    {
        _wallet = FindFirstObjectByType<WalletService>();
        if (_wallet == null)
            Debug.LogError("[DeliveryOutcomeService] No WalletService found. Is Core in the scene?");
    }


   
    
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

            // --- v0.7.0 wallet application ---
            if (_wallet == null)
            {
                _wallet = FindFirstObjectByType<WalletService>();
            }

            if (_wallet != null)
            {
                if (result.success && result.payout > 0)
                {
                    _wallet.Add(result.payout, "Contract payout");
                }

                if (result.fineAmount > 0)
                {
                    _wallet.TrySpend(result.fineAmount, $"Inspection fine ({result.severityBand}, {result.foundUnits} units)");
                }
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