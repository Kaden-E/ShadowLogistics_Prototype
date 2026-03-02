// CargoConfigSO.cs
// ScriptableObject. Create one asset: Assets/Configs/CargoConfig.asset
// Maps CargoSize enum values to concrete unit counts.

using UnityEngine;
using ShadowLogistics.Cargo;

namespace ShadowLogistics.Cargo
{
    [CreateAssetMenu(fileName = "CargoConfig", menuName = "ShadowLogistics/Configs/CargoConfig")]
    public class CargoConfigSO : ScriptableObject
    {
        [Header("Units per cargo size tier")]
        public int smallUnits  = 20;
        public int mediumUnits = 50;
        public int largeUnits  = 100;
        public int hugeUnits   = 180;

        /// <summary>Returns the unit count for the given CargoSize.</summary>
        public int GetUnits(CargoSize size)
        {
            return size switch
            {
                CargoSize.Small  => smallUnits,
                CargoSize.Medium => mediumUnits,
                CargoSize.Large  => largeUnits,
                CargoSize.Huge   => hugeUnits,
                _                => smallUnits
            };
        }
    }

    /// <summary>Stateless helper — use when you have both objects in scope.</summary>
    public static class CargoHelper
    {
        public static int GetCargoUnits(CargoSize size, CargoConfigSO config)
            => config.GetUnits(size);
    }
}