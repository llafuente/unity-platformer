unity-platformer (work name)
===

It's based on https://github.com/SebLague/2DPlatformer-Tutorial

Expanded fixing some edge cases and improving overall experience.
* Manually handle update order. Also do not use Update -> LateUpdate is better
for kinetic objects
* fix MovingPlatform issues, that allow player to fall though
* fix move down-slope and not jumping.  
* add ladder (very simple)
* add some distance to env, to better handle collisions, that gap it's really useful.

TODO
* Jump out of a ladder
* climb down a ladder (can be done if the ladder is a bit `above` the ground)
* Create a CharacterInput, that manage and map inputs.
* Refactor movements as separate components.


TODO (maybe)
* double jump
* Dash
* Attack range
* Attack melee
* states and area are must be in Controller2D



Known issues
* Player con move thought a horizontal moving platform by pushing it...
* Fall down ladders
* Small jump when leave ladders (top)



# Editable

The following classes contains staff that could be useful to edit for your game.

## Configuration.cs

Contains some static const values. (maybe in the future can be attached
to the scene)

## DamageType.cs

Contains `DamageTypes` Enum.
