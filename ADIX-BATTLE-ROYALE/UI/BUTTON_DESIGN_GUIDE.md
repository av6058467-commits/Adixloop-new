# ADIX BATTLE ROYALE — Custom Button Design Guide

Every interactive control in the game must use a consistent, custom-designed visual system. Do not leave default Unity UI buttons in the final interface.

## Visual direction
- Premium tactical battle-royale HUD.
- Dark graphite / gunmetal surfaces with restrained electric-blue highlights.
- Red is reserved for danger, damage, and destructive actions.
- Green is reserved for health, confirmation, and safe status.
- Strong contrast, readable labels, clear iconography, subtle depth and pressed-state feedback.
- Keep the HUD clean; do not duplicate actions or add decorative controls without purpose.

## Required button states
Every button must define:
1. Normal
2. Pressed (visible scale/depth response)
3. Selected / toggled (for aim, crouch, camera mode, etc.)
4. Disabled
5. Cooldown / unavailable where applicable

## Touch and accessibility
- Comfortable touch targets; aim for at least 48 dp where practical.
- Keep fire, aim, jump, crouch, reload and interact controls reachable on a phone.
- Support adjustable HUD layout and button size in settings.
- Add haptic feedback only where supported and user-configurable.
- Avoid tiny text; support safe areas and different Android aspect ratios.
- Do not rely on color alone; use icons and labels where clarity matters.

## Button groups
- Main menu: Play, Settings, Loadout, Exit.
- Match HUD: Fire, Aim, Reload, Jump, Crouch, Prone, Interact, Backpack, Map, Camera Switch.
- Vehicle HUD: Enter/Exit, Accelerate, Brake, Steering, Handbrake, Vehicle Seat Switch.
- Menus: Confirm, Back, Close, Resume, Restart.

## Acceptance checks
- One clear action per button; no duplicate controls.
- Icons and labels match the action.
- All states are visually distinct.
- Buttons remain usable in landscape on small Android screens.
- Test with touch input on the target Infinix Android device before release.
