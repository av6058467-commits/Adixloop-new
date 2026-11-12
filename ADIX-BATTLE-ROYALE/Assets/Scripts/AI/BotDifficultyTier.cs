using UnityEngine;

namespace AdixBattleRoyale.AI
{
    // Each tier raises bot capability through measurable gameplay parameters.
    [CreateAssetMenu(menuName = "ADIX Battle Royale/AI/Bot Difficulty Tier")]
    public sealed class BotDifficultyTier : ScriptableObject
    {
        [Min(1)] public int level = 1;
        [Range(0.1f, 1f)] public float accuracy = 0.25f;
        [Min(0f)] public float reactionTime = 1.8f;
        [Min(1f)] public float detectionRange = 35f;
        [Min(1f)] public float preferredCombatRange = 22f;
        [Min(0f)] public float aimSpreadDegrees = 7f;
        [Min(0f)] public float movementSpeed = 3.2f;
        [Min(0f)] public float decisionInterval = 1.2f;
        [Range(0f, 1f)] public float coverUsage = 0.15f;
        [Range(0f, 1f)] public float flankChance = 0.05f;
        [Min(0f)] public float healthMultiplier = 1f;
        [Min(0f)] public float damageMultiplier = 1f;
    }
}
