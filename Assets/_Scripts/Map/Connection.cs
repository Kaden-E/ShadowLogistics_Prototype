using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Connection : MonoBehaviour
{
    public Town from;
    public Town to;

    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    public bool Matches(Town a, Town b)
    {
        return (from == a && to == b) || (from == b && to == a);
    }

    public void SetVisible(bool visible)
    {
        if (lr == null) lr = GetComponent<LineRenderer>();
        lr.enabled = visible;
    }
}