using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShadowLogistics.Cargo;
using ShadowLogistics.Inspection;
using ShadowLogistics.Vehicles;
using ShadowLogistics.Heat;


public partial class RouteManager : MonoBehaviour
{
    private ContractContextService contractContext;
    private DeliveryOutcomeService deliveryOutcome;
    private bool wasInspectedThisRun;
    private bool illegalFoundThisRun;
    private bool bribeUsedThisRun;
    private bool runFailed;
    private string _lastHeatRegionId;
    public static string CurrentHeatRegionId { get; private set; }
    
    
    private float runStartTime;
    
    [SerializeField] private CargoConfigSO _cargoConfig;
    [SerializeField] private InspectionSeverityConfigSO _inspectConfig;
    
    private int _runFoundUnits;
    private InspectionSeverityBand _runSeverityBand;
    private int _runFineAmount;
    private bool _wasCaughtThisRun;
    
    
    [Header("Debug")]
    public bool forceInspection = false;
    
    public static RouteManager Instance;

    public GameObject truckPrefab;
    
    private void Start()
    {
        deliveryOutcome = FindFirstObjectByType<DeliveryOutcomeService>();
        if (deliveryOutcome == null)
            Debug.LogError("No DeliveryOutcomeService found. Is Core in the scene?");
        
        contractContext = FindFirstObjectByType<ContractContextService>();
        if (contractContext == null)
            Debug.LogWarning("No ContractContextService found. Contract data will be defaulted.");
    }
    
    void Awake()
    {
        Instance = this;
    }
    
