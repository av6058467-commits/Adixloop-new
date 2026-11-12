using UnityEngine;
using AdixBattleRoyale.Match;

namespace AdixBattleRoyale.Combat
{
    // Implements the same damage contract used by the player's weapon.
    public sealed class BotHealth : MonoBehaviour, IDamageable
    {
        [SerializeField, Min(1f)] private float baseHealth = 100f;
        [SerializeField] private BattleRoyaleMatchDirector matchDirector;
        private float currentHealth;
        private bool eliminated;

        public float CurrentHealth => currentHealth;

        private void Awake() => currentHealth = baseHealth;

        public void SetMatchDirector(BattleRoyaleMatchDirector director)
        {
            matchDirector = director;
        }

        public void ApplyDifficultyHealth(float multiplier)
        {
            currentHealth = baseHealth * Mathf.Max(0.1f, multiplier);
        }

        public void TakeDamage(float amount)
        {
            if (eliminated || amount <= 0f) return;
            currentHealth = Mathf.Max(0f, currentHealth - amount);
            if (currentHealth > 0f) return;

            eliminated = true;
            if (matchDirector != null) matchDirector.NotifyBotEliminated(gameObject);
            else Destroy(gameObject);
        }
    }
}