// CargoSize.cs
// Enum representing how large a contract's cargo load is.
// Maps to unit counts via CargoConfigSO.

namespace ShadowLogistics.Cargo
{
    public enum CargoSize
    {
        Small  = 0,
        Medium = 1,
        Large  = 2,
        Huge   = 3
    }
}