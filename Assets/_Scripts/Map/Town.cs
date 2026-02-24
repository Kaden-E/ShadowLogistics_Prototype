using UnityEngine;
using System.Collections.Generic;

public class Town : MonoBehaviour
{
    public List<Town> connectedTowns = new List<Town>();
    public string townName = "Unnamed";
    private SpriteRenderer sr;
    private Color defaultColor;
    public enum Country { A, B }   // add more later
    public Country country;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        defaultColor = sr.color;
    }

    public void OnClicked()
    {
        TownManager.Instance.SelectTown(this);
    }

    public void SetSelected(bool value)
    {
        sr.color = value ? Color.yellow : defaultColor;
    }
}