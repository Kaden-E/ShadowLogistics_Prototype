using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EconomyController : MonoBehaviour
{
    [Header("Data")]
    public CountryData[] countries;
    public CargoData[] cargos;
    
    [Header("UI")]
    public TMP_Text statusText;
    public TMP_Text contractText;
    public Button generateButton;
    public Button runButton;
    public Button buyUpgradeButton;

    [Header("Player State")]
    private int contractsCompleted = 0;
    public int money = 2000;
    public int reputation = 0;
    public bool hiddenCompartment = false;

    private Contract current;

    // Simple contract data container
    private struct Contract
    {
        public CargoData cargo;
        public CountryData origin;
        public CountryData destination;

        public int basePay;
        public int riskPercent;
        public int timeLimitDays;
    }

    private void Start()
    {
        generateButton.onClick.AddListener(GenerateContract);
        runButton.onClick.AddListener(RunShipment);
        buyUpgradeButton.onClick.AddListener(BuyHiddenCompartment);

        runButton.interactable = false;
        UpdateUI("Ready. Generate a contract.");
    }

private void GenerateContract()
{
    if (cargos == null || cargos.Length == 0)
    {
        UpdateUI("Assign at least 1 Cargo in the Inspector.");
        Debug.LogError("GenerateContract: cargos not assigned.");
        return;
    }

    if (countries == null || countries.Length < 2)
    {
        UpdateUI("Assign at least 2 Countries in the Inspector.");
        Debug.LogError("GenerateContract: countries not assigned or < 2.");
        return;
    }

    // Pick non-null cargo
    CargoData cargoPick = null;
    for (int i = 0; i < 20 && cargoPick == null; i++)
        cargoPick = cargos[Random.Range(0, cargos.Length)];

    if (cargoPick == null)
    {
        UpdateUI("Cargo array contains only None entries.");
        Debug.LogError("GenerateContract: cargoPick null.");
        return;
    }

    // Pick non-null origin
    CountryData originPick = null;
    for (int i = 0; i < 20 && originPick == null; i++)
        originPick = countries[Random.Range(0, countries.Length)];

    if (originPick == null)
    {
        UpdateUI("Countries array contains only None entries.");
        Debug.LogError("GenerateContract: originPick null.");
        return;
    }

    // Pick destination different from origin
    CountryData destPick = originPick;
    int safety = 0;
    while (destPick == originPick && safety++ < 50)
        destPick = countries[Random.Range(0, countries.Length)];

    if (destPick == null || destPick == originPick)
    {
        UpdateUI("Could not pick a valid destination. Ensure 2+ distinct countries are assigned.");
        Debug.LogError("GenerateContract: destPick invalid.");
        return;
    }

    // Build contract
    current = new Contract
    {
        cargo = cargoPick,
        origin = originPick,
        destination = destPick,
        basePay = cargoPick.basePay,
        timeLimitDays = Random.Range(1, 4)
    };

    // Risk
    int risk = cargoPick.isIllegal ? 45 : 20;
    risk += originPick.instability;
    risk += destPick.instability;
    risk += Mathf.Clamp(reputation / 5, 0, 10);

    if (hiddenCompartment && cargoPick.isIllegal)
        risk -= 12;

    current.riskPercent = Mathf.Clamp(risk, 5, 90);

    runButton.interactable = true;

    Debug.Log($"Generated contract: {current.origin.countryName}({current.origin.instability}) -> {current.destination.countryName}({current.destination.instability})");

    UpdateUI("Contract generated.");
}

    private int CountryRiskModifier(string country)
    {
        // A = stable, B = mixed, C = unstable
        switch (country)
        {
            case "Country A": return 0;
            case "Country B": return 5;
            case "Country C": return 12;
            default: return 0;
        }
    }
    private void CheckForWorldShift()
    {
        if (contractsCompleted % 3 != 0)
            return;

        if (countries == null || countries.Length == 0)
            return;

        // Pick random country
        CountryData target = countries[Random.Range(0, countries.Length)];
        if (target == null)
            return;

        int shift = Random.Range(-2, 3); // -2, -1, 0, 1, 2
        target.instability = Mathf.Clamp(target.instability + shift, 0, 20);

        Debug.Log($"World Shift: {target.countryName} instability changed by {shift}. New value: {target.instability}");
        Debug.Log($"World Shift: {target.countryName} instability now = {target.instability}");
    }

    private void RunShipment()
    {
        if (current.cargo == null)
        {
            UpdateUI("No contract. Generate one first.");
            return;
        }

        // Roll outcome
        int roll = Random.Range(1, 101); // 1..100
        bool caught = roll <= current.riskPercent;

        if (!caught)
        {
            // Success
            int payout = CalculatePayout(current);
            money += payout;

            // Rep: illegal gives more rep
            reputation += current.cargo.isIllegal ? 2 : 1;

            UpdateUI($"SUCCESS! You earned £{payout}.");
        }
        else
        {
            // Failure banding
            int severityRoll = Random.Range(1, 101);

            if (severityRoll <= 55)
            {
                // Partial seizure
                int loss = Mathf.RoundToInt(current.basePay * 0.35f);
                money -= loss;
                reputation = Mathf.Max(0, reputation - 1);
                UpdateUI($"PARTIAL SEIZURE. You lost £{loss}.");
            }
            else if (severityRoll <= 90)
            {
                // Full confiscation
                reputation = Mathf.Max(0, reputation - 2);
                UpdateUI("CONFISCATED. No payout.");
            }
            else
            {
                // Arrest / fine
                int fine = 1200;
                money -= fine;
                reputation = Mathf.Max(0, reputation - 4);
                UpdateUI($"ARRESTED. Fine: £{fine}.");
            }
        }

        // Completed contract + world shift check
        contractsCompleted++;
        CheckForWorldShift();

        // Clear contract
        current = default;
        runButton.interactable = false;
    }

    private int CalculatePayout(Contract c)
    {
        // Base × distance-ish modifier × risk tier
        string o = c.origin.countryName;
        string d = c.destination.countryName;

        float distanceMod =
            ((o == "Country A" && d == "Country B") || (o == "Country B" && d == "Country A")) ? 1.05f :
            ((o == "Country B" && d == "Country C") || (o == "Country C" && d == "Country B")) ? 1.10f :
            1.20f; // A <-> C is “furthest”

        float riskTier = Mathf.Lerp(1.0f, 1.45f, c.riskPercent / 100f);

        int payout = Mathf.RoundToInt(c.basePay * distanceMod * riskTier);
        return payout;
    }

    private void BuyHiddenCompartment()
    {
        int cost = 3000;

        if (hiddenCompartment)
        {
            UpdateUI("Already owned: Hidden Compartment.");
            return;
        }

        if (money < cost)
        {
            UpdateUI($"Not enough money. Need £{cost}.");
            return;
        }

        money -= cost;
        hiddenCompartment = true;
        UpdateUI("Purchased Hidden Compartment! Illegal cargo risk reduced.");
    }

    private void UpdateUI(string message)
    {
        statusText.text =
            $"{message}\n\n" +
            $"Money: £{money}\n" +
            $"Reputation: {reputation}\n" +
            $"Upgrade: Hidden Compartment = {(hiddenCompartment ? "YES" : "NO")}";

        if (current.cargo != null)
        {
            contractText.text =
                $"CONTRACT\n" +
                $"{current.origin.countryName} → {current.destination.countryName}\n" +
                $"Origin Instability: {current.origin.instability}\n" +
                $"Destination Instability: {current.destination.instability}\n" +
                $"Cargo: {current.cargo.cargoName} ({(current.cargo.isIllegal ? "Illegal" : "Legal")})\n" +
                $"Base Pay: £{current.basePay}\n" +
                $"Risk: {current.riskPercent}%\n" +
                $"Time Limit: {current.timeLimitDays} days";
        }
        else
        {
            contractText.text = "No active contract.";
        }
        
        // Upgrade button disabled if owned
        buyUpgradeButton.interactable = !hiddenCompartment;
    }
}