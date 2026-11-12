using System.Collections.Generic;
using UnityEngine;
using AdixBattleRoyale.AI;

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
                GameObject bot = Instantiate(botPrefab, spawn.position + offset, spawn.rotation);
                BattleBotBrain brain = bot.GetComponent<BattleBotBrain>();
                if (brain != null) brain.Configure(player, tier);
                aliveBots.Add(bot);
            }
        }

        public void NotifyBotEliminated(GameObject bot)
        {
            if (bot == null) return;
            aliveBots.Remove(bot);
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
