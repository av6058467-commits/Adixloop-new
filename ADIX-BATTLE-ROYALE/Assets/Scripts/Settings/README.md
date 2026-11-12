# ADIX Settings Foundation

## Reference analysis
The design study uses publicly documented gameplay concepts from Free Fire and PUBG MOBILE:
- Separate graphics and FPS preferences, with device-dependent frame-rate availability.
- Per-scope sensitivity controls.
- Customizable HUD/button placement and saved layouts.
- Gyroscope and aim behavior options.
Sources: Garena Free Fire general settings guide; PUBG MOBILE Help Center sensitivity FAQ; Google Play PUBG settings guide. ADIX uses original naming, layout and code.

## Implemented in code
- Graphics preset enum: UltraSmooth, Standard, Ultra.
- Target FPS request setting (30–120; actual rate is limited by device, display refresh, Unity platform and thermal conditions).
- Camera, Red Dot, 2x, 4x and Sniper sensitivity values.
- Gyroscope mode and Hold/Toggle aim mode settings.
- PlayerPrefs persistence.
- Runtime application of Unity quality level, shadows and anti-aliasing.
- Runtime setters for camera rig and touch look sensitivity.

## Unity integration still required
1. Add an `AdixGameSettings` GameObject to the bootstrap scene.
2. Assign `ThirdPersonCameraRig` and `TouchLookPad` references.
3. Build original ADIX Settings UI tabs: Graphics, Sensitivity, Controls, Audio, Gameplay.
4. Bind UI sliders/dropdowns/toggles to public setter methods.
5. Add HUD editor for drag, resize, opacity and save 3 control layouts.
6. Implement per-scope sensitivity routing in the aiming/weapon camera code; stored scope values are not yet connected to ADS.
7. Add device capability detection and show only supported FPS options; targetFrameRate is a request, not a guaranteed FPS.
8. Test quality tiers, heat, battery, memory and frame pacing on target Android devices.

## Proposed ADIX graphics tiers
- Ultra Smooth: reduced shadows/effects and lower render scale for stability.
- Standard: balanced quality and performance.
- Ultra: highest quality profile allowed by device budget.

Do not promise Ultra or 90/120 FPS on every phone. Select a stable supported target after profiling. Render scale and motion-effects fields are profile data for the future renderer/UI integration; Unity's built-in pipeline does not automatically apply them in this foundation.
