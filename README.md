# Fiptubat
A prototype for a first-person turn-based tactics game

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

### Unit manager
Allows user (player or bot) to change units. Requires the following functionality:
* Cycle units
* Select individual unit
* Return unit details (e.g. for UI purposes)

### AI controller
Effectively another player. Can choose which unit moves when (likely round-robin at first).

### Detection system
Line of sight and hearing. Line of sight based on raycasting.

### Damage system
Covers health of living and non-living objects (e.g. cover)

### Weapon system
Manages the low-level weapon physics, animations, etc.

### Sound manager
Manages music and environmental sounds

### UI Manager
Manages the UI elements. Interace into player's unit manager and menu



