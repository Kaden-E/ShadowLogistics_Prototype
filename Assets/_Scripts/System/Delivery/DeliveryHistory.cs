using System.Collections.Generic;
using UnityEngine;

public class DeliveryHistory : MonoBehaviour
{
    public IReadOnlyList<DeliveryResult> Results => _results;
    private readonly List<DeliveryResult> _results = new();

    public void Add(DeliveryResult result)
    {
        _results.Add(result);
    }

    public DeliveryResult GetLast()
    {
        if (_results.Count == 0) return null;
        return _results[_results.Count - 1];
    }

    public List<DeliveryResult> GetLastN(int n)
    {
        n = Mathf.Clamp(n, 0, _results.Count);
        return _results.GetRange(_results.Count - n, n);
    }
}