// ActiveVehicleContext.cs
// Lightweight singleton for the player's currently active vehicle.
// No persistence in v0.7.0 — defaults to Van on each session.
// Attach to the same persistent "Services" GameObject as WalletService,
// or call ActiveVehicleContext.Instance from anywhere after first access.

using UnityEngine;
using ShadowLogistics.Vehicles;

namespace ShadowLogistics.Vehicles
{
    public class ActiveVehicleContext : MonoBehaviour
    {
        // ── Singleton ─────────────────────────────────────────────────────
        public static ActiveVehicleContext Instance { get; private set; }

        // ── Inspector ─────────────────────────────────────────────────────
        [Header("Runtime vehicle (change in-game via SetVehicle)")]
        [SerializeField] private VehicleType _currentVehicle = VehicleType.Van;

        [Header("Config")]
        [SerializeField] private VehicleConfigSO _vehicleConfig;

        // ── Public API ────────────────────────────────────────────────────
        public VehicleType CurrentVehicle => _currentVehicle;

        /// <summary>Returns the capacity of the currently active vehicle.</summary>
        public int CurrentCapacityUnits
        {
            get
            {
                if (_vehicleConfig == null)
                {
                    Debug.LogWarning("[ActiveVehicleContext] VehicleConfig not assigned — returning 120.");
                    return 120;
                }
                return _vehicleConfig.GetCapacity(_currentVehicle);
            }
        }

        public void SetVehicle(VehicleType type)
        {
            _currentVehicle = type;
            Debug.Log($"[ActiveVehicleContext] Vehicle set to {type} ({CurrentCapacityUnits} units)");
        }

        // ── Unity ─────────────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}