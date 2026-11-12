# ADIX Original Emote System

This module provides the gameplay foundation for original, unlockable character and weapon emotes.

## Included
- ScriptableObject definitions with unique IDs, categories, icons, animation state names, durations, loop settings and allowed modes.
- Runtime controller with mode/combat/weapon validation, safe null checks, timed stop and events for UI/audio.
- Eight-slot emote wheel presenter with equip and select methods.

## Unity setup
1. Add an Animator to the operator rig and author original states such as `Emote_TornadoEntry`, `Emote_GunShowcase`, `Emote_WeaponInspect`, `Emote_VictoryReload`, and `Emote_AirPose`.
2. Create assets using **Assets > Create > ADIX > Emotes > Emote Definition**. Use unique original animation clips and audio; do not import another game's animation, sound, icon, character or UI.
3. Add `EmoteController` to the operator root; assign the Animator, weapon visual and optional AudioSource.
4. Add `EmoteWheelPresenter` to the wheel root; assign eight Buttons, controller and equipped definitions. Hook the open/close control to `Toggle()`.
5. Set the current mode to BattleRoyale, ClashSquad, Hybrid or Training before showing the wheel.

## Intended behavior
- The same equipped emote can be previewed in the lobby and triggered in BR/CS/Hybrid/Training when its mode rules allow it.
- Weapon showcase emotes require an active weapon visual.
- Combat-unsafe emotes are rejected while the player is in combat.
- This is a code foundation. It does not include finished rigged character animation clips, final art/audio, or a wired production UI yet.

## QA checklist
- Test every emote with and without a weapon equipped.
- Test all four game modes and combat-state restrictions.
- Verify Animator state names exist on each operator controller.
- Verify wheel open/close, all eight slots, rapid re-selection and scene unload.
- Profile animation and particle effects on the target Android device.
