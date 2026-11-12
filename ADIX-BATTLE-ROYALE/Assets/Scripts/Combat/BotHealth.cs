using UnityEngine;
using AdixBattleRoyale.Match;

namespace AdixBattleRoyale.Combat
{
    public sealed class BotHealth : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float baseHealth = 100f;
        [SerializeField] private BattleRoyaleMatchDirector matchDirector;
        private float currentHealth;
        public float CurrentHealth => currentHealth;

        private void Awake() => currentHealth = baseHealth;

        public void ApplyDifficultyHealth(float multiplier)
        {
            float safeMultiplier = Mathf.Max(0.1f, multiplier);
            currentHealth = baseHealth * safeMultiplier;
        }

        public void TakeDamage(float amount)
        {
            if (currentHealth <= 0f || amount <= 0f) return;
            currentHealth -= amount;
            if (currentHealth <= 0f)
            {
                currentHealth = 0f;
                if (matchDirector != null) matchDirector.NotifyBotEliminated(gameObject);
                else Destroy(gameObject);
            }
        }
    }
}
