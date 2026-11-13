using System;
using UnityEngine;

namespace AdixBattleRoyale.ClashNexus
{
    public enum ClashTeam { Orion, Aether }
    public enum ClashPhase { TeamSetup, BuyPhase, RoundActive, RoundResult, MatchComplete }
    public enum ClashWeaponClass { Sidearm, SMG, AssaultRifle, Shotgun, Marksman, Sniper }

    [Serializable] public sealed class ClashRosterSlot
    {
        public string callsign = "OPEN SLOT";
        public bool isReady;
        public bool isBot;
        public bool isOccupied;
        public int eliminations;
    }

    /// <summary>Offline 4v4 Clash Nexus match-flow foundation. Network play is not implied.</summary>
    public sealed class ClashNexusMatchDirector : MonoBehaviour
    {
        [Header("Match Rules")]
        [SerializeField, Range(1, 13)] private int roundsToWin = 7;
        [SerializeField, Min(0)] private int startingCredits = 800;
        [SerializeField, Min(0)] private int roundWinReward = 2500;
        [SerializeField, Min(0)] private int roundLossReward = 1200;
        [Header("Team Orion")] [SerializeField] private ClashRosterSlot[] orion = new ClashRosterSlot[4];
        [Header("Team Aether")] [SerializeField] private ClashRosterSlot[] aether = new ClashRosterSlot[4];
        [Header("Runtime")] [SerializeField] private ClashPhase phase = ClashPhase.TeamSetup;
        [SerializeField] private int orionRoundWins, aetherRoundWins, currentRound = 1;
        [SerializeField] private int orionCredits, aetherCredits;

        public ClashPhase Phase => phase;
        public int CurrentRound => currentRound;
        public int OrionRoundWins => orionRoundWins;
        public int AetherRoundWins => aetherRoundWins;
        public int OrionCredits => orionCredits;
        public int AetherCredits => aetherCredits;
        public event Action<ClashPhase> PhaseChanged;
        public event Action<int, int> ScoreChanged;

        private void Awake()
        {
            EnsureRosters();
            orionCredits = startingCredits;
            aetherCredits = startingCredits;
        }

        private void EnsureRosters()
        {
            if (orion == null || orion.Length != 4) orion = CreateRoster("ORI");
            if (aether == null || aether.Length != 4) aether = CreateRoster("AET");
            FillOfflineBotSlots(orion, "ORI");
            FillOfflineBotSlots(aether, "AET");
        }

        private static ClashRosterSlot[] CreateRoster(string prefix)
        {
            var slots = new ClashRosterSlot[4];
            for (int i = 0; i < 4; i++) slots[i] = new ClashRosterSlot { callsign = prefix + "-" + (i + 1) };
            return slots;
        }

        private static void FillOfflineBotSlots(ClashRosterSlot[] roster, string prefix)
        {
            for (int i = 0; i < 4; i++)
            {
                if (roster[i] == null) roster[i] = new ClashRosterSlot();
                if (string.IsNullOrWhiteSpace(roster[i].callsign)) roster[i].callsign = prefix + "-" + (i + 1);
                if (!roster[i].isOccupied)
                {
                    roster[i].isOccupied = true;
                    roster[i].isBot = true;
                    roster[i].isReady = true;
                }
            }
        }

        public ClashRosterSlot[] GetRoster(ClashTeam team) => team == ClashTeam.Orion ? orion : aether;

        public bool SetPlayerSlot(ClashTeam team, int slot, string callsign)
        {
            if (phase != ClashPhase.TeamSetup || slot < 0 || slot >= 4 || string.IsNullOrWhiteSpace(callsign)) return false;
            ClashRosterSlot entry = GetRoster(team)[slot];
            entry.callsign = callsign.Trim(); entry.isOccupied = true; entry.isBot = false; entry.isReady = false;
            return true;
        }

        public bool SetReady(ClashTeam team, int slot, bool ready)
        {
            if (phase != ClashPhase.TeamSetup || slot < 0 || slot >= 4) return false;
            ClashRosterSlot entry = GetRoster(team)[slot];
            if (!entry.isOccupied) return false;
            entry.isReady = ready; return true;
        }

        public bool StartBuyPhase()
        {
            if (phase != ClashPhase.TeamSetup && phase != ClashPhase.RoundResult) return false;
            if (!AllReady(orion) || !AllReady(aether)) return false;
            SetPhase(ClashPhase.BuyPhase); return true;
        }

        public bool Purchase(ClashTeam team, ClashWeaponClass weapon, int cost)
        {
            if (phase != ClashPhase.BuyPhase || cost < 0) return false;
            if (team == ClashTeam.Orion)
            { if (orionCredits < cost) return false; orionCredits -= cost; }
            else
            { if (aetherCredits < cost) return false; aetherCredits -= cost; }
            return true;
        }

        public bool StartRound()
        { if (phase != ClashPhase.BuyPhase) return false; SetPhase(ClashPhase.RoundActive); return true; }

        public void ResolveRound(ClashTeam winner)
        {
            if (phase != ClashPhase.RoundActive) return;
            if (winner == ClashTeam.Orion) { orionRoundWins++; orionCredits += roundWinReward; aetherCredits += roundLossReward; }
            else { aetherRoundWins++; aetherCredits += roundWinReward; orionCredits += roundLossReward; }
            ScoreChanged?.Invoke(orionRoundWins, aetherRoundWins);
            if (orionRoundWins >= roundsToWin || aetherRoundWins >= roundsToWin) { SetPhase(ClashPhase.MatchComplete); return; }
            currentRound++; SetPhase(ClashPhase.RoundResult);
        }

        public void ResetMatch()
        {
            orionRoundWins = 0; aetherRoundWins = 0; currentRound = 1;
            orionCredits = startingCredits; aetherCredits = startingCredits;
            SetPhase(ClashPhase.TeamSetup);
        }

        private static bool AllReady(ClashRosterSlot[] roster)
        {
            if (roster == null || roster.Length < 4) return false;
            for (int i = 0; i < 4; i++) if (roster[i] == null || !roster[i].isOccupied || !roster[i].isReady) return false;
            return true;
        }

        private void SetPhase(ClashPhase next) { phase = next; PhaseChanged?.Invoke(phase); }
    }
}