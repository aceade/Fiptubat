# Fiptubat
A prototype for a first-person turn-based tactics game.

Turn-based tactics games such as XCOM, Silent Storm, Jagged Alliance, etc. are based around the idea of the player commanding a small group (e.g. a fireteam) in a small-scale combat scenario. Usually, these are done from a third-person perspective, i.e. the player is commanding the unit remotely. The purpose of this is to see if this can be done from a first-person perspective, i.e. from the point-of-view of the units on the field.

## Current Prototype Link
You can find the WebGL prototype [here](https://aceade.github.io/Fiptubat/Fiptubat/Releases/ProtoType/index.html)

## Required systems/components

### Unit
The unit itself. Has the following attributes:
* Health
* Action points
* Aim

Has the following components:
* Hearing
* Eyesight
* Weapons
* Comm system
* Voice

Player-controlled units will have a camera attached.

### Unit manager
Allows user (player or bot) to change units. Requires the following functionality:
* Cycle units
* Select individual unit
* Return unit details (e.g. for UI purposes)
* Decide which unit to choose (AI-only)

### GameStateManager
Main controller for the game logic. Handles the following functionality:
* Pausing/resuming
* Swapping players
* Victory/defeat conditions
* Deciding when to change music

### AI controller
Effectively another player. Can choose which unit moves when (likely round-robin at first).

### Detection system
Line of sight and hearing. Line of sight based on raycasting inside a collider. Sound detection based on distance inside a collider.

### Damage system
Covers health of living and non-living objects (e.g. cover)

**Open questions**
* How does cover affect combat?

### Weapon system
Manages the low-level weapon physics, animations, etc. Aiming will be free-form for player units; computer-controlled units will select form a list of visible units.
Units may be able to suppress targets by means of dumping ammo into the targets' surroundings.

### Sound manager
Manages music and environmental sounds.

### UI Manager
Manages the UI elements. Interface into player's unit manager and menu.

## Art style

* Basic 3D shapes and textures. 
    * Scenery will be greyscale; characters in red (player) or yellow (opponents). 
    * Enemy units will only be visible based on line-of-sight; there will not be an outline to aid the player in keeping track of them.

* Minimal UI – just enough to portray the required details.
    * Panel along the bottom edge of the screen will display 2D images for up to six characters. Each image will contain a button to select that unit. Health and action points will be reported as numbers instead of bars.
    * Represent state using basic faces: be smiley faces for normal, frowning for under fire, and various states of blood spatter to indicate damage.

## Sound effects

Sounds will consist of character voices (confirmations, comments and chatter) and gun effects. The voices will be done using text-to-speech.

Music TBD

## Required levels

1. Main menu. This will be very bare-bones - just links to the main level or the options panel.

2. Main level. This will involve a "corridor" through various buildings. The player's goal will be for at least one unit to reach the far end of the level (referred to henceforth as the "extraction point"). The AI player will attempt to stop them.

## Third-party components used

* Unity NavMeshComponents
