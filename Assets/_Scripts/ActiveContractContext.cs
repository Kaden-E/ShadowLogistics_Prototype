using System;

[Serializable]
public class ActiveContractContext
{
    public string contractId;
    public string origin;
    public string destination;

    public int tier;

    public bool hasIllegalGoods;

    public int payout;
    public int penalty;

    // 0–100
    public int riskPercent;
}