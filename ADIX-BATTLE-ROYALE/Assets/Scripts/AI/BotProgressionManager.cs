using System;
using UnityEngine;

namespace AdixBattleRoyale.AI
{
    // Offline difficulty director. It adjusts the next match from the player's
    // recent results, while keeping a clear level progression and a fair ceiling.
    public sealed class BotProgressionManager : MonoBehaviour
    {
        private const string SavedLevelKey = "ADIX_BOT_LEVEL";
        [SerializeField] private BotDifficultyTier[] tiers;
        [SerializeField, Min(1)] private int startingLevel = 1;
        [SerializeField, Min(1)] private int maxLevel = 10;
        [SerializeField, Range(1, 10)] private int matchesPerEvaluation = 3;

        private int currentLevel;
        private int evaluatedMatches;
        private float performanceTotal;

        public int CurrentLevel => currentLevel;
        public event Action<int> LevelChanged;

        private void Awake()
        {
            currentLevel = Mathf.Clamp(
                PlayerPrefs.GetInt(SavedLevelKey, startingLevel),
                1, Mathf.Max(1, maxLevel));
        }

        public BotDifficultyTier GetCurrentTier()
        {
            if (tiers == null || tiers.Length == 0) return null;

            BotDifficultyTier selected = tiers[0];
            foreach (BotDifficultyTier tier in tiers)
            {
                if (tier != null && tier.level <= currentLevel &&
                    tier.level >= selected.level)
                    selected = tier;
            }
            return selected;
        }

        // Call once when a match ends. placement: 1 is winner; aliveSeconds
        // is the player's survival time in that match.
        public void RecordMatch(int kills, int deaths, int placement, int aliveSeconds)
        {
            float score = Mathf.Clamp(kills * 0.12f, 0f, 0.48f);
            score += placement == 1 ? 0.45f :
                     placement <= 5 ? 0.25f :
                     placement <= 15 ? 0.12f : 0f;
            score += Mathf.Clamp(aliveSeconds / 1800f, 0f, 0.2f);
            score -= Mathf.Clamp(deaths * 0.08f, 0f, 0.24f);

            performanceTotal += Mathf.Clamp(score, 0f, 1f);
            evaluatedMatches++;

            if (evaluatedMatches < matchesPerEvaluation) return;

            float average = performanceTotal / evaluatedMatches;
            int nextLevel = currentLevel;

            if (average >= 0.62f) nextLevel++;
            else if (average <= 0.22f) nextLevel--;

            nextLevel = Mathf.Clamp(nextLevel, 1, Mathf.Max(1, maxLevel));
            evaluatedMatches = 0;
            performanceTotal = 0f;

            if (nextLevel == currentLevel) return;

            currentLevel = nextLevel;
            PlayerPrefs.SetInt(SavedLevelKey, currentLevel);
            PlayerPrefs.Save();
            LevelChanged?.Invoke(currentLevel);
        }

        public void ResetProgress()
        {
            currentLevel = Mathf.Clamp(startingLevel, 1, Mathf.Max(1, maxLevel));
            evaluatedMatches = 0;
            performanceTotal = 0f;
            PlayerPrefs.SetInt(SavedLevelKey, currentLevel);
            PlayerPrefs.Save();
            LevelChanged?.Invoke(currentLevel);
        }
    }
}
