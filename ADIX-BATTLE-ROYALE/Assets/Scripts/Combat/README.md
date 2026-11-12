# Combat systems

Player weapon uses a center-screen camera ray, magazine/reserve ammo, reload timing and the shared `IDamageable` contract. Bot health implements the same contract. Bot weapons resolve the first non-self obstruction before applying damage to the player.

Prefab requirements:
- Player: `PlayerHealth`, `PlayerWeapon`, camera assigned or tagged MainCamera.
- Bot root: `BotHealth`, `BattleBotBrain`, `NavMeshAgent`.
- Bot weapon child: `BotWeaponEmitter` with muzzle assigned and player layer included in hit mask.
- Match director: assign player, bot prefab, spawn points and optional progression manager.

The match director wires bot damage ownership and bot firing events at spawn time.
