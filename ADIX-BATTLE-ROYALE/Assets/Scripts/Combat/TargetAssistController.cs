using UnityEngine;

namespace AdixBattleRoyale.Combat
{
    /// <summary>Optional local-game aim assist and auto-fire for nearby hostile targets.</summary>
    [DisallowMultipleComponent]
    public sealed class TargetAssistController : MonoBehaviour
    {
        [Header("Switches")]
        [SerializeField] private bool autoAimEnabled;
        [SerializeField] private bool autoFireEnabled;

        [Header("Targeting")]
        [SerializeField] private Camera aimCamera;
        [SerializeField] private PlayerWeapon weapon;
        [SerializeField] private LayerMask targetLayers = ~0;
        [SerializeField, Min(1f)] private float detectionRadius = 24f;
        [SerializeField, Range(1f, 180f)] private float aimConeDegrees = 32f;
        [SerializeField, Min(0f)] private float aimTurnSpeed = 8f;
        [SerializeField, Min(0f)] private float fireInterval = 0.12f;
        [SerializeField] private Transform aimPivot;

        private Transform currentTarget;
        private float nextAutoFireTime;

        public bool AutoAimEnabled => autoAimEnabled;
        public bool AutoFireEnabled => autoFireEnabled;
        public Transform CurrentTarget => currentTarget;

        private void Awake()
        {
            if (aimCamera == null) aimCamera = Camera.main;
            if (weapon == null) weapon = GetComponent<PlayerWeapon>();
            if (aimPivot == null && aimCamera != null) aimPivot = aimCamera.transform;
        }

        public void SetAutoAim(bool enabled) => autoAimEnabled = enabled;
        public void SetAutoFire(bool enabled) => autoFireEnabled = enabled;

        private void Update()
        {
            if ((!autoAimEnabled && !autoFireEnabled) || aimCamera == null || weapon == null)
            {
                currentTarget = null;
                return;
            }

            currentTarget = FindBestTarget();
            if (currentTarget == null) return;

            if (autoAimEnabled)
            {
                Vector3 direction = currentTarget.position - aimCamera.transform.position;
                if (direction.sqrMagnitude > 0.001f)
                {
                    Quaternion desired = Quaternion.LookRotation(direction.normalized, Vector3.up);
                    Transform pivot = aimPivot != null ? aimPivot : aimCamera.transform;
                    pivot.rotation = Quaternion.Slerp(pivot.rotation, desired, aimTurnSpeed * Time.deltaTime);
                }
            }

            if (autoFireEnabled && Time.time >= nextAutoFireTime && HasLineOfSight(currentTarget))
            {
                weapon.Fire();
                nextAutoFireTime = Time.time + Mathf.Max(0.05f, fireInterval);
            }
        }

        private Transform FindBestTarget()
        {
            Collider[] candidates = Physics.OverlapSphere(transform.position, detectionRadius, targetLayers, QueryTriggerInteraction.Ignore);
            Transform best = null;
            float bestAngle = aimConeDegrees;

            foreach (Collider candidate in candidates)
            {
                if (candidate == null || candidate.transform.root == transform.root) continue;
                IDamageable damageable = candidate.GetComponentInParent<IDamageable>();
                if (damageable == null) continue;

                Transform candidateRoot = candidate.transform.root;
                Vector3 toTarget = candidate.bounds.center - aimCamera.transform.position;
                if (toTarget.sqrMagnitude < 0.01f) continue;
                float angle = Vector3.Angle(aimCamera.transform.forward, toTarget);
                if (angle > bestAngle || !HasLineOfSight(candidateRoot)) continue;

                bestAngle = angle;
                best = candidateRoot;
            }

            return best;
        }

        private bool HasLineOfSight(Transform target)
        {
            if (target == null || aimCamera == null) return false;
            Vector3 origin = aimCamera.transform.position;
            Vector3 destination = target.position + Vector3.up;
            Vector3 delta = destination - origin;
            if (Physics.Raycast(origin, delta.normalized, out RaycastHit hit, delta.magnitude + 0.25f, ~0, QueryTriggerInteraction.Ignore))
                return hit.transform.root == target.root;
            return false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
