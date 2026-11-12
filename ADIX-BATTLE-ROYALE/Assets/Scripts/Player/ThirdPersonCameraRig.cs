using UnityEngine;

namespace AdixBattleRoyale.Player
{
    public sealed class ThirdPersonCameraRig : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 thirdPersonOffset = new Vector3(0f, 2.2f, -4.5f);
        [SerializeField] private Vector3 firstPersonOffset = new Vector3(0f, 1.65f, 0.12f);
        [SerializeField] private float sensitivity = 0.12f;
        [SerializeField] private float minPitch = -35f;
        [SerializeField] private float maxPitch = 65f;
        [SerializeField] private float followSmooth = 14f;

        private float yaw;
        private float pitch = 12f;
        private Vector2 lookInput;
        private bool firstPerson;

        public bool IsFirstPerson => firstPerson;
        public void SetLookInput(Vector2 input) => lookInput = input;
        public void SetSensitivity(float value) => sensitivity = Mathf.Clamp(value, 0.01f, 1.5f);
        public void ToggleCameraMode() => firstPerson = !firstPerson;

        private void LateUpdate()
        {
            if (target == null) return;

            yaw += lookInput.x * sensitivity;
            pitch = Mathf.Clamp(pitch - lookInput.y * sensitivity, minPitch, maxPitch);

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 offset = firstPerson ? firstPersonOffset : thirdPersonOffset;
            Vector3 desiredPosition = target.position + rotation * offset;

            transform.position = Vector3.Lerp(transform.position, desiredPosition,
                followSmooth * Time.deltaTime);
            transform.rotation = rotation;
        }
    }
}
