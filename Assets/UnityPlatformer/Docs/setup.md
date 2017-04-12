Setup {#setup}
============

# Project/Scene setup

Project can be setup using `Configuration` It's a Singleton MonoBehaviour
that you must attach to every scene.
Also attach UpdateManager and your scene is ready.

We really recommend to create a prefab with UpdateManager & Configuration.

## Configuration

Each scene can have different configuration, be vary careful when loading
a scene into because cannot use `Additive`.

Configure:

* Min distance to env
* Gravity
* Tags
* Layers


## UpdateManager

`unity-platformer` has it's own update `loop`.
`UpdateManager` it the loop owner, use `FixedUpdate` but none other class will.

If you want something to be updated by `UpdateManager` implement: `IUpdateEntity`
Basically has two methods: `PlatformerUpdate(float delta)` and
`LatePlatformerUpdate(float delta)`
Also implements OnEnable to attach the class to the `UpdateManager`
And implements OnDisable to detach


Why?
* Secuencial update, we are sure that Unity don't modify anything in the
middle of the simulation, something like call OnEnterTrigger2D is dangerous.
* Be precise in updating order, for example: move platforms before characters.
* Support time scale changes, Coroutines are not reliable.
`UpdateManager.SetTimeout` and `UpdateManager.SetInterval` are.
* Be able to survive Hotswapping


## Real Physics vs Fake Manual/Physics

We develop our custom manual collision, but it's proven to not be 100% reliable.

Problems raise when something is moving in your world and it's not a
MovingPlatform, or it's rotating. This kind of collision can penetrate
into Characters colliders and destroy the simulation.

To solve this there is two aproachs:

* Reduce Fixed Timestep to something near `0.002`. This will increment the
simulation time a lot, performance drop, but the simulation will be more
stable.
* Let Unity physics handle the hard part to avoid penetrations.
This also present many chalenges because some parts of our manual collision
cannot be ported straight so right now it's not the recommended way.

So in the end we will have a mixed physics.


## No graphics

There is no graphic here, just game-logic.


## Execution Order {#execution-order}

`unity-platformer` prioritize Configuration & UpdateManager to be
executed first, as they are Singletons,
many classes rely on them being initialized before.


## External libraries {#external-libraries}

`unity-platformer` require UnityTestTools or remove the `Assets/Test` folder.

The rest of libraries are optional. And handled with a macros

* `CharacterAnimatorSpriter.cs`

  macro: UP_USE_SPRITER

  Require [SpriterDotNet](https://github.com/loodakrawa/SpriterDotNet).Unity

* `DefaultInput.cs`

  macro: UP_USE_CN_INPUT_MANAGER

  Require [CnControls](https://www.assetstore.unity3d.com/en/#!/content/15233)
from the UnityStore.

  macro: UP_USE_WII_INPUT_MANAGER

  Require [Unity-Wiimote](https://github.com/Flafla2/Unity-Wiimote). This
enable the usage of a single wiimote (no nunchuck atm)

## Tagging

Tags are configured at `Configuration` component.

Tags can be mixed to create hybrid behaviours: like moving platforms that are
one way platforms.

This is achieved having a tag that contains both names.
ex: `MovingPlatform&OneWayPlatforms`, & is not mandatory
I use it for readability
purposes.


## prefabs

To include prefabs in the scene use `InstancePrefab`.
It's not mandatory, but it will save space in your .scene and you will be sure
that there is no change in the prefab.

`InstancePrefab` can be extended like what we do with `PlayerStart` to
setup the camera.

Also useful for testing like: `TestInputPatrolJumping`,
`TestInputPatrolLadder` etc.
