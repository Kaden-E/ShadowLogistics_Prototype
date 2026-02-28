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
    public int money = 2000;
    public int reputation = 0;
    public bool hiddenCompartment = false;

    private Contract current;

    // Simple contract data container
    private struct Contract
    {
        public string cargo;
        public string legality;
        public string origin;
        public string destination;
        public int basePay;
        public int riskPercent; // 0-100
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
    // Safety checks so we never hard-crash
    if (cargos == null || cargos.Length == 0)
    {
        UpdateUI("Assign at least 1 Cargo in the Inspector (GameController > EconomyController).");
        Debug.LogError("EconomyController.GenerateContract: cargos array is empty/null.");
        return;
    }

    if (countries == null || countries.Length < 2)
    {
        UpdateUI("Assign at least 2 Countries in the Inspector (GameController > EconomyController).");
        Debug.LogError("EconomyController.GenerateContract: countries array needs 2+ entries.");
        return;
    }

    // Pick a valid cargo
    CargoData randomCargo = null;
    for (int i = 0; i < 20 && randomCargo == null; i++)
        randomCargo = cargos[Random.Range(0, cargos.Length)];

    if (randomCargo == null)
    {
        UpdateUI("One or more Cargo slots are None. Fix Inspector references.");
        Debug.LogError("EconomyController.GenerateContract: all cargo entries were null.");
        return;
    }

    // Pick a valid origin
    CountryData origin = null;
    for (int i = 0; i < 20 && origin == null; i++)
        origin = countries[Random.Range(0, countries.Length)];

    if (origin == null)
    {
        UpdateUI("One or more Country slots are None. Fix Inspector references.");
        Debug.LogError("EconomyController.GenerateContract: all country entries were null (origin).");
        return;
    }

    // Pick a destination that is different from origin
    CountryData destination = origin;
    int safety = 0;
    while (destination == origin && safety++ < 50)
        destination = countries[Random.Range(0, countries.Length)];

    if (destination == null || destination == origin)
    {
        UpdateUI("Could not pick a valid destination. Ensure you have 2+ distinct Countries assigned.");
        Debug.LogError("EconomyController.GenerateContract: destination invalid.");
        return;
    }

    // Fill contract fields
    current = new Contract();
    current.cargo = randomCargo.cargoName;
    current.legality = randomCargo.isIllegal ? "Illegal" : "Legal";
    current.basePay = randomCargo.basePay;
    current.origin = origin.countryName;
    current.destination = destination.countryName;
    current.timeLimitDays = Random.Range(1, 4);

    // Risk calculation
    int risk = randomCargo.isIllegal ? 45 : 20;

    // Country instability adds risk
    risk += origin.instability;
    risk += destination.instability;

    // Reputation makes you a slightly bigger target
    risk += Mathf.Clamp(reputation / 5, 0, 10);

    // Upgrade reduces risk on illegal cargo
    if (hiddenCompartment && randomCargo.isIllegal)
        risk -= 12;

    // Clamp risk to sensible bounds
    current.riskPercent = Mathf.Clamp(risk, 5, 90);

    // Enable running the contract + refresh UI
    runButton.interactable = true;
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

    private void RunShipment()
    {
        if (string.IsNullOrEmpty(current.cargo))
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
            reputation += (current.legality == "Illegal") ? 2 : 1;

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
                // Full confiscation (no pay)
                reputation = Mathf.Max(0, reputation - 2);
                UpdateUI($"CONFISCATED. No payout.");
            }
            else
            {
                // Arrest/fine
                int fine = 1200;
                money -= fine;
                reputation = Mathf.Max(0, reputation - 4);
                UpdateUI($"ARRESTED. Fine: £{fine}.");
            }
        }

        // Clear contract
        current = default;
        runButton.interactable = false;
    }

    private int CalculatePayout(Contract c)
    {
        // Base × distance-ish modifier × risk tier
        float distanceMod = (c.origin == "Country A" && c.destination == "Country B") ||
                            (c.origin == "Country B" && c.destination == "Country A") ? 1.05f :
                            (c.origin == "Country B" && c.destination == "Country C") ||
                            (c.origin == "Country C" && c.destination == "Country B") ? 1.10f :
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

        if (!string.IsNullOrEmpty(current.cargo))
        {
            contractText.text =
                $"CONTRACT\n" +
                $"{current.origin} → {current.destination}\n" +
                $"Cargo: {current.cargo} ({current.legality})\n" +
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