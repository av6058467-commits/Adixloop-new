using System;
using System.Collections;
using UnityEngine;

namespace AdixBattleRoyale.Emotes
{
    [DisallowMultipleComponent]
    public sealed class EmoteController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Transform weaponVisual;
        [SerializeField] private AudioSource emoteAudio;
        [SerializeField] private EmoteGameMode currentMode = EmoteGameMode.BattleRoyale;
        [SerializeField] private bool inCombat;\n        [SerializeField] private bool healingInProgress;

        private Coroutine activeRoutine;
        private EmoteDefinition activeEmote;

        public EmoteDefinition ActiveEmote => activeEmote;
        public bool IsPlaying => activeEmote != null;
        public event Action<EmoteDefinition> EmoteStarted;
        public event Action EmoteEnded;

        private void Awake()
        {
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }

        public void SetGameMode(EmoteGameMode mode) => currentMode = mode;
        public void SetCombatState(bool value) => inCombat = value;\n        public void SetHealingState(bool value) => healingInProgress = value;

        public bool CanPlay(EmoteDefinition emote)
        {
            if (emote == null || animator == null) return false;
            if (!emote.IsAvailableIn(currentMode)) return false;
            if (inCombat && !emote.AllowInCombat) return false;\n            if (healingInProgress && !emote.AllowDuringHealing) return false;
            if (emote.RequiresWeapon && (weaponVisual == null || !weaponVisual.gameObject.activeInHierarchy)) return false;
            return !string.IsNullOrWhiteSpace(emote.AnimatorState);
        }

        public bool Play(EmoteDefinition emote)
        {
            if (!CanPlay(emote)) return false;
            Stop();

            activeEmote = emote;
            animator.CrossFadeInFixedTime(emote.AnimatorState, Mathf.Max(0f, emote.CrossFadeSeconds));
            if (emoteAudio != null && !emoteAudio.isPlaying) emoteAudio.Play();
            EmoteStarted?.Invoke(emote);

            if (!emote.Loop && emote.DurationSeconds > 0f)
                activeRoutine = StartCoroutine(StopAfter(emote.DurationSeconds));
            return true;
        }

        public void Stop()
        {
            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
                activeRoutine = null;
            }

            if (activeEmote == null) return;
            activeEmote = null;
            if (emoteAudio != null && emoteAudio.isPlaying) emoteAudio.Stop();
            EmoteEnded?.Invoke();
        }

        private IEnumerator StopAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            activeRoutine = null;
            Stop();
        }

        private void OnDisable() => Stop();
    }
}
