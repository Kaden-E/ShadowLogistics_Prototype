// WalletService.cs
// Singleton MonoBehaviour. Place on a persistent GameObject (e.g. "Services").
// Wraps PlayerWallet and broadcasts balance changes.

using System;
using UnityEngine;

namespace ShadowLogistics.Economy
{
    public class WalletService : MonoBehaviour
    {
        // ── Singleton ─────────────────────────────────────────────────────
        public static WalletService Instance { get; private set; }

        // ── Events ────────────────────────────────────────────────────────
        /// <summary>Fires with the NEW balance after any change.</summary>
        public event Action<int> OnBalanceChanged;

        // ── State ─────────────────────────────────────────────────────────
        private PlayerWallet _wallet;

        public int Balance => _wallet.Balance;

        // ── Unity ─────────────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _wallet = new PlayerWallet(); // loads from PlayerPrefs
            Debug.Log($"[WalletService] Initialised — Balance: {_wallet.Balance}");
        }

        // ── Public API ────────────────────────────────────────────────────

        /// <summary>Credit the wallet. reason is logged for traceability.</summary>
        public void Add(int amount, string reason = null)
        {
            if (amount <= 0) return;
            _wallet.Add(amount);
            Debug.Log($"[Wallet] +{amount}  ({reason ?? "—"})  → {_wallet.Balance}");
            OnBalanceChanged?.Invoke(_wallet.Balance);
        }

        /// <summary>
        /// Debit the wallet.
        /// Returns true on success, false if insufficient funds.
        /// When insufficient, the fine is clamped to the available balance
        /// so the player goes to 0 but never negative.
        /// </summary>
        public bool TrySpend(int amount, string reason = null)
        {
            if (amount <= 0) return true;

            // Clamp: player cannot go below 0
            int actualAmount = Math.Min(amount, _wallet.Balance);
            bool success     = _wallet.TrySpend(actualAmount);

            Debug.Log(
                success
                    ? $"[Wallet] -{actualAmount}  ({reason ?? "—"})  → {_wallet.Balance}"
                    : $"[Wallet] Spend of {amount} FAILED (insufficient funds). Balance: {_wallet.Balance}"
            );

            if (success) OnBalanceChanged?.Invoke(_wallet.Balance);
            return success;
        }

        public bool CanAfford(int amount) => _wallet.CanAfford(amount);

        // ── Dev helpers ───────────────────────────────────────────────────
        [ContextMenu("Debug: Reset Wallet")]
        private void DebugReset()
        {
            _wallet.DebugReset();
            OnBalanceChanged?.Invoke(_wallet.Balance);
            Debug.Log($"[WalletService] Wallet reset to default. Balance: {_wallet.Balance}");
        }
    }
}