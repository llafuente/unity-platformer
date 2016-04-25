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

### One Way Platforms (Height)

The collider try to be simple enough. If OWP are thick you will see
characters "climbing" when start falling and their feet are inside the OWP.

OWP Height should be aounr Character.skinWidth ~ 0.1.

## FAQ

### I see some null access at the begining.

Most provably caused by UpdateManager being initialized after other scripts.
<a href="#execution-order">Follow this instructions to fixed it.</a>

# MovingPlatforms don't always detect players

Moving platforms use Raycast to detect passengers. If the Movingplatform is
big, you need to increase the ray count to a number big enough that the
minimum width of the player could be covered anytime.

# Usage

It's recommended to use the same project configuration (Tags/Layers).
But it's not necessary, `Setup` prefab allow you to configure most of
the project properties. Drop it a try it.

Then you can drop `Basic Player` and start working.
There are many test scenes to see how things are built.

Most of the components are very reusable-chainable. The best example
is IA, that it's implemented as a fake Input for a normal Character.

[<a name="execution-order"]
## Execution Order

Configuration & UpdateManager need to be executed first, as they are
Singletons, many classes rely on them being initialized first.

Go to: Edit/ProjectSettings/Script Execution Order and add them with negative
priority.

This is already done in this project, Just for reference is you integrate
the codebase in your project.

## Manual Editable

The following classes contains stuff that could be useful to edit for your game.

### Character.cs

Contains `Areas` and `States`. Both useful for new Actions/Movements/Animation.

### DamageType.cs

Contains `DamageTypes` Enum.
