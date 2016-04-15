unity-platformer (work name)
===

Based on https://github.com/SebLague/2DPlatformer-Tutorial evolve in it's own creature.

Features
* Manual update. Most of the problems in platformers are solved by sorting the updates
* Use `FixedUpdate` and no `Update`, is better for kinetic objects.
* fix MovingPlatform issues, that allow player to fall though
* fix while moving down-slope charcater cannot jump. Adding some distance to env,
for better collisions handling, that gap it's really useful.
* Add Ladders
* Add IA (Patrol, Projectiles, Jumpers, etc...)
* Add better input handling
* Character handle actions. Those actions are configured individially (Jump, Ladder, Air movement,
Ground movement, etc...)
* Add projectiles

TODO
* Dismount ladder conditions?
  * Left/Right
  * Jump out of a ladder as Action? / bool on Ladder

TODO (maybe)
* double jump
* Dash
* Attack range
* Attack melee
* states and area are must be in Controller2D



Known issues
* Player con move thought a horizontal moving platform by pushing it...
* Slope issues, this is just not very well handled yet.
* Jump logic need review and more features to tweak it.

# Usage

It's recommended to use this same project configuration (Tags/Layers) gives some
space for future additions if you need more.

Setup prefab allow yoy to configure most of the project. Drop it a try it.

# Manual Editable

The following classes contains staff that could be useful to edit for your game.

## Character.cs

Contains `Areas` and `States`. Both useful for new Actions/Movements.

## DamageType.cs

Contains `DamageTypes` Enum.
