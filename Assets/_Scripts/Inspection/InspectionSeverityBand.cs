// InspectionSeverityBand.cs

namespace ShadowLogistics.Inspection
{
    public enum InspectionSeverityBand
    {
        Minor    = 0,   // foundUnits < 20
        Moderate = 1,   // foundUnits 20–59
        Major    = 2,   // foundUnits 60–119
        Extreme  = 3    // foundUnits >= 120
    }
}