# unity-platformer
===

Based on https://github.com/SebLague/2DPlatformer-Tutorial evolve
in it's own beast.

## Features

* Moving platforms
* One way platforms
* Ladders
* AI (Patrol, Projectiles, Jumpers, etc...)
* Component based Actions. Instead of a big class mapping all actions,
each Action (Jump, Climb ladder, ground/air movement) it's a separate component.
* Projectiles
* Melee attacks
* Wallstick/WallJump
* Push boxes
* Water (liquid) bouyancy
* Climb/Descent Slopes

* Input abstraction (this will help if you add a pad or touch controls)
* Update everything in order (UpdateManager).

  This allow total control over who is updated first and act accordingly

## Known issues

[https://github.com/llafuente/unity-platformer/labels/bug](https://github.com/llafuente/unity-platformer/labels/bug)

## TODO

[https://github.com/llafuente/unity-platformer/issues?q=is%3Aissue+is%3Aopen+label%3Aenhancement](https://github.com/llafuente/unity-platformer/issues?q=is%3Aissue+is%3Aopen+label%3Aenhancement)

## Caveats

[https://github.com/llafuente/unity-platformer/labels/caveat](https://github.com/llafuente/unity-platformer/labels/caveat)

## Wiki

The wiki contains the documentation along with the issues above

* [Home](/llafuente/unity-platformer/wiki)
* [Character setup](/llafuente/unity-platformer/wiki/Character-setup)
* [FAQ](/llafuente/unity-platformer/wiki/FAQ)
* [Usage](/llafuente/unity-platformer/wiki/Usage)

## Hotswapping

`unity-platformer` support (mostly) hotswapping.

There are some limitations, like the character current actions is lost: Example: If character is grabbing will fall.

# License

License is MIT Copyright © 2016 Luis Lafuente Morales <llafuente@noboxout.com>

Except a few files that are shared with Sebastian (https://github.com/SebLague)