    public void DispatchRoute(List<Town> route)
    {
        var active = (contractContext != null && contractContext.HasActive)
            ? contractContext.Active
            : null;

        if (active != null)
        {
            if (route == null || route.Count == 0)
            {
                Debug.Log("Route must start at contract origin.");

                if (ConnectionManager.Instance != null)
                    ConnectionManager.Instance.HideAll();

                return;
            }

            if (route[0].name != active.origin)
            {
                Debug.Log("Route must start at contract origin.");

                if (ConnectionManager.Instance != null)
                    ConnectionManager.Instance.HideAll();

                return;
            }
        }

        List<Town> routeCopy = new List<Town>(route);
        StartCoroutine(MoveTruckAlongRoute(routeCopy));

        if (ConnectionManager.Instance != null)
            ConnectionManager.Instance.ShowRoute(routeCopy);
    }
private IEnumerator MoveTruckAlongRoute(List<Town> route)
{
    if (route == null || route.Count < 2)
        yield break;

    wasInspectedThisRun = false;
    illegalFoundThisRun = false;
    bribeUsedThisRun = false;
    runFailed = false;
    runStartTime = Time.time;
    _runFoundUnits = 0;
    _runFineAmount = 0;
    _wasCaughtThisRun = false;
    _runSeverityBand = default; // don’t assume “None” exists yet

    GameObject truck = Instantiate(truckPrefab, route[0].transform.position, Quaternion.identity);

    float baseSpeed = 2f;

    for (int i = 0; i < route.Count - 1; i++)
    {
        Town a = route[i];
        Town b = route[i + 1];

        // Segment speed: if either endpoint is a BorderNode, apply its multiplier
        float multiplier = 1f;
        if (a is BorderNode ba) multiplier *= ba.speedMultiplier;
        if (b is BorderNode bb) multiplier *= bb.speedMultiplier;

        float speed = baseSpeed * multiplier;

        yield return StartCoroutine(MoveTruckTo(truck, b.transform.position, speed));

        if (b is BorderNode border)
        {
            yield return StartCoroutine(HandleBorderInspection(border));

            if (runFailed)
            {
                Debug.Log("Run terminated due to inspection.");
                break;
            }
        }
    }

    if (ConnectionManager.Instance != null)
        ConnectionManager.Instance.HideAll();

    var active = (contractContext != null && contractContext.HasActive)
        ? contractContext.Active
        : null;

    string finalNodeName = route[route.Count - 1].name;
    bool wrongDestination = false;

    if (active != null)
    {
        wrongDestination = finalNodeName != active.destination;

        if (wrongDestination)
        {
            runFailed = true;
            Debug.Log("Delivery failed: wrong destination.");
        }
    }

    bool success = !runFailed;

    if (deliveryOutcome != null)
    {
        if (string.IsNullOrEmpty(_lastHeatRegionId))
            _lastHeatRegionId = active != null ? active.origin : route[0].name;

        var result = new DeliveryResult
        {
            contractId = active != null ? active.contractId : "route_test",

            origin = active != null ? active.origin : route[0].name,
            destination = active != null ? active.destination : finalNodeName,
            finalNode = finalNodeName,

            tier = active != null ? active.tier : 1,

            success = success,
            failureReason = wrongDestination
                ? DeliveryFailureReason.WrongDestination
                : DeliveryFailureReason.None,

            wasInspected = wasInspectedThisRun,
            illegalFound = illegalFoundThisRun,
            bribeUsed = bribeUsedThisRun,
            wasCaught = _wasCaughtThisRun,
            foundUnits = _runFoundUnits,
            severityBand = _runSeverityBand,
            fineAmount = _runFineAmount,
            regionId = _lastHeatRegionId,

            instabilityAtStart = 0,
            riskAtStart = active != null ? active.riskPercent : 0,

            payout = success && active != null ? active.payout : 0,
            penalty = _runFineAmount,

            heatChange = illegalFoundThisRun ? 5 : (wasInspectedThisRun ? 1 : 0),

            timeTakenSeconds = Time.time - runStartTime
        };

        deliveryOutcome.CompleteDelivery(result);
    }

    Destroy(truck);
}

    
    private IEnumerator MoveTruckTo(GameObject truck, Vector3 target, float speed)
    {
        while (Vector3.Distance(truck.transform.position, target) > 0.01f)
        {
            truck.transform.position = Vector3.MoveTowards(
                truck.transform.position,
                target,
                speed * Time.deltaTime
            );

            Vector3 dir = (target - truck.transform.position).normalized;
            if (dir.sqrMagnitude > 0.0001f)
                truck.transform.up = dir;

            yield return null;
        }

        truck.transform.position = target;
    }
    private IEnumerator FlashBorder(BorderNode border, float duration)
    {
        SpriteRenderer sr = border.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Color originalColor = sr.color;
        Vector3 originalScale = border.transform.localScale;

        float timer = 0f;

        while (timer < duration)
        {
            sr.color = Color.red;
            border.transform.localScale = originalScale * 1.15f;

            yield return new WaitForSeconds(0.15f);

            sr.color = originalColor;
            border.transform.localScale = originalScale;

            yield return new WaitForSeconds(0.15f);

            timer += 0.3f;
        }

        sr.color = originalColor;
        border.transform.localScale = originalScale;
    }
    private IEnumerator HandleBorderInspection(BorderNode border)
    { 
        
    _lastHeatRegionId = border.townName;
    CurrentHeatRegionId = _lastHeatRegionId;
        
    yield return new WaitForSeconds(0.25f);

    // Stage 1: inspected?
// Prefer: border.regionId / border.countryId / border.regionName if it exists.

    float inspectionChance = border.inspectionChance;
    if (HeatService.Instance != null)
        inspectionChance = HeatService.Instance.ModifyInspectionChance(_lastHeatRegionId, inspectionChance);
    Debug.Log($"[HEAT] {_lastHeatRegionId} heat={HeatService.Instance.GetHeat(_lastHeatRegionId):0} rollAChance={inspectionChance:0.00}");

    bool inspected = forceInspection || Random.value < inspectionChance;
    if (!inspected)
        yield break;

    wasInspectedThisRun = true;

    if (HeatService.Instance != null)
        HeatService.Instance.OnInspectionTriggered(_lastHeatRegionId);

    Debug.Log($"INSPECTION at {border.townName} ({border.type})");

    // Visual feedback
    yield return StartCoroutine(FlashBorder(border, border.inspectionDelay));

    if (border.inspectionDelay > 0f)
        yield return new WaitForSeconds(0.5f);

    // -------------------------------
    // Stage 2: caught? + severity (v0.7.0)
    // -------------------------------

    bool carryingIllegalGoods =
        contractContext != null &&
        contractContext.HasActive &&
        contractContext.Active.hasIllegalGoods;

    // If nothing illegal, inspection ends here
    if (!carryingIllegalGoods)
        yield break;

    // Roll B: caught?
    float caughtChance = (contractContext.Active.riskPercent / 100f);
    bool caught = Random.value < caughtChance;

    if (!caught)
        yield break;

    _wasCaughtThisRun = true;

    // Convert CargoSize -> units (requires ActiveContractContext.cargoSize)
    int cargoUnits = 0;
    if (_cargoConfig != null && contractContext != null && contractContext.HasActive)
    {
        cargoUnits = _cargoConfig.GetUnits(contractContext.Active.cargoSize);
    }

    // Vehicle capacity from ActiveVehicleContext
    int vehicleCapacity = 0;
    if (ActiveVehicleContext.Instance != null)
    {
        vehicleCapacity = ActiveVehicleContext.Instance.CurrentCapacityUnits;
    }

    // Calculate severity + found units + fine (static, 3 params)
    var inspectionResult = InspectionSeverityCalculator.Calculate(
        cargoUnits,
        vehicleCapacity,
        _inspectConfig
    );

    _runFoundUnits = inspectionResult.foundUnits;
    _runSeverityBand = inspectionResult.band;
    _runFineAmount = inspectionResult.fineAmount;

// --- HEAT: severity bias ---

        if (HeatService.Instance != null)
    {
        _runSeverityBand = HeatService.Instance.BiasSeverity(_lastHeatRegionId, _runSeverityBand);
    }

// --- HEAT: caught spike ---
    if (HeatService.Instance != null)
        
        illegalFoundThisRun = (_runFoundUnits > 0);

    Debug.Log($"CAUGHT: FoundUnits={_runFoundUnits}, Band={_runSeverityBand}, Fine={_runFineAmount}");

    // Outcome rule (v0.8.0)
    if (_runSeverityBand == InspectionSeverityBand.Major ||
        _runSeverityBand == InspectionSeverityBand.Extreme)
    {
        runFailed = true;

        if (HeatService.Instance != null)
            HeatService.Instance.OnDeliveryFailed(_lastHeatRegionId, _runSeverityBand);

        Debug.Log("Caught with Major/Extreme severity. Delivery failed.");
        yield break;
    }

    // --- Bribe logic placeholder (future expansion) ---
    bool attemptBribe = false; // hook into UI later

    if (attemptBribe)
    {
        bribeUsedThisRun = true;

        bool bribeSuccess = Random.value < 0.7f; // 70% success chance

        if (!bribeSuccess)
        {
            illegalFoundThisRun = true;
            runFailed = true;

            Debug.Log("Bribe failed! Delivery failed.");
            yield break;
        }

        Debug.Log("Bribe succeeded.");
    }
}
}