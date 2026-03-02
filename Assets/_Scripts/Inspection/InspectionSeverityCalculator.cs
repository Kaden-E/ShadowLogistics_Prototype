// InspectionSeverityCalculator.cs
// Pure static calculator — no MonoBehaviour, no Unity lifecycle.
// All inputs come from other systems; this just does the math.

using UnityEngine;
using ShadowLogistics.Inspection;

namespace ShadowLogistics.Inspection
{
    // ── Result ────────────────────────────────────────────────────────────────
    public readonly struct InspectionResult
    {
        /// <summary>How many cargo units were actually "found" by inspectors.</summary>
        public readonly int foundUnits;

        /// <summary>The severity category derived from foundUnits.</summary>
        public readonly InspectionSeverityBand band;

        /// <summary>Total fine = foundUnits * finePerUnit.</summary>
        public readonly int fineAmount;

        public InspectionResult(int foundUnits, InspectionSeverityBand band, int fineAmount)
        {
            this.foundUnits = foundUnits;
            this.band       = band;
            this.fineAmount = fineAmount;
        }

        public override string ToString() =>
            $"[Inspection] Found={foundUnits}  Band={band}  Fine={fineAmount}";
    }

    // ── Calculator ────────────────────────────────────────────────────────────
    public static class InspectionSeverityCalculator
    {
        /// <summary>
        /// Compute how much contraband was "found" and what fine applies.
        /// </summary>
        /// <param name="cargoUnitsCarried">Units the player was carrying (from contract + CargoConfig).</param>
        /// <param name="vehicleCapacityUnits">Max capacity of the active vehicle.</param>
        /// <param name="config">InspectionSeverityConfigSO asset.</param>
        public static InspectionResult Calculate(
            int cargoUnitsCarried,
            int vehicleCapacityUnits,
            InspectionSeverityConfigSO config)
        {
            // Guard: avoid divide-by-zero; treat empty vehicle as fully loaded
            if (vehicleCapacityUnits <= 0) vehicleCapacityUnits = cargoUnitsCarried;

            // 1. How full is the vehicle? (0.0 → 1.0)
            float loadRatio = Mathf.Clamp01(cargoUnitsCarried / (float)vehicleCapacityUnits);

            // 2. Inspectors find between 20 % and 85 % of what's loaded,
            //    scaling linearly with how stuffed the vehicle is.
            //    Empty van: 20 % found.  Fully packed: 85 % found.
            float foundPercent = Mathf.Clamp(0.20f + 0.60f * loadRatio, 0.20f, 0.85f);

            // 3. Actual units found (always ≤ what was carried)
            int foundUnits = Mathf.Clamp(
                Mathf.RoundToInt(cargoUnitsCarried * foundPercent),
                0,
                cargoUnitsCarried);

            // 4. Derive band and fine
            InspectionSeverityBand band = config.ResolveBand(foundUnits);
            int fineAmount              = foundUnits * config.finePerUnit;

            return new InspectionResult(foundUnits, band, fineAmount);
        }
    }
}