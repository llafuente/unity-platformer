# unity-platformer
===

Based on https://github.com/SebLague/2DPlatformer-Tutorial evolve
in it's own beast.

## Features
* Ladders
* IA (Patrol, Projectiles, Jumpers, etc...)
* Input abstraction (this will help if you add a pad or touch controls)
* Character-handled-actions. Instead of a big class mapping all actions,
each Action (Jump, Climb ladder, ground/air movement) it's a separate component.
* Projectiles

## Changes from the original & Thigs to know
* Manual update: Most of the problems in platformers are solved by
sorting updates. UpdateManager is a singleton part of `Setup` prefab
* Use `FixedUpdate` and no `Update`, is better for kinetic objects.
* fix MovingPlatform issues, that allow player to fall though
* fix while moving down-slope character cannot jump.


## Known issues

* Bug: Player con move thought a horizontal moving platform by pushing it.
* Bug: Slope issues when the slope rotates, even climb up without player input.

# TODO
* Dismount ladder conditions?
  * Left/Right
  * Jump out of a ladder as Action? / bool on Ladder
* Use AI to do some automated-test.
* Attack melee

## Whishlist
* N-jump (double/triple/n jump) with different heights
* Dash
* Rope
* Ledge


## Caveats

### Jump: hang time

If you want hanging in max Jump (like Peach in Mario Bross) with a little
up/down movement, like the original, you have to do it in your animation.

It can't be done at CharacterActionJump, because Falling state will be set,
when moving down and it will be impossible to animate.

### Jump: grace jump time

Could be unexpected to see a double-jump just by increasing graceJumpTime.
This value should be low enough: 0.2f

# Usage

It's recommended to use the same project configuration (Tags/Layers).
But it's not necessary, `Setup` prefab allow you to configure most of
the project properties. Drop it a try it.

Then you can drop `Basic Player` and start working.
There are many test scenes to see how things are built.

Most of the components are very reusable-chainable. The best example
is IA, that it's implemented as a fake Input for a normal Character.

# Manual Editable

The following classes contains stuff that could be useful to edit for your game.

## Character.cs

Contains `Areas` and `States`. Both useful for new Actions/Movements/Animation.

## DamageType.cs

Contains `DamageTypes` Enum.
