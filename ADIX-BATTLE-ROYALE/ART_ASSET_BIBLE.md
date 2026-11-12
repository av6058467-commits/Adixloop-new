# ADIX BATTLE ROYALE — Art Asset Bible
Version: 1.0
Target: Unity 2022.3 LTS / Android ARM64
Visual target: the approved ADIX BATTLE ROYALE concept artwork in this conversation.
Purpose: production specification for real in-game 3D assets. Concept images are references, not runtime assets.

## Visual identity
- Premium, grounded, realistic battle-royale art direction.
- Physically based materials (metallic/roughness workflow), baked normal maps, ambient occlusion, clean LODs.
- Consistent scale, shared texel density, restrained saturation, readable silhouettes at phone-screen size.
- No copyrighted franchise characters, logos, weapon skins, or copied branded vehicle designs.

## Character roster
Build four original playable operator archetypes matching the approved concept's silhouettes and palette:
1. Frost: pale/white-haired female operator; cool grey/white tactical outfit; light armour; dark boots; compact utility rig.
2. Shadow: dark-haired male operator; charcoal tactical clothing; layered vest; gloves; utility belt.
3. Ember: red-haired male operator; dark red/black tactical layers; reinforced shoulder and knee protection.
4. Volt: female operator; yellow/black field jacket; tactical trousers; compact backpack and gloves.

Each operator requires:
- Separate head, hair, body, clothing, boots, gloves, backpack and accessory meshes where practical.
- PBR base-color, normal, metallic, roughness and ambient-occlusion textures.
- Rigged humanoid skeleton; idle, walk, run, sprint, jump, land, crouch, prone, aim, fire, reload, hit, death and vehicle enter/exit animations.
- First-person arms/weapon rig and third-person full-body rig.
- LOD0/LOD1/LOD2, colliders, animator controller, avatar, materials and prefab.
- Character select portrait and in-game icon.

## Weapon family
Original, unbranded weapon designs:
- Assault rifle, SMG, shotgun, marksman rifle, sniper rifle, pistol, melee tool and throwable grenade.
Each weapon requires:
- Receiver, barrel, stock, magazine, sights, grip and attachment sockets as separate components.
- PBR textures, normal map, roughness/metallic maps, muzzle flash socket, shell-ejection socket, audio events and recoil profile.
- Idle/aim/fire/reload/inspect animations, hit effects, projectile or hitscan tuning, pickup icon and world pickup prefab.
- Mobile-friendly LODs and collision mesh.

## Vehicle family
1. Off-road SUV
2. Sports coupe
3. Dirt bike
4. Lightweight buggy
5. Military transport truck
6. Utility helicopter

Each vehicle requires:
- Original unbranded body design; separate wheels/rotors, glass, lights, doors and interior.
- PBR paint, rubber, glass, metal and dirt materials.
- Wheel/rotor animation, suspension, steering, brake lights, damage states, enter/exit points, seats and audio.
- Low-poly collision mesh, LODs, mobile-friendly shadows and vehicle HUD integration.
- Helicopter additionally needs rotor blades, lift/collective/yaw controls, landing gear, camera and rotor audio.

## Island world
Zones:
- Dense coastal city with streets, alleys, rooftops and enterable selected buildings.
- Military base with hangars, watchtowers, barracks and loot rooms.
- Mountain ridge, forest, river, beach, bridges and rural compounds.
- Roads and off-road routes that connect all named zones.
- Playable shoreline, ocean boundary, sky, clouds, foliage, rocks and terrain decals.
- Loot containers, doors, cover props, destructible presentation props and navigation blockers.

World material set:
- Grass, forest floor, sand, asphalt, concrete, painted metal, corrugated roof, brick, glass, rock, water and muddy ground.
- Each surface uses tiling PBR maps and terrain blending where appropriate.
- Daylight baseline, atmospheric fog, baked lighting for static structures, selective realtime lights for muzzle flashes/explosions.

## Effects and audio
- Muzzle flash, bullet tracer, impact sparks/dust, bloodless hit feedback, grenade blast, smoke, fire, vehicle dust, water splash, zone warning and supply-drop smoke.
- Separate audio for weapon fire/reload, footsteps by surface, vehicles, helicopter rotor, UI, ambience and match announcements.
- Pool particle systems and audio sources; avoid unbounded runtime instantiation.

## HUD and UI
- Main menu, operator selection, loadout, settings, pause, match start, minimap, compass, health/armour, ammo, weapon slots, backpack, loot, kill feed, safe-zone warning, vehicle controls, results and restart.
- Touch targets sized for Android; all buttons have normal, pressed, selected, disabled and cooldown states.
- Third-person/first-person toggle and configurable sensitivity.
- No duplicate controls; clear left movement / right look / action grouping.

## Performance and delivery gates
- Target Android 16, ARM64, 8 GB RAM class device.
- Use ASTC where supported with ETC2 fallback; compress audio; stream large environment bundles only if offline availability is preserved.
- LODs, occlusion culling, GPU instancing, baked lighting, texture atlases and pooled effects.
- Maintain stable frame pacing and thermal limits; quality presets for low/medium/high.
- Installed-size target 500 MB–1 GB only when filled by genuine game content; never add padding.
- Every asset must pass: import validation, material validation, prefab validation, collider check, LOD check, animation check, mobile memory check and in-device visual test.

## Required Unity folder structure
Assets/Art/Characters/{Frost,Shadow,Ember,Volt}/
Assets/Art/Weapons/{AssaultRifle,SMG,Shotgun,Marksman,Sniper,Pistol,Melee,Throwable}/
Assets/Art/Vehicles/{SUV,SportsCoupe,DirtBike,Buggy,MilitaryTruck,Helicopter}/
Assets/Art/Environment/{Terrain,City,MilitaryBase,Forest,Mountain,Coast,Props}/
Assets/Art/Materials/
Assets/Art/Textures/
Assets/Art/Animations/
Assets/Art/VFX/
Assets/Audio/{Weapons,Footsteps,Vehicles,Ambience,UI}/
Assets/Prefabs/{Characters,Weapons,Vehicles,World,UI}/

## Honest production status
This document is the complete visual production specification. It does not claim that the listed meshes, texture maps, animation clips, audio files or prefabs have already been created. Those must be authored/imported, validated in Unity, and tested on the target Android device before the game can be called visually complete.
