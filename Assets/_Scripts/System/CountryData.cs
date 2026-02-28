using UnityEngine;

[CreateAssetMenu(fileName = "NewCountry", menuName = "ShadowLogistics/Country")]
public class CountryData : ScriptableObject
{
    public string countryName;
    [Range(0, 20)]
    public int instability; // affects risk
}