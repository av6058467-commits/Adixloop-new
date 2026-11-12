using UnityEngine;

namespace AdixBattleRoyale.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class MobilePlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 4.2f;
        [SerializeField] private float sprintSpeed = 6.2f;
        [SerializeField] private float jumpHeight = 1.25f;
        [SerializeField] private float gravity = -22f;
        [SerializeField] private Transform cameraTransform;

        [Header("State")]
        [SerializeField] private bool sprint;
        [SerializeField] private bool crouch;

        private CharacterController controller;
        private Vector2 moveInput;
        private float verticalVelocity;

        public bool IsCrouching => crouch;
        public bool IsGrounded => controller != null && controller.isGrounded;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;
        }

        public void SetMoveInput(Vector2 input) => moveInput = Vector2.ClampMagnitude(input, 1f);
        public void SetSprint(bool value) => sprint = value;
        public void ToggleCrouch() => crouch = !crouch;

        public void Jump()
        {
            if (controller != null && controller.isGrounded)
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        private void Update()
        {
            if (controller == null) return;

            Vector3 forward = cameraTransform != null ? cameraTransform.forward : transform.forward;
            Vector3 right = cameraTransform != null ? cameraTransform.right : transform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 move = forward * moveInput.y + right * moveInput.x;
            float speed = sprint ? sprintSpeed : walkSpeed;
            if (crouch) speed *= 0.55f;

            if (move.sqrMagnitude > 0.01f)
            {
                controller.Move(move.normalized * speed * Time.deltaTime);
                Quaternion face = Quaternion.LookRotation(move.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, face, 12f * Time.deltaTime);
            }

            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;
            verticalVelocity += gravity * Time.deltaTime;
            controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
        }
    }
}
