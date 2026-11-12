using UnityEngine;
using AdixBattleRoyale.Combat;

namespace AdixBattleRoyale.Match
{
    /// <summary>Place at the center of the playable island and assign the player health.</summary>
    public sealed class SafeZoneController : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField, Min(10f)] private float startingRadius = 800f;
        [SerializeField, Min(10f)] private float finalRadius = 90f;
        [SerializeField, Min(1f)] private float shrinkDuration = 900f;
        [SerializeField, Min(0f)] private float damagePerSecond = 5f;

        public float CurrentRadius { get; private set; }
        private float elapsed;

        private void Start() => CurrentRadius = startingRadius;

        private void Update()
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / shrinkDuration);
            CurrentRadius = Mathf.Lerp(startingRadius, finalRadius, t);

            if (player == null || playerHealth == null || !playerHealth.IsAlive) return;
            Vector3 offset = player.position - transform.position;
            offset.y = 0f;
            if (offset.sqrMagnitude > CurrentRadius * CurrentRadius)
                playerHealth.TakeDamage(damagePerSecond * Time.deltaTime);
        }

        public float GetRemainingShrinkFraction()
        {
            return 1f - Mathf.Clamp01(elapsed / shrinkDuration);
        }
    }
}