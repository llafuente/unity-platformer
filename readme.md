# unity-platformer (work name)
===

Based on https://github.com/SebLague/2DPlatformer-Tutorial evolve in it's own beast.

## Features
* Ladders
* IA (Patrol, Projectiles, Jumpers, etc...)
* Input abstraction (this will help if you add a pad or touch controls)
* Character-handled-actions. Instead of a big class mapping all actions, each Action (Jump, Climb ladder, ground/air movement) it's a separate component.
* Projectiles

## Changes from the original & Thigs to know
* Manual update. Most of the problems in platformers are solved by sorting the updates.
* Use `FixedUpdate` and no `Update`, is better for kinetic objects.
* fix MovingPlatform issues, that allow player to fall though
* fix while moving down-slope character cannot jump.


## Known issues / TODO

* Bug: Player con move thought a horizontal moving platform by pushing it...
* Bug: Slope issues the slope rotate in time.
* Dismount ladder conditions?
  * Left/Right
  * Jump out of a ladder as Action? / bool on Ladder
* Use AI to do some automated-test.
* Attack melee

## Whishlist
* N-jump (double jump) with different heights
* Dash
* Rope
* Ledge


## Caveats / FAQ

This list is not necessary limitations, also contains things
you should know about how to do advanced things

* If you want hanging in max Jump (like Peach in Mario Bross) with a little
up/down movement, like the original, you have to do it in animation.

  You can't do it in the Character/Action, otherwise Falling state will be set, and you will have a hard time Animating.

# Usage

It's recommended to use the same project configuration (Tags/Layers).
If you need more gives some space for my future additions.

`Setup` prefab allow you to configure most of the project. Drop it a try it. So it's not completely necessary.

Then you can drop `Basic Player` and start working.
There are many test scenes to see how things are built.

# Manual Editable

The following classes contains stuff that could be useful to edit for your game.

## Character.cs

Contains `Areas` and `States`. Both useful for new Actions/Movements/Animation.

## DamageType.cs

Contains `DamageTypes` Enum.
