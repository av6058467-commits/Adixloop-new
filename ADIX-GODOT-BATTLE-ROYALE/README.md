# ADIX BATTLE ROYALE — Godot 4 prototype

Separate Godot 4 project. Early prototype only; not a finished game or verified APK.

## Authored prototype systems
- Runtime-generated test arena, cover and simple trees
- Third-person player capsule and camera
- Desktop movement, mouse look and touch movement/look controls
- Eight rival bots with simple chase/attack behavior
- Hitscan shooting, bot health and elimination counter
- Shrinking safe-zone ring and damage outside the radius
- Health pickups
- Sidearm and ADIX-R1 test weapon
- Two weapon slots with direct tap-to-select HUD, active-slot highlight and ammo readout
- Weapon switching via slot buttons, Q / TAB or SWAP button
- Magazine ammo, reserve ammo, reload timer and ammo pickups
- Victory / Defeat result panel with retry and exit controls
- Mobile HUD buttons with original icon labels for FIRE, RELOAD, SWAP, JUMP and camera ZOOM
- NEW HUD Customize panel with edit mode for dragging combat buttons
- Top compass heading indicator with cardinal directions and degree readout

## Open
1. Install Godot 4.x from https://godotengine.org/download/
2. Import this folder as a project.
3. Open and run scenes/Main.tscn.

## Important
This code has not yet been opened, compiled or device-tested in Godot in this environment. The mobile controls are a first implementation and require real-device tuning. Final weapon models/animations, polished map, sound, menus, Android export setup and APK export are still outstanding.

## Next
Run the project in Godot 4 editor, fix any parser/runtime errors, tune controls on Android, then prepare and test an APK.
