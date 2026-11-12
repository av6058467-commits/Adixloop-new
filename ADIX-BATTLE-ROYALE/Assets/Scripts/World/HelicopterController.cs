using UnityEngine;
using AdixBattleRoyale.Audio;

namespace AdixBattleRoyale.World
{
    /// <summary>Arcade helicopter flight controller. Drive from mobile throttle, yaw and lift buttons.</summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class HelicopterController : MonoBehaviour
    {
        [SerializeField] private Transform centerOfMass;
        [SerializeField, Min(1f)] private float liftAcceleration = 18f;
        [SerializeField, Min(0f)] private float forwardAcceleration = 10f;
        [SerializeField, Min(0f)] private float yawSpeed = 65f;
        [SerializeField, Min(0f)] private float tiltAngle = 18f;
        [SerializeField, Min(1f)] private float maxSpeed = 42f;
        [SerializeField, Min(0f)] private float stabilization = 3f;

        private Rigidbody body;
        private HelicopterRotorAudio rotorAudio;
        private float lift;
        private float pitch;
        private float roll;
        private float yaw;
        private bool occupied;

        public bool HasPilot => occupied;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            rotorAudio = GetComponent<HelicopterRotorAudio>();
            body.useGravity = true;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            if (centerOfMass != null)
                body.centerOfMass = transform.InverseTransformPoint(centerOfMass.position);
        }

        public void SetPilot(bool value)
        {
            if (value && !occupied) rotorAudio?.PlayStartup();
            occupied = value;
        }

        // Values are normalized from -1 to 1. Positive lift ascends.
        public void SetFlightInput(float liftInput, float pitchInput, float rollInput, float yawInput)
        {
            if (!occupied)
            {
                lift = pitch = roll = yaw = 0f;
                return;
            }
            lift = Mathf.Clamp(liftInput, -1f, 1f);
            pitch = Mathf.Clamp(pitchInput, -1f, 1f);
            roll = Mathf.Clamp(rollInput, -1f, 1f);
            yaw = Mathf.Clamp(yawInput, -1f, 1f);
        }

        private void FixedUpdate()
        {
            rotorAudio?.SetRotorPower(occupied ? Mathf.Clamp01(0.65f + Mathf.Abs(lift) * 0.35f) : 0.25f);
            if (!occupied)
            {
                body.AddTorque(-body.angularVelocity * stabilization, ForceMode.Acceleration);
                return;
            }

            body.AddForce(Vector3.up * lift * liftAcceleration, ForceMode.Acceleration);
            Vector3 planarForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            body.AddForce(planarForward * (pitch * forwardAcceleration), ForceMode.Acceleration);
            Quaternion targetTilt = Quaternion.Euler(
                pitch * -tiltAngle, transform.eulerAngles.y, roll * -tiltAngle);
            Quaternion tiltError = targetTilt * Quaternion.Inverse(body.rotation);
            tiltError.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f) angle -= 360f;
            body.AddTorque(axis * (angle * Mathf.Deg2Rad * stabilization), ForceMode.Acceleration);
            body.AddTorque(Vector3.up * yaw * yawSpeed * Mathf.Deg2Rad, ForceMode.Acceleration);

            if (body.velocity.magnitude > maxSpeed)
                body.velocity = body.velocity.normalized * maxSpeed;
        }
    }
}