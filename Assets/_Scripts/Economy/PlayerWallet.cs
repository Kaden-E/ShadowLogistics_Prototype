// PlayerWallet.cs
// Pure data model. Holds the balance and handles PlayerPrefs persistence.
// No MonoBehaviour — owned and driven by WalletService.

namespace ShadowLogistics.Economy
{
    public class PlayerWallet
    {
        // ── Constants ────────────────────────────────────────────────────────
        private const string PREFS_KEY        = "PLAYER_BALANCE";
        private const int    DEFAULT_BALANCE  = 1000; // starter fund

        // ── State ─────────────────────────────────────────────────────────
        public int Balance { get; private set; }

        // ── Init ──────────────────────────────────────────────────────────
        public PlayerWallet()
        {
            Load();
        }

        // ── Mutations ─────────────────────────────────────────────────────

        /// <summary>Add a positive amount to the balance and persist.</summary>
        public void Add(int amount)
        {
            if (amount <= 0) return;
            Balance += amount;
            Save();
        }

        /// <summary>
        /// Attempt to subtract <paramref name="amount"/> from the balance.
        /// Returns false (and does NOT mutate) if funds are insufficient.
        /// </summary>
        public bool TrySpend(int amount)
        {
            if (amount <= 0) return true;
            if (Balance < amount) return false;
            Balance -= amount;
            Save();
            return true;
        }

        public bool CanAfford(int amount) => Balance >= amount;

        // ── Persistence ───────────────────────────────────────────────────
        private void Load()
        {
            Balance = UnityEngine.PlayerPrefs.GetInt(PREFS_KEY, DEFAULT_BALANCE);
        }

        private void Save()
        {
            UnityEngine.PlayerPrefs.SetInt(PREFS_KEY, Balance);
            UnityEngine.PlayerPrefs.Save();
        }

        // ── Debug ─────────────────────────────────────────────────────────
        /// <summary>Hard-reset balance to default (dev/testing use only).</summary>
        public void DebugReset()
        {
            Balance = DEFAULT_BALANCE;
            Save();
        }
    }
}