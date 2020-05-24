# Fiptubat
A prototype for a first-person turn-based tactics game.

Turn-based tactics games such as XCOM, Silent Storm, Jagged Alliance, etc. are based around the idea of the player commanding a small group (e.g. a fireteam) in a small-scale combat scenario. Usually, these are done from a third-person perspective, i.e. the player is commanding the unit remotely. The purpose of this is to see if this can be done from a first-person perspective, i.e. from the point-of-view of the units on the field.

## Current Prototype Link
You can find the WebGL prototype [here](https://aceade.github.io/Fiptubat/Fiptubat/Releases/ProtoType/index.html)

The full controls will be displayed in the main menu. For the sake of convenience, they are posted here.
* Use the mouse or arrow keys to rotate the current unit. This will be used to aim later.
* Hold down right mouse to view possible positions; left click to set your destination.
* Cycle units using Tab
* End your turn using Backspace. If you have units moving, this will be rejected.
* Crouch or stand up using C.
* Strafe by pressing WASD
* Freeze or enable rotation by clicking left Ctrl.
* Fire by clicking the left mouse button
* Reload by pressing R
* Cycle fire modes with Q

### Known issues
The list of known issues is visible [here](https://github.com/aceade/Fiptubat/issues?q=is%3Aissue+is%3Aopen)

## Required systems/components
These are initial thoughts and may not be up-to-date. Not all systems will be covered here.

### BaseUnit
The unit itself. Has the following attributes:
* Health
* Action points
* Armour

Has the following components:
* LineOfSight (visual detection)
* WeaponBase (weapon)
* UnitVoice (voice lines, primarily for atmospheric purposes)
* TargetSelection (chooses targets. See the relevant section)
* CoverFinder (optional - some units don't care about cover)
* NavMeshAgent (optional, only used in moving enemies)

Implements the `IDamage` interface, which mandates the following functionality:
* Deal a certain amount of damage of the specified `DamageType`
* Return the implementer's Transform (for location purposes)
* Return the implementer's remaining health
* Get their potential damage output. This is intended to be used when selecting a target, where e.g. a sniper unit might decide to go for a target using a rocket launcher over one wielding a handgun.
* Announce that a bullet hit nearby. This will be used to provide suppression effects.

Player-controlled units will have the following additional components:
* PlayerUnitControl - handles input for the currently selected unit. This component is disabled when the unit is deselected.
* PlayerUnitDisplay - displays unit health, action points and ammo count.

### Unit manager
Allows user (player or bot) to change units. Requires the following functionality:
* Cycle units
* Select individual unit
* Return unit details (e.g. for UI purposes)
* Decide which unit to choose (AI-only)

### TargetSelection
Chooses a target. Has two implementations: `UnitTargetSelection` and `AdvancedTargetSelection`.

##### UnitTargetSelection
Base class. Hard-coded to returns the closest target.

##### AdvancedTargetSelection
Subclass of UnitTargetSelection. Allows for more intelligent target selection by using one of the following algorithms:
* Closest target (default method)
* Target with lowest remaining health
* Target with the highest remaining health
* Most exposed target
* Target with the highest damage output

### UIManager
Handles UI interactions (hiding/displaying panels, disable or enabling buttons)

### GameStateManager
Main controller for the game logic. Handles the following functionality:
* Pausing/resuming
* Swapping players
* Victory/defeat conditions
* Deciding when to change music

### Detection system
Line of sight and hearing. Line of sight is based on raycasting inside a collider. Sound detection based on distance inside a collider, not implemented yet.
Also contains methods to check if the specified target can be seen from a specified location.

### Weapon system
Manages the low-level weapon physics, animations, etc. Aiming is free-form for player units; computer-controlled units will select from a list of visible units.
Units may suppress targets by dumping ammo into the targets' surroundings.

### Sound manager
Manages music.

### Miscellaneous
* FallDeathTrigger: deal fatal damage to anything that enters it. This was intended to be in case some idiot walked off a cliff.
* DummyDamage: used when testing combat and dealing damage. The original intention was to drop it into the `FallDeathTrigger` to test that.
* ExtractionPoint: the player's goal. Once the last surviving unit enters this, the player wins.
* DieInstantlyTest: attached to an `IDamage` implementation to check what happens when a unit dies.
* TracerEffect: used to show where units are aiming. Could be converted into regular projectile instead of using hitscan.
* TimeBoundVictory: instant victory for one side. Used to test cleanup after one side wins.

## Current levels

1. Main menu. This has some basic scenery, with the menus and other panels visible as "grafitti" in the scene.
2. Main level. This is an industrial complex with a bridge down the far end. The extraction point is on the other side of the bridge; the player starts at the entrance to the yard.

### Possible future levels
1. Motorway bridge through a city. Lots of potential sniper roosts.
2. Sky docks. The Autarca has docked and is waiting for the player. Could be used to add more NPC units that are allied with the player.

## Third-party components used
These are **not** committed to the repo.

### Code
* Unity NavMeshComponents

### Models & Animations
* Kubold's [PistolAimsetPro](https://assetstore.unity.com/packages/3d/animations/pistol-animset-pro-15828) animation pack
* Autarca's [Dieselpunk Corvette](https://assetstore.unity.com/packages/3d/vehicles/air/dieselpunk-airship-corvette-131140) as an escape or scenery vehicle
* vUv's [Workshop Props](https://assetstore.unity.com/packages/3d/props/workshop-props-81112)
* [Container Collection](https://assetstore.unity.com/packages/3d/props/industrial/container-collection-750) by VIS Games

### Sounds
* Siren noises from [guitarguy](https://www.freesound.org/people/guitarguy1985/packs/3355/) (public domain)
* Beeps from [AlaskaRobotics](https://freesound.org/people/AlaskaRobotics/packs/14049/) (public domain)
* Background music:
    * Background loops: [Sirkoto51](https://freesound.org/people/Sirkoto51/packs/21233/) (CC BY 3.0)
    * "Theme for Sadistic.wav": [Grezgor](https://freesound.org/people/Gregzor/sounds/181827/) (CC0)
    * "Orchestral Victory Fanfare": [Sheyvan](https://freesound.org/people/Sheyvan/sounds/470083/) (CC BY 3.0)
* Laser guns from [Terry93D](https://freesound.org/people/Terry93D/packs/18390/) (public domain)
* Scifi gun noise pack from [AniCator](https://freesound.org/people/AniCator/packs/2524/) (CC BY 3.0)
* Propellor noises by [JillianCallahan](http://www.freesound.org/people/JillianCallahan/packs/671/) (Sampling+)

### Images
Cursor from [WikiMedia Commons](https://commons.wikimedia.org/wiki/File:Mouse_pointer.svg) (CC BY-SA 2.5)
Numerous textures from [textures.com](https://www.textures.com/)
Skybox textures from [AllSky](https://assetstore.unity.com/packages/2d/textures-materials/sky/allsky-200-sky-skybox-set-10109#content)

### Fonts
* [Archaic 1897](https://www.dafont.com/archaic1897.font) (c) 2015 Paul Davy
* [Ampad brush set](https://www.dafont.com/ampad.font), public domain(?) by Gene Gilmore
