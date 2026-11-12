# ADIX BATTLE ROYALE — Audio Asset & Integration Guide

## Runtime audio hooks added
- `Assets/Scripts/Audio/GunAudioController.cs`: unique fire, reload-start, reload-finish and empty-magazine clips per weapon profile; 3D positional audio.
- `Assets/Scripts/Audio/VehicleEngineAudio.cs`: idle/running engine loop, startup, brake and horn hooks; engine pitch/volume respond to speed.
- `Assets/Scripts/Audio/HelicopterRotorAudio.cs`: spatial rotor loop with power/pitch control and startup cue.
- `PlayerWeapon.cs` calls fire/reload/empty hooks.
- `VehicleDrive.cs` calls engine start and brake hooks.
- `HelicopterController.cs` drives rotor power and startup cue.

## Required unique clip sets
| Item | Required clips |
|---|---|
| Assault Rifle | fire loop/shot, reload start, reload finish, empty |
| SMG | its own rapid-fire clip, reload start/finish, empty |
| Shotgun | its own heavy blast, shell/pump reload, empty |
| Sniper | its own bolt-action report, reload start/finish, empty |
| Pistol | its own report, magazine reload, empty |
| Marksman | distinct semi-auto report and reload set |
| SUV | diesel/petrol idle, acceleration loop, startup, brake, horn |
| Sports Car | sports idle, high-rev acceleration, startup, brake, horn |
| Dirt Bike | motorcycle idle/rev loop, startup, brake |
| Buggy | light off-road engine idle/rev, startup, brake |
| Military Truck | heavy diesel idle/rev, startup, air-brake |
| Helicopter | rotor loop, rotor startup/spool-up |

## Important production status
The C# audio controllers and gameplay hooks are committed, but actual licensed audio recordings/clips have not yet been imported or assigned. Until those clips are supplied and configured on the prefabs in Unity, the game will not magically have final unique sounds. Use original recordings or properly licensed sound assets; do not ship copyrighted game rips. Configure AudioSource spatial blend to 1 (3D) and tune each clip's volume/rolloff in the Inspector.

## Suggested project folder
`Assets/Audio/Weapons/<WeaponName>/`
`Assets/Audio/Vehicles/<VehicleName>/`
`Assets/Audio/Helicopter/`
