# unity-platformer
===

Based on https://github.com/SebLague/2DPlatformer-Tutorial evolve
in it's own beast.

## Features
* Moving platforms
* One way platforms
* Ladders
* AI (Patrol, Projectiles, Jumpers, etc...)
* Input abstraction (this will help if you add a pad or touch controls)
* Character-handled-actions. Instead of a big class mapping all actions,
each Action (Jump, Climb ladder, ground/air movement) it's a separate component.
* Projectiles
* Melee attacks
* Wallstick/WallJump
* Update everything in order (UpdateManager).

  This allow total control over who is updated first and act accordingly

* Slopes
* Water (liquid)

## Known issues

[https://github.com/llafuente/unity-platformer/labels/bug](https://github.com/llafuente/unity-platformer/labels/bug)

## TODO

[https://github.com/llafuente/unity-platformer/issues?q=is%3Aissue+is%3Aopen+label%3Aenhancement](https://github.com/llafuente/unity-platformer/issues?q=is%3Aissue+is%3Aopen+label%3Aenhancement)

## Caveats

[https://github.com/llafuente/unity-platformer/labels/caveat](https://github.com/llafuente/unity-platformer/labels/caveat)

## Character Setup

### Body

#### Physics body (BoxCollider2D: used for collisions)

How much the real body extend, used to calculate collisions, raycast, etc.

#### Real body (HitBox: EnterAreas)

This is the body used to check where the character is in Ladders, Grabbables, Liquids...

#### HitBox: Recieve damage / Deal Damage

Where Character recieve or deal damage. *NOTE*: Cannot overlap.


### Actions

* GroundMovement

  Ground horizontal movement. keyboard: right-left while OnGround

  *NOTE*: Slope rules are not here, are part of PlatformCollider2D

* AirMovement

  Airborne horizontal movement. keyboard: right-left while not "OnGround"

* Jump

  Variable Jump, also handle the rest of custom jumps. keyboard: space

* Grab

  When Character enter a Grabbable area, first position player in the "desired position".

  Then Stop all movement.

  To Dismount press Down, or jump.

* Ladder

  When a Character enter a Ladder and is pressing Up/Down, start climbing the
  ladder. keyboard: up/down

  Dismount pressing left/right or jumping.

* LiquidMovement

  When a Character enter "enough" in a Liquid Liquid the liquid will
  oppose gravity and give the Bouyancy sensation.

  Character can move horizontally and exit Jumping or runnning in a slope.

* Melee

  When an action (button) is held perform and attack.

  It's basically a succession of DamageArea overtime.

* MovingPlatforms

  Fall through platforms while pressing down.

* Projectile

  Fire projectiles with given offest and difference fire modes.

* Slipping

  Slip when slope can't be climbed.

  While slipping player cannot move horizontally, it's forced down slope.

* WallStick

  Stick to walls, and perform wall-jumps.

* Push

  Push objects (Box)

  *NOTE*: Require CharacterActionGroundMovement


### Input

### DefaultInput

Keyboard and CnControls.

### AIInput

Fake input used by AI to perform actions.
This make very easy to code AI because you just need to think in key press/up,
and configure the AI the same way you main character works.



## FAQ

### Compilation errors

See [External libraries](#external-libraries)
Just disable some `#def`

### I see some null access at the begining.

`Setup` prefab is mandatory. Did you forget it?

If you dont use our project settings, most probably caused by
`UpdateManager` being initialized after other scripts.

See <a href="#execution-order">Usage - Execution Order</a> for instructions.

### MovingPlatforms don't always detect players

Moving platforms use Raycast to detect passengers. If the moving platform is
big, you need to increase the ray count to a number big enough that the
minimum width of the player could be covered anytime.

## Usage

It's recommended to use the same project configuration (Tags/Layers).
But it's not necessary, `Setup` prefab allow you to configure most of
the project properties. Drop it a try it.

Then you can drop `Basic Player` and start working.
There are many test scenes to see how things are built.

Most of the components are very reusable-chainable. The best example
is IA, that it's implemented as a fake Input and depends on what
action you add you can achieve many different behaviors with no extra code.

<a name="external-libraries"></a>
### External libraries

`unity-platformer` don't require any external library, but I used in my
projects and shared the code here.

When a class require a library everything is enclosed inside `#if/endif`
that you will find in the first line.

* `CharacterAnimatorSpriter.cs`  (UP_USE_SPRITER)

  Require [SpriterDotNet](https://github.com/loodakrawa/SpriterDotNet).Unity

* `DefaultInput.cs` (UP_USE_CN_INPUT_MANAGER)

  Require [CnControls](https://www.assetstore.unity3d.com/en/#!/content/15233) from the UnityStore.

<a name="execution-order"></a>
### Execution Order

Configuration & UpdateManager need to be executed first, as they are
Singletons, many classes rely on them being initialized first.

Go to: Edit/ProjectSettings/Script Execution Order and add them with negative
priority.

This is already done in this project, but don't forget it if you use your custom
project setting.

### Tagging

Tags can be mixed to create hybrid behaviours: like moving platforms that are
one way platforms.

This is achieved having a tag that contains both names. ex: `MovingPlatform&OneWayPlatforms`, & is not mandatory I use it for readability
purposes.

### Manual Editable

The following classes contains stuff that could be useful to edit for your game
and cannot be extended.

#### Areas.cs

Contains `Areas` Enum. Where the characters can be in.

#### States.cs

Contains `States` Enum. States the Character can be.
When you add c State remember to invalidate other states the Character can be
at the same time `Character.EnterState`& `Character.ExistState`.

This is crucial information for Animation.

#### DamageType.cs

Contains `DamageTypes` Enum.


# License

License is MIT Copyright © 2016 Luis Lafuente Morales <llafuente@noboxout.com>

Except a few files that are shared with Sebastian (https://github.com/SebLague)
