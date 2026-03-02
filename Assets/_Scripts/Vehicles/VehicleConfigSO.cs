// VehicleConfigSO.cs
// ScriptableObject. Create one asset: Assets/Configs/VehicleConfig.asset

using UnityEngine;
using ShadowLogistics.Vehicles;

namespace ShadowLogistics.Vehicles
{
    [CreateAssetMenu(fileName = "VehicleConfig", menuName = "ShadowLogistics/Configs/VehicleConfig")]
    public class VehicleConfigSO : ScriptableObject
    {
        [Header("Cargo capacity in units")]
        public int vanCapacity   = 120;
        public int truckCapacity = 240;

        /// <summary>Returns the capacity (units) for the given vehicle type.</summary>
        public int GetCapacity(VehicleType type)
        {
            return type switch
            {
                VehicleType.Van   => vanCapacity,
                VehicleType.Truck => truckCapacity,
                _                 => vanCapacity
            };
        }
    }

    /// <summary>Stateless helper.</summary>
    public static class VehicleHelper
    {
        public static int GetCapacity(VehicleType type, VehicleConfigSO config)
            => config.GetCapacity(type);
    }
}