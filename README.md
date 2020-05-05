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
* Hearing (TODO, see [#6](https://github.com/aceade/Fiptubat/issues/6) )
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
Handles UI interactions.

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
Units may be able to suppress targets by means of dumping ammo into the targets' surroundings.

### Sound manager
Manages music and environmental sounds.

### UI Manager
Manages the UI elements. Interface into player's unit manager and menu.

### Miscellaneous
* FallDeathTrigger: deal fatal damage to anything that enters it. Will be used in case some idiot walks off a cliff.
* DummyDamage: used when testing combat and dealing damage. Original intention was to drop it into the `FallDeathTrigger` to test that.
* ExtractionPoint: the player's goal.
* DieInstantlyTest: Attached to an `IDamage` implementation to check what happens when a unit dies.
* TracerEffect: used to show where units are aiming. Could be converted into regular projectile instead of using hitscan.

## Art style

* Basic 3D shapes and textures. 
    * Scenery will be greyscale; characters in red (player) or yellow (opponents). 
    * Enemy units will only be visible based on line-of-sight; there will not be an outline to aid the player in keeping track of them.

* Minimal UI – just enough to portray the required details.
    * Panel along the bottom edge of the screen will display 2D images for up to six characters. Each image will contain a button to select that unit. Health and action points will be reported as numbers instead of bars.
    * Represent state using basic faces: be smiley faces for normal, frowning for under fire, and various states of blood spatter to indicate damage.

## Required levels

1. Main menu. This will be very bare-bones - just links to the main level or the options panel.
2. Main level. This will involve a "corridor" through various buildings. The player's goal will be for at least one unit to reach the far end of the level (referred to henceforth as the "extraction point"). The AI player will attempt to stop them.

## Third-party components used
These are **not** committed to the repo.

### Code
* Unity NavMeshComponents

### Models & Animations
* Kubold's [PistolAimsetPro](https://assetstore.unity.com/packages/3d/animations/pistol-animset-pro-15828) animation pack
* Autarca's [Dieselpunk Corvetter](https://assetstore.unity.com/packages/3d/vehicles/air/dieselpunk-airship-corvette-131140) as an escape or scenery vehicle
* vUv's [Workshop Props](https://assetstore.unity.com/packages/3d/props/workshop-props-81112)
* [Container Collection](https://assetstore.unity.com/packages/3d/props/industrial/container-collection-750) by VIS Games

### Sounds
* Siren noises from [guitarguy](https://www.freesound.org/people/guitarguy1985/packs/3355/)
* Beeps from [AlaskaRobotics](https://freesound.org/people/AlaskaRobotics/packs/14049/)
* Background music:
    * Background loops: [Sirkoto51](https://freesound.org/people/Sirkoto51/packs/21233/)
    * "Theme for Sadistic.wav": [Grezgor](https://freesound.org/people/Gregzor/sounds/181827/)
    * "Orchestral Victory Fanfare": [Sheyvan](https://freesound.org/people/Sheyvan/sounds/470083/)
* Laser guns from [Terry93D](https://freesound.org/people/Terry93D/packs/18390/)
* Scifi gun noise pack from [AniCator](https://freesound.org/people/AniCator/packs/2524/)

### Images
* Smiley face: [WikiMedia Commonds](https://commons.wikimedia.org/wiki/File:Face-smile.svg) (public domain)
* Sad face: [WikiMedia Commonds](https://commons.wikimedia.org/wiki/File:Face-sad.svg) (public domain)
* Cursor: [WikiMedia Commonds](https://commons.wikimedia.org/wiki/File:Mouse_pointer.svg) (CC BY-SA 2.5)

Numerous textures from [textures.com](https://www.textures.com/)
Skybox textures from [AllSky](https://assetstore.unity.com/packages/2d/textures-materials/sky/allsky-200-sky-skybox-set-10109#content)

### Fonts
* [Archaic 1897](https://www.dafont.com/archaic1897.font) (c) 2015 Paul Davy
* [Ampad brush set](https://www.dafont.com/ampad.font), public domain(?) by Gene Gilmore
