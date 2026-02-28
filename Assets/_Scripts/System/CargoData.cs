using UnityEngine;

[CreateAssetMenu(fileName = "NewCargo", menuName = "ShadowLogistics/Cargo")]
public class CargoData : ScriptableObject
{
    public string cargoName;
    public bool isIllegal;
    public int basePay;
}