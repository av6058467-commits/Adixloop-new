using UnityEngine;

namespace AdixBattleRoyale.Emotes
{
    public enum EmoteCategory { Dance, Victory, Entry, WeaponShowcase, Team, Fun }
    public enum EmoteGameMode { Any, BattleRoyale, ClashSquad, Hybrid, Training }

    [CreateAssetMenu(fileName = "ADIX_Emote", menuName = "ADIX/Emotes/Emote Definition")]
    public sealed class EmoteDefinition : ScriptableObject
    {
        [SerializeField] private string emoteId = "adix_emote";
        [SerializeField] private string displayName = "New Emote";
        [SerializeField] private Sprite icon;
        [SerializeField] private EmoteCategory category;
        [SerializeField] private string animatorState = "Emote_Idle";
        [SerializeField, Min(0f)] private float crossFadeSeconds = 0.15f;
        [SerializeField, Min(0f)] private float durationSeconds = 3f;
        [SerializeField] private bool loop;
        [SerializeField] private bool requiresWeapon;
        [SerializeField] private bool allowInCombat;
        [SerializeField] private EmoteGameMode[] availableModes = { EmoteGameMode.Any };

        public string EmoteId => emoteId;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public EmoteCategory Category => category;
        public string AnimatorState => animatorState;
        public float CrossFadeSeconds => crossFadeSeconds;
        public float DurationSeconds => durationSeconds;
        public bool Loop => loop;
        public bool RequiresWeapon => requiresWeapon;
        public bool AllowInCombat => allowInCombat;

        public bool IsAvailableIn(EmoteGameMode mode)
        {
            if (availableModes == null || availableModes.Length == 0) return false;
            foreach (EmoteGameMode allowed in availableModes)
                if (allowed == EmoteGameMode.Any || allowed == mode) return true;
            return false;
        }
    }
}
