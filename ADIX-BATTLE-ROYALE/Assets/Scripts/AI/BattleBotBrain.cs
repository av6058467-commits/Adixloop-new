using System;
using UnityEngine;
using UnityEngine.AI;

namespace AdixBattleRoyale.AI
{
    public enum BotCombatState { Idle, Search, Chase, Engage }

    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class BattleBotBrain : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private BotDifficultyTier difficulty;
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private LayerMask lineOfSightMask = ~0;
        [SerializeField, Min(0.5f)] private float lostTargetMemory = 5f;
        [SerializeField, Min(1f)] private float flankDistance = 8f;
        [SerializeField, Min(1f)] private float coverSearchRadius = 12f;

        private NavMeshAgent agent;
        private BotCombatState state = BotCombatState.Idle;
        private float nextDecisionTime;
        private float nextShotTime;
        private float lastSeenTime = -999f;
        private Vector3 lastKnownTargetPosition;
        private Vector3 tacticalDestination;
        private int patrolIndex;
        private bool hasTacticalDestination;

        public event Action<float, float> ShotRequested;
        public BotCombatState State => state;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            ApplyDifficulty();
        }

        public void Configure(Transform playerTarget, BotDifficultyTier tier)
        {
            target = playerTarget;
            difficulty = tier;
            ApplyDifficulty();
        }

        public void ApplyDifficulty()
        {
            if (agent == null || difficulty == null) return;
            agent.speed = difficulty.movementSpeed;
            agent.stoppingDistance = Mathf.Max(2f, difficulty.preferredCombatRange * 0.65f);
        }

        private void Update()
        {
            if (difficulty == null || agent == null || !agent.isOnNavMesh || target == null) return;

            bool visible = CanSeeTarget();
            if (visible)
            {
                lastSeenTime = Time.time;
                lastKnownTargetPosition = target.position;
            }

            if (Time.time >= nextDecisionTime)
            {
                nextDecisionTime = Time.time + Mathf.Max(0.15f, difficulty.decisionInterval);
                Decide(visible);
            }

            RunState(visible);
        }

        private bool CanSeeTarget()
        {
            Vector3 origin = transform.position + Vector3.up * 1.5f;
            Vector3 destination = target.position + Vector3.up;
            Vector3 direction = destination - origin;
            float distance = direction.magnitude;
            if (distance > difficulty.detectionRange) return false;

            // Ignore this bot's own colliders by checking all hits in distance order.
            RaycastHit[] hits = Physics.RaycastAll(origin, direction.normalized, distance,
                lineOfSightMask, QueryTriggerInteraction.Ignore);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform == transform || hit.transform.IsChildOf(transform)) continue;
                return hit.transform == target || hit.transform.IsChildOf(target);
            }
            return true;
        }

        private void Decide(bool visible)
        {
            hasTacticalDestination = false;
            if (!visible)
            {
                state = Time.time - lastSeenTime <= lostTargetMemory
                    ? BotCombatState.Search
                    : (patrolPoints != null && patrolPoints.Length > 0
                        ? BotCombatState.Search : BotCombatState.Idle);
                return;
            }

            float distance = Vector3.Distance(transform.position, target.position);
            state = distance <= difficulty.preferredCombatRange
                ? BotCombatState.Engage : BotCombatState.Chase;

            if (state == BotCombatState.Engage)
            {
                float roll = UnityEngine.Random.value;
                if (roll < difficulty.coverUsage)
                    hasTacticalDestination = TryFindCover(out tacticalDestination);
                else if (roll < difficulty.coverUsage + difficulty.flankChance)
                    hasTacticalDestination = TryFindFlank(out tacticalDestination);
            }
        }

        private void RunState(bool visible)
        {
            switch (state)
            {
                case BotCombatState.Chase:
                    agent.isStopped = false;
                    agent.SetDestination(target.position);
                    break;

                case BotCombatState.Engage:
                    FaceTarget(target.position);
                    if (hasTacticalDestination)
                    {
                        agent.isStopped = false;
                        agent.SetDestination(tacticalDestination);
                    }
                    else
                    {
                        agent.isStopped = true;
                    }

                    if (visible && Time.time >= nextShotTime)
                    {
                        nextShotTime = Time.time + Mathf.Max(0.25f, difficulty.reactionTime);
                        ShotRequested?.Invoke(difficulty.accuracy, difficulty.damageMultiplier);
                    }
                    break;

                case BotCombatState.Search:
                    agent.isStopped = false;
                    if (Time.time - lastSeenTime <= lostTargetMemory)
                        agent.SetDestination(lastKnownTargetPosition);
                    else if (patrolPoints != null && patrolPoints.Length > 0)
                    {
                        Transform point = patrolPoints[patrolIndex % patrolPoints.Length];
                        if (point != null && (!agent.hasPath || agent.remainingDistance < 1.5f))
                            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                        if (point != null) agent.SetDestination(point.position);
                    }
                    break;

                default:
                    agent.isStopped = true;
                    break;
            }
        }

        private bool TryFindFlank(out Vector3 destination)
        {
            Vector3 toTarget = (target.position - transform.position).normalized;
            Vector3 side = Vector3.Cross(Vector3.up, toTarget) *
                           (UnityEngine.Random.value < 0.5f ? -1f : 1f);
            Vector3 candidate = target.position + side * flankDistance -
                                toTarget * (flankDistance * 0.35f);
            return NavMesh.SamplePosition(candidate, out NavMeshHit hit,
                flankDistance, NavMesh.AllAreas) && (destination = hit.position) == hit.position;
        }

        private bool TryFindCover(out Vector3 destination)
        {
            Vector3 away = (transform.position - target.position).normalized;
            for (int i = 0; i < 8; i++)
            {
                Vector2 random = UnityEngine.Random.insideUnitCircle * coverSearchRadius;
                Vector3 candidate = transform.position + new Vector3(random.x, 0f, random.y) + away * 3f;
                if (!NavMesh.SamplePosition(candidate, out NavMeshHit navHit, 3f, NavMesh.AllAreas))
                    continue;

                Vector3 eye = navHit.position + Vector3.up * 1.3f;
                Vector3 targetEye = target.position + Vector3.up;
                if (Physics.Linecast(eye, targetEye, lineOfSightMask, QueryTriggerInteraction.Ignore))
                {
                    destination = navHit.position;
                    return true;
                }
            }
            destination = default;
            return false;
        }

        private void FaceTarget(Vector3 position)
        {
            Vector3 direction = position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.01f) return;
            Quaternion desired = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, desired, Time.deltaTime * 7f);
        }

        public void SetTarget(Transform newTarget) => target = newTarget;
    }
}