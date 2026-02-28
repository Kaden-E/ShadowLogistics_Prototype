using System;
using UnityEngine;

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