// InspectionSeverityConfigSO.cs
// ScriptableObject. Create one asset: Assets/Configs/InspectionSeverityConfig.asset

using UnityEngine;
using ShadowLogistics.Inspection;

namespace ShadowLogistics.Inspection
{
    [CreateAssetMenu(
        fileName = "InspectionSeverityConfig",
        menuName  = "ShadowLogistics/Configs/InspectionSeverityConfig")]
    public class InspectionSeverityConfigSO : ScriptableObject
    {
        [Header("Band thresholds (inclusive lower bound)")]
        [Tooltip("foundUnits below this value → Minor")]
        public int moderateThreshold = 20;   // Minor  = [0,  19]

        [Tooltip("foundUnits below this value → Moderate")]
        public int majorThreshold    = 60;   // Moderate = [20, 59]

        [Tooltip("foundUnits below this value → Major")]
        public int extremeThreshold  = 120;  // Major    = [60,119]
        // Extreme  = [120, ∞)

        [Header("Fine")]
        [Tooltip("Fine charged per found unit (e.g. 5 = $5 per unit found)")]
        public int finePerUnit = 5;

        /// <summary>Resolve a found-units count into its severity band.</summary>
        public InspectionSeverityBand ResolveBand(int foundUnits)
        {
            if (foundUnits < moderateThreshold) return InspectionSeverityBand.Minor;
            if (foundUnits < majorThreshold)    return InspectionSeverityBand.Moderate;
            if (foundUnits < extremeThreshold)  return InspectionSeverityBand.Major;
            return InspectionSeverityBand.Extreme;
        }
    }
}