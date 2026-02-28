using UnityEngine;

public class ContractContextService : MonoBehaviour
{
    public ActiveContractContext Active { get; private set; }

    public void SetActive(ActiveContractContext ctx)
    {
        Active = ctx;
        Debug.Log($"[Contract] Active set: {ctx.contractId} T{ctx.tier} {ctx.origin}->{ctx.destination} illegal:{ctx.hasIllegalGoods}");
    }

    public void Clear()
    {
        Active = null;
    }

    public bool HasActive => Active != null;
}