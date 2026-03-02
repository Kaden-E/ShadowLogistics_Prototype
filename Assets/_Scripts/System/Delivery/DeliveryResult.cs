using System;
using UnityEngine;
using ShadowLogistics.Inspection;

[Serializable]
public class DeliveryResult
{
    public string contractId;
    public string origin;
    public string destination;
    public int tier;

    public bool success;

    // Border/inspection flags
    public bool wasInspected;
    public bool illegalFound;
    public bool bribeUsed;
    
    // v0.7.0 inspection severity
    public bool wasCaught;
    public int foundUnits;
    public InspectionSeverityBand severityBand;
    public int fineAmount;

    // World state snapshot
    public int instabilityAtStart;
    public int riskAtStart;

    // Economy placeholders (even if not used yet)
    public int payout;
    public int penalty;
    public int heatChange;

    // Timing / meta
    public float timeTakenSeconds;
    public long unixTimeUtc;

    public DeliveryResult()
    {
        unixTimeUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}