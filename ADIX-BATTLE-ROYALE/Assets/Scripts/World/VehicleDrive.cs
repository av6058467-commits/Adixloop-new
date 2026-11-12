using UnityEngine;
using AdixBattleRoyale.Audio;

namespace AdixBattleRoyale.World
{
    /// <summary>Arcade mobile vehicle controller. Requires Rigidbody and WheelColliders for full setup.</summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class VehicleDrive : MonoBehaviour
    {
        [SerializeField] private WheelCollider[] steeringWheels;
        [SerializeField] private WheelCollider[] driveWheels;
        [SerializeField] private float motorTorque = 1500f;
        [SerializeField] private float brakeTorque = 2800f;
        [SerializeField] private float maxSteerAngle = 28f;
        [SerializeField] private float maxSpeedKph = 115f;
        [SerializeField] private Transform centerOfMass;
        private Rigidbody body;
        private VehicleEngineAudio vehicleAudio;
        private float throttle;
        private float steering;
        private bool braking;
        public bool HasDriver { get; private set; }

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            vehicleAudio = GetComponent<VehicleEngineAudio>();
            if (centerOfMass != null) body.centerOfMass = transform.InverseTransformPoint(centerOfMass.position);
            body.interpolation = RigidbodyInterpolation.Interpolate;
        }

        public void SetDriver(bool occupied)
        {
            if (occupied && !HasDriver) vehicleAudio?.PlayStart();
            HasDriver = occupied;
        }
        public void SetInput(float throttleInput, float steeringInput, bool brake)
        {
            throttle = HasDriver ? Mathf.Clamp(throttleInput, -1f, 1f) : 0f;
            steering = HasDriver ? Mathf.Clamp(steeringInput, -1f, 1f) : 0f;
            braking = HasDriver && brake;
            vehicleAudio?.SetBraking(braking);
        }

        private void FixedUpdate()
        {
            float speed = body.velocity.magnitude * 3.6f;
            float torque = speed < maxSpeedKph || throttle < 0f ? throttle * motorTorque : 0f;
            foreach (WheelCollider wheel in driveWheels)
                if (wheel != null) wheel.motorTorque = torque;
            foreach (WheelCollider wheel in steeringWheels)
                if (wheel != null) wheel.steerAngle = steering * maxSteerAngle;
            foreach (WheelCollider wheel in driveWheels)
                if (wheel != null) wheel.brakeTorque = braking ? brakeTorque : 0f;
        }
    }
}