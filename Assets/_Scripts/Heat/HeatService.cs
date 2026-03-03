using System;
using System.Collections.Generic;
using UnityEngine;
using ShadowLogistics.Inspection;

namespace ShadowLogistics.Heat
{
    public class HeatService : MonoBehaviour
    {
        public static HeatService Instance { get; private set; }

        [SerializeField] private HeatConfig config;

        // regionId -> heat
        private readonly Dictionary<string, float> _heatByRegion = new();

        private const string SaveKey = "ShadowLogistics.Heat.v1";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Load();
        }

        public float GetHeat(string regionId)
        {
            if (string.IsNullOrEmpty(regionId)) return 0f;
            return _heatByRegion.TryGetValue(regionId, out var h) ? h : 0f;
        }

        public float GetHeat01(string regionId)
        {
            float h = GetHeat(regionId);
            return Mathf.InverseLerp(config.minHeat, config.maxHeat, h);
        }

        public void AddHeat(string regionId, float amount)
        {
            if (string.IsNullOrEmpty(regionId)) return;

            float current = GetHeat(regionId);
            float next = Mathf.Clamp(current + amount, config.minHeat, config.maxHeat);
            _heatByRegion[regionId] = next;

            Save();
        }

        public void DecayHeat(string regionId, float amount)
        {
            AddHeat(regionId, -Mathf.Abs(amount));
        }

        // --- Accumulation events ---
        public void OnInspectionTriggered(string regionId)
        {
            AddHeat(regionId, config.onInspectionTriggered);
        }

        public void OnCaught(string regionId, InspectionSeverityBand band, int foundUnits)
        {
            float mult = GetSeverityMult(band);
            float extra = Mathf.Max(0, foundUnits) * config.perFoundUnit;
            AddHeat(regionId, (config.onCaughtBase + extra) * mult);
        }

        public void OnDeliveryFailed(string regionId, InspectionSeverityBand band)
        {
            float mult = GetSeverityMult(band);
            AddHeat(regionId, config.onDeliveryFailed * mult);
        }

        public void OnDeliveryCompleted(string regionId, bool cleanRun)
        {
            float decay = config.decayPerDelivery + (cleanRun ? config.cleanRunBonusDecay : 0f);
            DecayHeat(regionId, decay);
        }

        // --- Enforcement influence ---
        public float ModifyInspectionChance(string regionId, float baseChance01)
        {
            float heat = GetHeat(regionId);
            float bonus = config.heatToInspectionChanceBonus.Evaluate(heat);
            return Mathf.Clamp01(baseChance01 + bonus);
        }

        // Simple “bump severity up a band” hook when caught
        public InspectionSeverityBand BiasSeverity(string regionId, InspectionSeverityBand currentBand)
        {
            float heat = GetHeat(regionId);
            float bumpChance = Mathf.Clamp01(config.heatToSeverityBumpChance.Evaluate(heat));

            if (UnityEngine.Random.value <= bumpChance)
                return BumpUp(currentBand);

            return currentBand;
        }

        // --- Persistence ---
        public void Save()
        {
            var data = new HeatSaveData();
            foreach (var kvp in _heatByRegion)
            {
                data.regionIds.Add(kvp.Key);
                data.heatValues.Add(kvp.Value);
            }

            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }

        public void Load()
        {
            _heatByRegion.Clear();

            if (!PlayerPrefs.HasKey(SaveKey))
                return;

            string json = PlayerPrefs.GetString(SaveKey);
            if (string.IsNullOrEmpty(json))
                return;

            try
            {
                var data = JsonUtility.FromJson<HeatSaveData>(json);
                if (data == null) return;

                int count = Mathf.Min(data.regionIds.Count, data.heatValues.Count);
                for (int i = 0; i < count; i++)
                {
                    string region = data.regionIds[i];
                    float heat = Mathf.Clamp(data.heatValues[i], config.minHeat, config.maxHeat);
                    if (!string.IsNullOrEmpty(region))
                        _heatByRegion[region] = heat;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"HeatService Load failed: {e.Message}");
            }
        }

        private float GetSeverityMult(InspectionSeverityBand band)
        {
            return band switch
            {
                InspectionSeverityBand.Minor => config.minorMult,
                InspectionSeverityBand.Moderate => config.moderateMult,
                InspectionSeverityBand.Major => config.majorMult,
                InspectionSeverityBand.Extreme => config.extremeMult,
                _ => 1f
            };
        }

        private InspectionSeverityBand BumpUp(InspectionSeverityBand band)
        {
            return band switch
            {
                InspectionSeverityBand.Minor => InspectionSeverityBand.Moderate,
                InspectionSeverityBand.Moderate => InspectionSeverityBand.Major,
                InspectionSeverityBand.Major => InspectionSeverityBand.Extreme,
                InspectionSeverityBand.Extreme => InspectionSeverityBand.Extreme,
                _ => band
            };
        }
    }
    
}