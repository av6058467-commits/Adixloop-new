# ADIX BATTLE ROYALE — Bot AI foundation

This folder contains the first offline bot-progression and combat-AI scripts.

## Included
- `BotDifficultyTier.cs`: ScriptableObject parameters for each difficulty tier.
- `BotProgressionManager.cs`: stores the current level locally with PlayerPrefs and adjusts it after a group of matches.
- `BattleBotBrain.cs`: NavMesh-based target detection, chase, engage, search, and patrol foundation.

## Unity setup
1. Add these scripts to a Unity project using a compatible Unity version with the AI Navigation package.
2. Create 10 Bot Difficulty Tier assets from **Assets → Create → ADIX Battle Royale → AI → Bot Difficulty Tier**.
3. Set their level values from 1 to 10 and tune accuracy, reaction time, detection range, movement speed, combat range, health and damage multipliers progressively.
4. Add a NavMeshAgent and BattleBotBrain to each bot prefab. Bake a NavMesh for the walkable map.
5. Add patrol point transforms if the bot should patrol before seeing the player.
6. A match director must call `Configure(playerTransform, progressionManager.GetCurrentTier())` for each spawned bot.
7. Connect `ShotRequested` to the bot's weapon/shooting component. The weapon must use the supplied accuracy and damage multiplier and perform its own hit validation.
8. At match end, call `RecordMatch(kills, deaths, placement, aliveSeconds)` once.

## Progression behavior
The manager evaluates the average performance across the configured number of matches. Strong results raise the next match's bot level; repeated poor results can lower it. The level is saved locally, so no server or database is required.

## Important current limitations
This is source-code foundation, not a finished game. The match director, bot spawning, weapon implementation, health/damage system, animations, cover selection, coordinated flanking, squad logic, map, UI, and Android build pipeline still need to be built and tested. The tier asset values must be authored in Unity; the scripts do not automatically create balanced tier assets. Use mobile profiling before enabling large numbers of active NavMesh agents.
