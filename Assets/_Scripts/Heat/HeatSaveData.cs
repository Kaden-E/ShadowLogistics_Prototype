using System;
using System.Collections.Generic;

namespace ShadowLogistics.Heat
{
    [Serializable]
    public class HeatSaveData
    {
        public List<string> regionIds = new List<string>();
        public List<float> heatValues = new List<float>();
    }
}