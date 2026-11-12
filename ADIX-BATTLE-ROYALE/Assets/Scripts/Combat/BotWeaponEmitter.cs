using System;
using UnityEngine;

namespace AdixBattleRoyale.Combat
{
    /// <summary>Receives BattleBotBrain.ShotRequested and resolves hits on the player.</summary>
    public sealed class BotWeaponEmitter : MonoBehaviour
    {
        [SerializeField] private Transform muzzle;
        [SerializeField] private Transform target;
        [SerializeField] private LayerMask hitMask = ~0;
        [SerializeField, Min(1f)] private float range = 260f;
        [SerializeField, Min(0f)] private float baseDamage = 8f;

        private Transform botRoot;

        private void Awake() => botRoot = transform.root;

        public void SetTarget(Transform newTarget) => target = newTarget;

        public void Fire(float accuracy, float damageMultiplier)
        {
            if (target == null || muzzle == null) return;

            Vector3 aimPoint = target.position + Vector3.up;
            Vector3 ideal = (aimPoint - muzzle.position).normalized;
            float spread = Mathf.Lerp(12f, 0.4f, Mathf.Clamp01(accuracy));
            Vector3 direction = Quaternion.Euler(
                UnityEngine.Random.Range(-spread, spread),
                UnityEngine.Random.Range(-spread, spread), 0f) * ideal;

            // RaycastAll lets us ignore the bot's own colliders instead of
            // accidentally registering its weapon/body as the first hit.
            RaycastHit[] hits = Physics.RaycastAll(
                muzzle.position, direction, range, hitMask,
                QueryTriggerInteraction.Ignore);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform == botRoot || hit.transform.IsChildOf(botRoot))
                    continue;

                PlayerHealth health = hit.collider.GetComponentInParent<PlayerHealth>();
                if (health != null)
                    health.TakeDamage(baseDamage * Mathf.Max(0.1f, damageMultiplier));
                break; // First non-self obstruction blocks the shot.
            }
        }
    }
}