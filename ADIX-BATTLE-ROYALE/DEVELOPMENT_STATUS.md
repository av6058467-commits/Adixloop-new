# ADIX BATTLE ROYALE — Development Status

## Current milestone: offline gameplay foundation

### Source systems committed
- [x] Bot difficulty tiers and local performance-based progression
- [x] Bot AI foundation: detection, chase, engage, search and patrol
- [x] Match director spawning configurable bot count (target: 50)
- [x] Bot/player health and shared damage interface
- [x] Player hitscan weapon, magazine, reserve ammo and reload
- [x] Mobile player movement, camera mode and touch-input foundations
- [x] Bot weapon event hookup and self-collider filtering
- [x] Shrinking safe-zone logic and outside-zone damage
- [x] Health, armor and ammo pickup logic
- [x] Rigidbody/WheelCollider vehicle driving and seat enter/exit
- [x] Arcade helicopter flight controller
- [x] Bot tactical cover/flank decisions with NavMesh sampling
- [x] Mobile HUD bridge for vehicle throttle, steering, brake and helicopter axes
- [x] Grenade throw and timed radial damage action
- [x] Medkit healing action with limited charges
- [x] Match victory/defeat overlay foundation
- [x] Demo scene wired with grenade/medkit buttons and 50-bot target count
- [x] Unity project package manifest and editor-version metadata

### Still required before a playable 90% demo
- [ ] Complete Unity scenes, prefabs, materials, models, animations and audio
- [ ] Bake and validate island/city NavMesh; tune bot navigation and combat
- [ ] Implement cover/flanking behavior and bot variety across all tiers
- [ ] Finish HUD, touch layout, full inventory/weapon switching, loot spawning and restart flow
- [ ] Finish car/helicopter seat camera transitions and fully bind HUD widgets
- [ ] Add map geometry, safe-zone ring visuals and environment art
- [ ] Configure Android player settings, icons, permissions and build pipeline
- [ ] Compile in Unity, resolve all compiler/runtime errors
- [ ] Profile memory, heat and FPS on the target Android phone
- [ ] Build and install an APK; complete full match play-test

## Verification honesty
The checked items above are source-code foundations only. Unity Editor compilation, scene/prefab integration, Android APK generation and real-device gameplay have **not** been verified. Therefore this is not yet a playable demo and must not be presented as 90% complete.

### Original emote system foundation added
- [x] Emote definition assets with categories, icons, animation state, duration, loop and mode availability
- [x] Runtime emote controller with combat/weapon checks, timed stop and start/end events
- [x] Eight-slot emote wheel presenter with equip/select and safe listener cleanup
- [ ] Author original rigged animation clips (Tornado Royal Entry, Gun Showcase, Weapon Inspect, Victory Reload, Air Pose)
- [ ] Wire emote preview/equip UI into lobby and wheel into BR/CS/BRCS/Training HUD
- [ ] Add original emote icons, VFX, sound and mission-coin unlock inventory
- [ ] Test every emote on all operator rigs and Android device

### Nearby target assist foundation added
- [x] Optional Auto Aim and Auto Fire switches with nearby hostile detection, aim cone, range and line-of-sight checks
- [ ] Bind switches and sliders in gameplay settings/HUD
- [ ] Configure hostile target layers and test bot target filtering
- [ ] Tune aim speed, range and fire interval for fair offline gameplay on Android
- [ ] Compile and device-test; not yet Unity-verified

### Settings foundation added
- [x] Graphics preset foundation: Ultra Smooth, Standard, Ultra
- [x] Target FPS request setting (30–120; device may cap actual FPS)
- [x] Camera, Red Dot, 2x, 4x and Sniper sensitivity storage
- [x] Gyroscope and aim-mode settings storage
- [x] PlayerPrefs persistence and runtime camera/touch sensitivity application
- [ ] Build and bind Settings UI tabs and controls
- [ ] Connect scope sensitivity values to actual ADS camera/weapon logic
- [ ] Build draggable HUD editor and save multiple layouts
- [ ] Device capability detection and Android performance/thermal profiling
- [ ] Unity compile and device testing
