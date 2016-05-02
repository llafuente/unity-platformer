# unity-platformer
===

Based on https://github.com/SebLague/2DPlatformer-Tutorial evolve
in it's own beast.

## Features
* Moving platforms.
* One way platforms.
* Ladders.
* AI (Patrol, Projectiles, Jumpers, etc...)
* Input abstraction (this will help if you add a pad or touch controls)
* Character-handled-actions. Instead of a big class mapping all actions,
each Action (Jump, Climb ladder, ground/air movement) it's a separate component.
* Projectiles.
* Melee attacks
* Wallstick/WallJump.
* Update everything in order (UpdateManager). This allow total control over
who is updated first and act accordingly.
* Slopes

## Known issues

[https://github.com/llafuente/unity-platformer/labels/bug](https://github.com/llafuente/unity-platformer/labels/bug)

## TODO

[https://github.com/llafuente/unity-platformer/issues?q=is%3Aissue+is%3Aopen+label%3Aenhancement](https://github.com/llafuente/unity-platformer/issues?q=is%3Aissue+is%3Aopen+label%3Aenhancement)

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

OWP Height should be around `Character.skinWidth` ~ 0.1-0.2

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
action you add you can achieve many behaviors.

<a name="external-libraries"></a>
### External libraries

`unity-platformer` don't require any external library, but I used in my
projects and shared the code here.

When a class require a library everything is enclosed inside `#if/endif`
that you will find in the first line.

* `CharacterAnimatorSpriter.cs`  (UP_USE_SPRITER)

  Enabled by default (sorry ^^)

  Require [SpriterDotNet](https://github.com/loodakrawa/SpriterDotNet).Unity


* `DefaultInput.cs` (UP_USE_CN_INPUT_MANAGER)

  Require [CnControls](https://www.assetstore.unity3d.com/en/#!/content/15233) from the UnityStore.

<a name="execution-order"></a>
### Execution Order

Configuration & UpdateManager need to be executed first, as they are
Singletons, many classes rely on them being initialized first.

Go to: Edit/ProjectSettings/Script Execution Order and add them with negative
priority.

This is already done in this project, Just for reference is you integrate
the codebase in your project.

### Tagging

Tags can be mixed to create hybrid behaviours: like moving platforms that are
one way platforms.

This is achieved having a tag that contains both names. ex: `MovingPlatform&OneWayPlatforms`

### Manual Editable

The following classes contains stuff that could be useful to edit for your game.

#### Character.cs

Contains `Areas` and `States`. Both useful for new Actions/Movements/Animation.

#### DamageType.cs

Contains `DamageTypes` Enum.


# License

License is MIT Copyright © 2016 Luis Lafuente Morales <llafuente@noboxout.com>

Except a few files that are shared with Sebastian (https://github.com/SebLague)
