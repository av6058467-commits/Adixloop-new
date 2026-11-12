# World gameplay systems

- `SafeZoneController`: shrinking radius, outside-zone damage, optional scaled ring visual.
- `LootPickup`: health, armor and reserve-ammo pickups.
- `VehicleDrive`: Rigidbody + WheelCollider arcade driving.
- `VehicleSeat` and `VehicleInteraction`: nearest-seat enter/exit flow.

## Unity setup
1. Add a trigger collider and a `LootPickup` component to each pickup prefab.
2. Add Rigidbody, WheelColliders, `VehicleDrive`, and one or more `VehicleSeat` anchors to each car prefab.
3. Assign the player root, CharacterController, and MobilePlayerController to `VehicleInteraction`.
4. Assign the player and a flat circular ring mesh to `SafeZoneController`.
5. Bake a NavMesh for the island/city walkable surfaces before spawning bots.

These are gameplay scripts; prefabs, meshes, animations, sound, scenes and device profiling remain separate integration work.
