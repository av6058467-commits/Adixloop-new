using System;
using UnityEngine;
using AdixBattleRoyale.Combat;

namespace AdixBattleRoyale.World
{
    /// <summary>Offline shrinking safe zone. Assign the player and optional ring visual.</summary>
    public sealed class SafeZoneController : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Transform ringVisual;
        [SerializeField, Min(10f)] private float startRadius = 650f;
        [SerializeField, Min(5f)] private float finalRadius = 35f;
        [SerializeField, Min(1f)] private float waitBeforeShrink = 90f;
        [SerializeField, Min(1f)] private float shrinkDuration = 240f;
        [SerializeField, Min(0f)] private float outsideDamagePerSecond = 4f;
        [SerializeField] private Vector3 zoneCenter;
        private float elapsed;
        private float damageAccumulator;
        private float currentRadius;

        public float CurrentRadius => currentRadius;
        public float RemainingTime => Mathf.Max(0f, waitBeforeShrink + shrinkDuration - elapsed);
        public event Action<float> RadiusChanged;

        private void Awake()
        {
            currentRadius = startRadius;
            ApplyRingScale();
        }

        private void Update()
        {
            if (player == null) return;
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01((elapsed - waitBeforeShrink) / shrinkDuration);
            float desiredRadius = Mathf.Lerp(startRadius, finalRadius, progress);
            if (!Mathf.Approximately(desiredRadius, currentRadius))
            {
                currentRadius = desiredRadius;
                ApplyRingScale();
                RadiusChanged?.Invoke(currentRadius);
            }

            Vector3 flatPlayer = player.position;
            flatPlayer.y = zoneCenter.y;
            bool outside = Vector3.Distance(flatPlayer, zoneCenter) > currentRadius;
            if (!outside) { damageAccumulator = 0f; return; }

            damageAccumulator += Time.deltaTime;
            if (damageAccumulator >= 1f)
            {
                int ticks = Mathf.FloorToInt(damageAccumulator);
                damageAccumulator -= ticks;
                PlayerHealth health = player.GetComponentInParent<PlayerHealth>();
                if (health != null) health.TakeDamage(outsideDamagePerSecond * ticks);
            }
        }

        public void SetPlayer(Transform value) => player = value;

        private void ApplyRingScale()
        {
            if (ringVisual == null) return;
            Vector3 scale = ringVisual.localScale;
            scale.x = currentRadius * 2f;
            scale.z = currentRadius * 2f;
            ringVisual.localScale = scale;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(zoneCenter, Application.isPlaying ? currentRadius : startRadius);
        }
    }
}