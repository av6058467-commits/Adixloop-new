using UnityEngine;

namespace AdixBattleRoyale.Audio
{
    public enum VehicleSoundProfile { SUV, SportsCar, DirtBike, Buggy, MilitaryTruck }

    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class VehicleEngineAudio : MonoBehaviour
    {
        [SerializeField] private VehicleSoundProfile profile;
        [Header("Use unique clips for each vehicle type")]
        [SerializeField] private AudioClip engineIdle;
        [SerializeField] private AudioClip engineRunning;
        [SerializeField] private AudioClip engineStart;
        [SerializeField] private AudioClip brakeClip;
        [SerializeField] private AudioClip hornClip;
        [SerializeField, Range(0.5f, 2f)] private float minPitch = 0.8f;
        [SerializeField, Range(0.5f, 3f)] private float maxPitch = 1.65f;
        private AudioSource engine;
        private Rigidbody body;
        private float lastSpeed;
        private bool wasBraking;

        private void Awake()
        {
            engine = GetComponent<AudioSource>();
            body = GetComponent<Rigidbody>();
            engine.loop = true;
            engine.playOnAwake = false;
            engine.spatialBlend = 1f;
            engine.rolloffMode = AudioRolloffMode.Logarithmic;
            engine.minDistance = 5f;
            engine.maxDistance = 140f;
            if (engineIdle != null) { engine.clip = engineIdle; engine.Play(); }
        }

        private void Update()
        {
            float speed = body.velocity.magnitude;
            float acceleration = (speed - lastSpeed) / Mathf.Max(Time.deltaTime, 0.001f);
            lastSpeed = speed;
            AudioClip desired = speed > 1.2f && engineRunning != null ? engineRunning : engineIdle;
            if (desired != null && engine.clip != desired)
            {
                engine.clip = desired;
                engine.Play();
            }
            engine.pitch = Mathf.Lerp(minPitch, maxPitch, Mathf.InverseLerp(0f, 32f, speed));
            engine.volume = Mathf.Lerp(0.25f, 0.9f, Mathf.InverseLerp(0f, 24f, speed));
        }

        public void PlayStart()
        {
            if (engineStart != null) engine.PlayOneShot(engineStart);
        }

        public void SetBraking(bool braking)
        {
            if (braking && !wasBraking && brakeClip != null) engine.PlayOneShot(brakeClip);
            wasBraking = braking;
        }

        public void Honk()
        {
            if (hornClip != null) engine.PlayOneShot(hornClip);
        }
    }
}