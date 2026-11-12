using UnityEngine;

namespace AdixBattleRoyale.Audio
{
    public enum GunSoundProfile { AssaultRifle, SMG, Shotgun, Sniper, Pistol, Marksman, Melee }

    [RequireComponent(typeof(AudioSource))]
    public sealed class GunAudioController : MonoBehaviour
    {
        [SerializeField] private GunSoundProfile profile;
        [Header("Assign a unique set of clips for this weapon")]
        [SerializeField] private AudioClip fireClip;
        [SerializeField] private AudioClip reloadStartClip;
        [SerializeField] private AudioClip reloadFinishClip;
        [SerializeField] private AudioClip emptyClip;
        [SerializeField, Range(0f, 1f)] private float fireVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float reloadVolume = 0.8f;
        private AudioSource source;

        public GunSoundProfile Profile => profile;

        private void Awake()
        {
            source = GetComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.minDistance = 4f;
            source.maxDistance = 110f;
        }

        public void PlayFire()
        {
            if (fireClip != null) source.PlayOneShot(fireClip, fireVolume);
            else Debug.LogWarning($"[{profile}] Fire clip is not assigned.", this);
        }

        public void PlayReloadStart()
        {
            if (reloadStartClip != null) source.PlayOneShot(reloadStartClip, reloadVolume);
        }

        public void PlayReloadFinish()
        {
            if (reloadFinishClip != null) source.PlayOneShot(reloadFinishClip, reloadVolume);
        }

        public void PlayEmpty()
        {
            if (emptyClip != null) source.PlayOneShot(emptyClip, 0.7f);
        }
    }
}