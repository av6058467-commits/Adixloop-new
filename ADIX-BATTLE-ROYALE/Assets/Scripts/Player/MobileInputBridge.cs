using UnityEngine;
using AdixBattleRoyale.Combat;
using AdixBattleRoyale.Player;
using AdixBattleRoyale.World;

namespace AdixBattleRoyale.Player
{
    /// <summary>Central bridge from mobile HUD controls to player and vehicle systems.</summary>
    public sealed class MobileInputBridge : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private VirtualJoystick moveJoystick;
        [SerializeField] private MobilePlayerController player;
        [SerializeField] private ThirdPersonCameraRig cameraRig;
        [SerializeField] private PlayerWeapon weapon;

        [Header("Vehicle")]
        [SerializeField] private VehicleInteraction vehicleInteraction;
        [SerializeField] private VehicleDrive vehicleDrive;
        [SerializeField] private HelicopterController helicopter;
        private float vehicleThrottle;
        private float vehicleSteering;
        private bool vehicleBraking;

        private void Update()
        {
            if (moveJoystick != null && player != null && player.enabled)
                player.SetMoveInput(moveJoystick.Value);
        }

        public void FirePressed() => weapon?.Fire();
        public void ReloadPressed() => weapon?.Reload();
        public void JumpPressed() => player?.Jump();
        public void ToggleCameraPressed() => cameraRig?.ToggleCameraMode();
        public void SetLookInput(Vector2 value) => cameraRig?.SetLookInput(value);
        public void SprintDown() => player?.SetSprint(true);
        public void SprintUp() => player?.SetSprint(false);
        public void ToggleCrouchPressed() => player?.ToggleCrouch();
        public void InteractVehiclePressed() => vehicleInteraction?.EnterOrExitVehicle();

        // Connect gas/reverse, steering and brake UI controls to these methods.
        public void SetVehicleThrottle(float value)
        {
            vehicleThrottle = Mathf.Clamp(value, -1f, 1f);
            ApplyVehicleInput();
        }

        public void SetVehicleSteering(float value)
        {
            vehicleSteering = Mathf.Clamp(value, -1f, 1f);
            ApplyVehicleInput();
        }

        public void VehicleBrakeDown()
        {
            vehicleBraking = true;
            ApplyVehicleInput();
        }

        public void VehicleBrakeUp()
        {
            vehicleBraking = false;
            ApplyVehicleInput();
        }

        public void VehicleControlsReleased()
        {
            vehicleThrottle = vehicleSteering = 0f;
            vehicleBraking = false;
            ApplyVehicleInput();
        }

        private void ApplyVehicleInput()
        {
            if (vehicleDrive != null)
                vehicleDrive.SetInput(vehicleThrottle, vehicleSteering, vehicleBraking);
        }

        // Helicopter HUD sliders/buttons supply normalized axis values.
        public void SetHelicopterInput(Vector4 axes)
        {
            if (helicopter != null)
                helicopter.SetFlightInput(axes.x, axes.y, axes.z, axes.w);
        }
    }
}