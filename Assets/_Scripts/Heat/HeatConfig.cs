using UnityEngine;

namespace ShadowLogistics.Heat
{
    [CreateAssetMenu(menuName = "Shadow Logistics/Heat/Heat Config")]
    public class HeatConfig : ScriptableObject
    {
        [Header("Heat Range")]
        public float minHeat = 0f;
        public float maxHeat = 100f;

        [Header("Accumulation - Base Adds")]
        public float onInspectionTriggered = 2f;   // Roll A succeeds
        public float onCaughtBase = 6f;            // Roll B caught outcome
        public float onDeliveryFailed = 8f;        // Graduated failure / forced abort etc.

        [Header("Accumulation - Multipliers")]
        public float minorMult = 1.0f;
        public float moderateMult = 1.3f;
        public float majorMult = 1.7f;
        public float extremeMult = 2.2f;

        [Tooltip("Adds extra heat proportional to discovered scale (foundUnits or contraband units).")]
        public float perFoundUnit = 0.15f;

        [Header("Decay")]
        [Tooltip("Decay applied to regions traveled per completed delivery.")]
        public float decayPerDelivery = 1.25f;

        [Tooltip("Extra decay if delivery was clean (no inspection triggered).")]
        public float cleanRunBonusDecay = 1.0f;

        [Header("Enforcement Influence")]
        [Tooltip("Adds to inspection trigger probability (Roll A). Example: at max heat, +0.15 = +15%.")]
        public AnimationCurve heatToInspectionChanceBonus = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(100f, 0.15f)
        );

        [Tooltip("Chance to bump severity up one band when caught, based on heat (0..1).")]
        public AnimationCurve heatToSeverityBumpChance = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(100f, 0.35f)
        );
    }
}