# Player control foundation

## Player prefab
1. Create a Player object with a CharacterController.
2. Attach `MobilePlayerController`.
3. Assign the active camera Transform.
4. Create a camera rig and attach `ThirdPersonCameraRig`; assign the Player as target.
5. Connect the left virtual joystick output to `SetMoveInput(Vector2)`.
6. Connect the right look pad output to `ThirdPersonCameraRig.SetLookInput(Vector2)`.
7. Connect Jump button to `Jump()`, Sprint toggle to `SetSprint(bool)`, Crouch button to `ToggleCrouch()`, and camera switch to `ToggleCameraMode()`.
8. Create UI controls with custom normal/pressed/selected/disabled sprites and wire their visual Animator states.

## Current limits
This is the movement and camera script foundation. A virtual joystick/look-pad implementation, player animations, weapon controller, collision tuning, camera obstruction handling, HUD prefab, and scene wiring still need to be added and tested. First-person mode currently changes camera position; a dedicated first-person arms/weapon view is still required.
