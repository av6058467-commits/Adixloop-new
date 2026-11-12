using UnityEngine;

namespace AdixBattleRoyale.Combat
{
    /// <summary>Subscribe this component to BattleBotBrain.ShotRequested.</summary>
    public sealed class BotWeaponEmitter : MonoBehaviour
    {
        [SerializeField] private Transform muzzle;
        [SerializeField] private Transform target;
        [SerializeField] private LayerMask hitMask = ~0;
        [SerializeField, Min(1f)] private float range = 260f;
        [SerializeField, Min(0f)] private float baseDamage = 8f;

        public void SetTarget(Transform newTarget) => target = newTarget;

        public void Fire(float accuracy, float damageMultiplier)
        {
            if (target == null || muzzle == null) return;
            Vector3 aimPoint = target.position + Vector3.up;
            Vector3 ideal = (aimPoint - muzzle.position).normalized;
            float spread = Mathf.Lerp(12f, 0.4f, Mathf.Clamp01(accuracy));
            Vector3 direction = Quaternion.Euler(
                Random.Range(-spread, spread),
                Random.Range(-spread, spread), 0f) * ideal;

            if (!Physics.Raycast(muzzle.position, direction, out RaycastHit hit,
                    range, hitMask, QueryTriggerInteraction.Ignore)) return;

            PlayerHealth health = hit.collider.GetComponentInParent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(baseDamage * Mathf.Max(0.1f, damageMultiplier));
        }
    }
}