using UnityEngine;
using AdixBattleRoyale.Combat;
using AdixBattleRoyale.Player;

namespace AdixBattleRoyale.Player
{
    /// <summary>Connect UI joystick/look-pad values to movement and camera scripts.</summary>
    public sealed class MobileInputBridge : MonoBehaviour
    {
        [SerializeField] private VirtualJoystick moveJoystick;
        [SerializeField] private MobilePlayerController player;
        [SerializeField] private ThirdPersonCameraRig cameraRig;
        [SerializeField] private PlayerWeapon weapon;

        private void Update()
        {
            if (moveJoystick != null && player != null)
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
    }
}