# ADIX BATTLE ROYALE — Game Vision

## Product goal
Build a distinctive, premium-feeling offline mobile battle royale. The ambition is to compete on moment-to-moment gameplay quality, not copy another game's identity, maps, UI, characters, sounds, or assets.

## Signature pillars
1. **Adaptive AI rivals** — 50-bot target, with difficulty tiers that evolve from match performance. Higher tiers should improve decision-making, positioning, teamwork and aim discipline, not simply gain unfair health or damage.
2. **Living battlefield** — a large island with a dense city, smaller settlements, hills, forest, coastline and meaningful tactical routes. Weather and time-of-day changes are later goals.
3. **Tactical combat** — responsive movement, readable recoil, distinct weapon roles, meaningful reloads, armor and healing, clear hit feedback.
4. **Vehicle sandbox** — land vehicles first; helicopter after flight controls, physics, camera and mobile performance are stable.
5. **Custom mobile HUD** — coherent premium tactical visual language; every control has normal, pressed, selected, disabled and cooldown states. Allow HUD layout and size customization.
6. **Offline-first** — no server or account required for the core solo mode; local progression and settings.
7. **Performance modes** — Low, Balanced and High graphics presets; dynamic resolution and effects scaling; target stable frame pacing on Android before increasing visual effects.

## Content size and visual-quality target
- **Target installed game size: 500 MB–1 GB.**
- Build toward this range with genuine premium content: detailed original island/city environments, character and vehicle models, weapon models, high-quality textures, animations, effects, voice/audio and music.
- Keep the core match fully offline: required maps, gameplay assets and audio must be included locally, not dependent on downloading content during play.
- Use sensible texture/audio compression and asset reuse; do not add empty or duplicated files just to inflate size.
- Report both APK/AAB download size and installed size separately because compression and Android packaging make them different.
- Preserve Low/Balanced/High graphics options so the premium assets can scale to the target Android phone.

## First playable milestone
- One compact test area, not the full island
- One player character with third-person movement and camera
- Basic first-person camera toggle
- One working weapon with ammo, reload, hit detection and damage
- A small number of bots using NavMesh, then scale toward 50 after profiling
- Health, armor, elimination and match-end loop
- Custom touch HUD and pause/settings menu
- Android APK built and tested on a real device

## Quality gates
- Do not call a feature complete until it is wired into a scene and tested.
- Profile CPU, GPU, memory and frame rate on target Android hardware.
- Scale bot count gradually (e.g. 5, 10, 20, then 50); 50 is a target, not a guaranteed safe count.
- Use original or properly licensed art, animation, audio and UI assets.
- Keep a playable build available at each milestone once the Unity project and cloud build pipeline exist.

## Honest current status
The repository currently contains early C# foundations and design notes. It is not yet a complete Unity project, playable build, or APK. Scenes, prefabs, assets, input wiring, weapon implementation, map, Android configuration and real-device QA remain to be done.
