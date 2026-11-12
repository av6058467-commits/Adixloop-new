using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AdixBattleRoyale.AI;
using AdixBattleRoyale.Combat;

namespace AdixBattleRoyale.Match
{
    public sealed class BattleRoyaleMatchDirector : MonoBehaviour
    {
        [Header("Match")]
        [SerializeField, Min(1)] private int botCount = 50;
        [SerializeField] private Transform player;
        [SerializeField] private GameObject botPrefab;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private BotProgressionManager progression;

        private readonly List<GameObject> aliveBots = new List<GameObject>();
        public int AliveBots => aliveBots.Count;
        public bool MatchStarted { get; private set; }

        private void Start()
        {
            StartOfflineMatch();
        }

        public void StartOfflineMatch()
        {
            if (MatchStarted || player == null || botPrefab == null ||
                spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogWarning("Match setup incomplete: assign player, bot prefab and spawn points.");
                return;
            }

            MatchStarted = true;
            BotDifficultyTier tier = progression != null ? progression.GetCurrentTier() : null;

            for (int i = 0; i < botCount; i++)
            {
                Transform spawn = spawnPoints[i % spawnPoints.Length];
                Vector3 offset = new Vector3(Random.Range(-4f, 4f), 0f, Random.Range(-4f, 4f));
                Vector3 desiredPosition = spawn.position + offset;
                if (NavMesh.SamplePosition(desiredPosition, out NavMeshHit navHit, 12f, NavMesh.AllAreas))
                    desiredPosition = navHit.position;
                GameObject bot = Instantiate(botPrefab, desiredPosition, spawn.rotation);

                BotHealth health = bot.GetComponent<BotHealth>();
                if (health != null)
                {
                    health.SetMatchDirector(this);
                    if (tier != null) health.ApplyDifficultyHealth(tier.healthMultiplier);
                }

                BattleBotBrain brain = bot.GetComponent<BattleBotBrain>();
                BotWeaponEmitter weapon = bot.GetComponentInChildren<BotWeaponEmitter>();
                if (weapon != null) weapon.SetTarget(player);
                if (brain != null)
                {
                    brain.Configure(player, tier);
                    if (weapon != null) brain.ShotRequested += weapon.Fire;
                }

                aliveBots.Add(bot);
            }
        }

        public void NotifyBotEliminated(GameObject bot)
        {
            if (bot == null || !aliveBots.Remove(bot)) return;
            Destroy(bot);
            if (MatchStarted && aliveBots.Count == 0)
                Debug.Log("Winner: Player — all bots eliminated.");
        }

        public void EndMatch()
        {
            MatchStarted = false;
            foreach (GameObject bot in aliveBots)
                if (bot != null) Destroy(bot);
            aliveBots.Clear();
        }
    }
}