using UnityEngine;

namespace AdixBattleRoyale.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class HelicopterRotorAudio : MonoBehaviour
    {
        [SerializeField] private AudioClip rotorLoop;
        [SerializeField] private AudioClip startupClip;
        [SerializeField, Range(0f, 1f)] private float volume = 0.85f;
        private AudioSource source;

        private void Awake()
        {
            source = GetComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = true;
            source.spatialBlend = 1f;
            source.minDistance = 8f;
            source.maxDistance = 220f;
            if (rotorLoop != null) { source.clip = rotorLoop; source.volume = 0f; source.Play(); }
        }

        public void SetRotorPower(float power)
        {
            float value = Mathf.Clamp01(power);
            source.volume = value * volume;
            source.pitch = Mathf.Lerp(0.75f, 1.2f, value);
        }

        public void PlayStartup()
        {
            if (startupClip != null) source.PlayOneShot(startupClip);
        }
    }
}