# ADIX BATTLE ROYALE — Godot 4 prototype

Separate Godot 4 project. Early prototype only; not a finished game or verified APK.

## Authored prototype systems
- Runtime-generated test arena, cover and simple trees
- Full-body procedural operator model with head, torso, arms, legs and boots
- Third-person follow camera framed to show the full character
- Procedural idle, walk/run limb swing and jump pose
- Desktop movement, mouse look and touch movement/look controls
- Eight rival bots with simple chase/attack behavior
- Hitscan shooting, bot health and elimination counter
- Shrinking safe-zone ring and damage outside the radius
- Health pickups
- Sidearm and ADIX-R1 test weapon
- Three firearm slots plus a dedicated FISTS melee slot, with tap-to-select HUD, active-slot highlight and ammo readout
- Weapon switching via slot buttons, Q / TAB or SWAP button
- Magazine ammo, reserve ammo, reload timer and ammo pickups
- Victory / Defeat result panel with retry and exit controls
- Mobile HUD buttons with original icon labels for FIRE, RELOAD, SWAP, JUMP and camera ZOOM
- Reference-inspired mobile HUD: minimap/team strip, compass, live/kill indicators, ammo card, three gun slots plus fist, movement stick, health bar and combat controls
- HUD customization: select and drag controls, resize and adjust opacity, save layout to device storage, and reset layout
- Some visual controls (throwable, crouch/prone, team/minimap indicators) are currently presentation placeholders; their gameplay systems are not implemented
- Top compass heading indicator with cardinal directions and degree readout

## Open
1. Install Godot 4.x from https://godotengine.org/download/
2. Import this folder as a project.
3. Open and run scenes/Main.tscn.

## Important
This code has not yet been opened, compiled or device-tested in Godot in this environment. The mobile controls are a first implementation and require real-device tuning. Final weapon models/animations, polished map, sound, menus, Android export setup and APK export are still outstanding.

## Next
Run the project in Godot 4 editor, fix any parser/runtime errors, tune controls on Android, then prepare and test an APK.
