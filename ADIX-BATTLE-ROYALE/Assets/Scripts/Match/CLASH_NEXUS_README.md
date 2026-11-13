# Clash Nexus — CS 4v4 Foundation

This is an original offline 4v4 round-flow foundation for ADIX. It does not copy another game's UI or assets.

## Included
- Team Orion and Team Aether, four roster slots each.
- Offline prototype fills empty positions with local AI placeholders.
- Team setup and ready-state validation.
- Buy phase with per-team credits and weapon purchase cost checks.
- Round start, round resolution, score tracking and win condition.
- Match reset and phase-change / score events for future UI binding.

## Not included yet
- Actual bot spawning and round combat integration.
- Network matchmaking, friend invites or remote players.
- Weapon inventory/equip UI and currency persistence.
- Arena scene, objective logic, UI screens, audio or animations.
- Unity compilation or device verification.

## Integration
1. Add the ClashNexusMatchDirector component to a CS match-root GameObject.
2. Bind Team Orion/Aether roster cards to GetRoster(team).
3. Call SetReady, StartBuyPhase, Purchase, StartRound, then ResolveRound.
4. Subscribe to PhaseChanged and ScoreChanged for the CS UI.
5. Do not describe bot placeholders as real multiplayer players.
