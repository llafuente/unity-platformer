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
