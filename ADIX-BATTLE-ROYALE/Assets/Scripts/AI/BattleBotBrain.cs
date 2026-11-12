using System;
using UnityEngine;
using UnityEngine.AI;

namespace AdixBattleRoyale.AI
{
    public enum BotCombatState { Idle, Search, Chase, Engage }

    // Scene-ready AI foundation. A match director assigns the target and tier.
    // A weapon component should subscribe to ShotRequested to perform damage.
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class BattleBotBrain : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private BotDifficultyTier difficulty;
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private LayerMask lineOfSightMask = ~0;
        [SerializeField, Min(0.5f)] private float lostTargetMemory = 5f;

        private NavMeshAgent agent;
        private BotCombatState state = BotCombatState.Idle;
        private float nextDecisionTime;
        private float nextShotTime;
        private float lastSeenTime = -999f;
        private Vector3 lastKnownTargetPosition;
        private int patrolIndex;

        // accuracy and damage multiplier are passed to the equipped weapon.
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
            if (difficulty == null || agent == null || !agent.isOnNavMesh) return;

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
            if (target == null) return false;

            Vector3 origin = transform.position + Vector3.up * 1.5f;
            Vector3 destination = target.position + Vector3.up;
            Vector3 direction = destination - origin;
            float distance = direction.magnitude;

            if (distance > difficulty.detectionRange) return false;
            if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit,
                    distance, lineOfSightMask, QueryTriggerInteraction.Ignore))
                return hit.transform == target || hit.transform.IsChildOf(target);

            return true;
        }

        private void Decide(bool visible)
        {
            if (visible)
            {
                float distance = Vector3.Distance(transform.position, target.position);
                state = distance <= difficulty.preferredCombatRange
                    ? BotCombatState.Engage : BotCombatState.Chase;
            }
            else if (Time.time - lastSeenTime <= lostTargetMemory)
                state = BotCombatState.Search;
            else
                state = patrolPoints != null && patrolPoints.Length > 0
                    ? BotCombatState.Search : BotCombatState.Idle;
        }

        private void RunState(bool visible)
        {
            switch (state)
            {
                case BotCombatState.Chase:
                    if (target != null) agent.SetDestination(target.position);
                    break;

                case BotCombatState.Engage:
                    if (target == null) break;
                    agent.SetDestination(transform.position);
                    FaceTarget(target.position);
                    if (visible && Time.time >= nextShotTime)
                    {
                        nextShotTime = Time.time + Mathf.Max(0.25f, difficulty.reactionTime);
                        ShotRequested?.Invoke(difficulty.accuracy, difficulty.damageMultiplier);
                    }
                    break;

                case BotCombatState.Search:
                    if (Time.time - lastSeenTime <= lostTargetMemory)
                        agent.SetDestination(lastKnownTargetPosition);
                    else if (patrolPoints != null && patrolPoints.Length > 0)
                    {
                        Transform point = patrolPoints[patrolIndex % patrolPoints.Length];
                        if (point != null && (!agent.hasPath || agent.remainingDistance < 1.5f))
                            patrolIndex++;
                        if (point != null) agent.SetDestination(point.position);
                    }
                    break;
            }
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
