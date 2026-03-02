using TMPro;
using UnityEngine;
using ShadowLogistics.Economy;

public class WalletUI : MonoBehaviour
{
    [SerializeField] private TMP_Text balanceText;

    private void Update()
    {
        if (WalletService.Instance != null)
        {
            balanceText.text = $"£{WalletService.Instance.Balance}";
        }
    }
}