# Offline match foundation

## Scene wiring
1. Create a Unity scene named `BattleRoyale`.
2. Add an empty GameObject named `MatchDirector` and attach `BattleRoyaleMatchDirector`.
3. Assign the player Transform, a bot prefab, spawn point Transforms, and the BotProgressionManager.
4. Set botCount to 50 (lower it during mobile performance tests).
5. The bot prefab needs a NavMeshAgent and BattleBotBrain. Bake a NavMesh over walkable terrain.
6. Call `StartOfflineMatch()` from the Start Match UI button.
7. Add `BotHealth` to the bot prefab and assign the MatchDirector. Connect weapon hit detection to `TakeDamage`.
8. Call `NotifyBotEliminated` only through BotHealth on elimination.

This is a code foundation. It does not create the scene, terrain, player controller, weapons, animations, UI, assets, or APK automatically.
